using proj.InnerObjects;
using proj.Interfaces;
using proj.Storage;
using proj.Utilities;

namespace proj.InputParsing;

public class BinaryPackedFlight : IStorage.SourcePacket, IVisitor
{
    // ------------------------------
    // Class creation
    // ------------------------------

    public BinaryPackedFlight(byte[] rawBytes)
    {
        _msg = rawBytes;
    }
    
    // ------------------------------
    // Class interaction
    // ------------------------------
    
    public override bool BuildObject(IStorage storage)
    {
        _storage = storage;
        (string ident, byte[] otherBytes) = GetIdent(_msg);
        FlightsSystemObject obj = FactoryMethods[ident]();
        _msg = otherBytes;
        
        bool result = obj.ProcessVisitor(this);

        if (result == false)
            Logger.Log($"Item with ident: {ident} failed to be build!");
        
        return result;
    }
    
    public bool Visit(Airport airport)
    {
        airport.ID = BitConverter.ToUInt64(_msg, 0);
        UInt16 nl = BitConverter.ToUInt16(_msg, 8);
        airport.Name = ConvToStr(_msg[10..(10 + nl)]);
        airport.Code = ConvToStr(_msg[(10 + nl)..(13 + nl)]);

        var pos = new GeographicPosition();
        var rawBytes = pos.Parse(_msg[(13 + nl)..]);
        
        airport.GeoPos = pos;
        airport.Country = ConvToStr(rawBytes[..3]);

        return _storage.Store(airport);
    }

    public bool Visit(Cargo cargo)
    {
        cargo.ID = BitConverter.ToUInt64(_msg[..8]);
        cargo.Weight = BitConverter.ToSingle(_msg[8..12]);
        cargo.Code = ConvToStr(_msg[12..18]);
        UInt16 dl = BitConverter.ToUInt16(_msg[18..20]);
        cargo.Description = ConvToStr(_msg[20..(20 + dl)]);

        return _storage.Store(cargo);
    }

    public bool Visit(CargoPlane cargoPlane)
    {
        var rawBytes = cargoPlane.Parse(_msg);
        cargoPlane.MaxLoad = BitConverter.ToSingle(rawBytes[..4]);

        return _storage.Store(cargoPlane);
    }

    public bool Visit(Crew crew)
    {
        var rawBytes = crew.Parse(_msg);
        crew.Practice = BitConverter.ToUInt16(rawBytes[..2]);
        crew.Role = ConvToStr(rawBytes[2..3]);

        return _storage.Store(crew);
    }

    public bool Visit(Flight flight)
    {
        UInt64 plainId = BitConverter.ToUInt64(_msg[40..48]);

        Flight tempFlight;
        if (_storage.Get(plainId, out CargoPlane cargoPlane) && (tempFlight = _initCargoFlight(cargoPlane)) != null) {}
        else if (_storage.Get(plainId, out PassengerPlane passengerPlane) && (tempFlight = _initPassengerFlight(passengerPlane)) != null) {}
        else return false;
        flight = tempFlight;
        
        flight.ID = BitConverter.ToUInt64(_msg[..8]);
         
        ulong origin = BitConverter.ToUInt64(_msg[8..16]);

        if (_storage.Get(origin, out Airport originAirport) == false)
            return false;
        
        ulong target = BitConverter.ToUInt64(_msg[16..24]);
        
        if (_storage.Get(target, out Airport targetAirport) == false)
            return false;

        flight.OriginAirport = originAirport;
        flight.TargetAirport = targetAirport;

        Int64 takeoffEpoch = BitConverter.ToInt64(_msg[24..32]);
        Int64 landingEpoch = BitConverter.ToInt64(_msg[32..40]);
        flight.TakeOffTime = DateTime.UnixEpoch.AddMilliseconds(takeoffEpoch);
        flight.LandingTime = DateTime.UnixEpoch.AddMilliseconds(landingEpoch);

        if (flight.LandingTime < flight.TakeOffTime)
            flight.LandingTime = flight.LandingTime.AddDays(1);
        
        UInt16 cc = BitConverter.ToUInt16(_msg[48..50]);
        int ccBytes = cc * sizeof(UInt64);
        var crewIDs = new ulong[cc];
        Buffer.BlockCopy(_msg[50..(50 + ccBytes)], 0, crewIDs, 0, 8*cc);

        var crews = new List<Crew>(crewIDs.Length);
        foreach(var cid in crewIDs)
            if (_storage.Get(cid, out Crew crew))
                crews.Add(crew);
            else
                return false;
        
        flight.Crew = crews.ToArray();

        return _storage.Store(flight);
    }

    public bool Visit(Passenger passenger)
    {
        var rawBytes = passenger.Parse(_msg);
        passenger.Class = ConvToStr(rawBytes[..1]);
        passenger.Miles = BitConverter.ToUInt64(rawBytes[1..]);

        return _storage.Store(passenger);
    }

    public bool Visit(PassengerPlane passengerPlane)
    {
        var rawBytes = passengerPlane.Parse(_msg);
        passengerPlane.FirstClassSize = BitConverter.ToUInt16(rawBytes[..2]);
        passengerPlane.BusinessClassSize = BitConverter.ToUInt16(rawBytes[2..4]);
        passengerPlane.EconomyClassSize = BitConverter.ToUInt16(rawBytes[4..6]);

        return _storage.Store(passengerPlane);
    }
    
    // ------------------------------
    // Static methods
    // ------------------------------
    
    public static string ConvToStr(byte[] bytes)
        => System.Text.Encoding.ASCII.GetString(bytes).TrimEnd('\0');

    private static (string, byte[]) GetIdent(byte[] bytes)
    {
        var ident = ConvToStr(bytes[..3]);
        UInt32 msgLength = BitConverter.ToUInt32(bytes[3..7]);

        if (bytes.Length - 7 != msgLength)
            throw new ApplicationException(
                $"[ ERROR ] Received incomplete message, written msg length: {msgLength},\n" +
                $"but real msg length is: {bytes.Length - 7}!");
        
        if (!FactoryMethods.ContainsKey(ident))
            throw new ApplicationException($"[ ERROR ] Received unknown identifier: {ident}");

        return (ident, bytes[7..]);
    }
    
    // ------------------------------
    // Private methods
    // ------------------------------
    
    private ulong[] _retreiveLoadIDs()
    {
        UInt16 cc = BitConverter.ToUInt16(_msg[48..50]);
        int ccBytes = cc * sizeof(UInt64);
        UInt16 lc = BitConverter.ToUInt16(_msg[(50 + ccBytes)..(52 + ccBytes)]);
        var load = new ulong[lc];
        Buffer.BlockCopy(_msg[(52 + ccBytes)..], 0, load, 0, lc*sizeof(UInt64));
        return load;
    }

    private CargoFlight _initCargoFlight(CargoPlane plane)
    {
        var rv = new CargoFlight();
        rv.Plane = plane;

        var loadIDs = _retreiveLoadIDs();
        var cargos = new List<Cargo>(loadIDs.Length);
        foreach (var lid in loadIDs)
            if (_storage.Get(lid, out Cargo cargo))
                cargos.Add(cargo);
            else return null;

        rv.Load = cargos.ToArray();
        
        return rv;
    }

    private PassengerFlight _initPassengerFlight(PassengerPlane plane)
    {
        var rv = new PassengerFlight();
        rv.Plane = plane;

        var loadIDs = _retreiveLoadIDs();
        var passengers = new List<Passenger>(loadIDs.Length);
        foreach (var lid in loadIDs)
            if (_storage.Get(lid, out Passenger passenger))
                passengers.Add(passenger);
            else return null;

        rv.Load = passengers.ToArray();
        
        return rv;
    }
    
    // ------------------------------
    // private fields
    // ------------------------------
    
    private static readonly Dictionary<string, Func<FlightsSystemObject>> FactoryMethods = new()
    {
        { "NCR", () => new Crew() },
        { "NPA", () => new Passenger() },
        { "NCA", () => new Cargo() },
        { "NCP", () => new CargoPlane() },
        { "NPP", () => new PassengerPlane() },
        { "NAI", () => new Airport() },
        { "NFL", () => new Flight() }
    };

    private byte[] _msg;
    private IStorage _storage = null!;
}
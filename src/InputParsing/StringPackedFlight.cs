using proj.InnerObjects;
using proj.Interfaces;
using proj.Storage;
using proj.Utilities;

namespace proj.InputParsing;

public partial class Parser
{
    // ------------------------------
    // Class inner types
    // ------------------------------
    
    public class StringPackedFlight : IStorage.SourcePacket, IVisitor
    {
        // ------------------------------
        // Class creation
        // ------------------------------
        
        public StringPackedFlight(Func<string, List<string>> formatArrayExtractor, List<string> stringValues, Func<FlightsSystemObject> creator)
        {
            _formatArrayExtractor = formatArrayExtractor;
            _stringValues = stringValues;
            _creator = creator;
        }
        
        // ------------------------------
        // Class interaction
        // ------------------------------

        public override bool BuildObject(IStorage storage)
        {
            _storage = storage;
            var obj = _creator();
            bool result = obj.ProcessVisitor(this);

            if (result == false)
                Logger.Log("Failed to built object from string packed bundle!");

            return result;
        }

        public bool Visit(Airport airport)
        {
            if (_stringValues.Count != 7) throw new Exception("Wrong argument count");

            airport.ID = UInt64.Parse(_stringValues[0]);
            airport.Name = _stringValues[1];
            airport.Code = _stringValues[2];

            var geoPos = new GeographicPosition();
            geoPos.Parse(_stringValues[3..6]);
            airport.GeoPos = geoPos;
            airport.Country = _stringValues[6];

            return _storage.Store(airport);
        }

        public bool Visit(Cargo cargo)
        {
            if (_stringValues.Count != 4) throw new Exception("Wrong argument count");

            cargo.ID = UInt64.Parse(_stringValues[0]);
            cargo.Weight = Single.Parse(_stringValues[1]);
            cargo.Code = _stringValues[2];
            cargo.Description = _stringValues[3];

            return _storage.Store(cargo);
        }

        public  bool Visit(CargoPlane cargoPlane)
        {
            if (_stringValues.Count != 5) throw new Exception("Wrong argument count");

            cargoPlane.Parse(_stringValues[..4]);
            cargoPlane.MaxLoad = Single.Parse(_stringValues[4]);

            return _storage.Store(cargoPlane);
        }

        public  bool Visit(Crew crew)
        {
            if (_stringValues.Count != 7) throw new Exception("Wrong argument count");

            crew.Parse(_stringValues[..5]);
            crew.Practice = UInt16.Parse(_stringValues[5]);
            crew.Role = _stringValues[6];

            return _storage.Store(crew);
        }

        public bool Visit(Flight flight)
        {
            if (_stringValues.Count != 11) throw new Exception("Wrong argument count");
            var plainId = UInt64.Parse(_stringValues[8]);
            
            Flight tempFlight;
            if (_storage.Get(plainId, out CargoPlane cargoPlane) && (tempFlight = _initCargoFlight(cargoPlane)) != null) {}
            else if (_storage.Get(plainId, out PassengerPlane passengerPlane) && (tempFlight = _initPassengerFlight(passengerPlane)) != null) {}
            else return false;
            flight = tempFlight;
            
            flight.ID = UInt64.Parse(_stringValues[0]);
            
            ulong origin = UInt64.Parse(_stringValues[1]);

            if (_storage.Get(origin, out Airport originAirport) == false)
                return false;
        
            ulong target = UInt64.Parse(_stringValues[2]);
        
            if (_storage.Get(target, out Airport targetAirport) == false)
                return false;
            
            flight.OriginAirport = originAirport;
            flight.TargetAirport = targetAirport;
            
            flight.TakeOffTime = DateTime.Parse(_stringValues[3]);
            flight.LandingTime = DateTime.Parse(_stringValues[4]);
            
            if (flight.LandingTime < flight.TakeOffTime)
                flight.LandingTime = flight.LandingTime.AddDays(1);
            
            var geoPos = new GeographicPosition();
            geoPos.Parse(_stringValues[5..8]);
            flight.GeoPos = geoPos;
            
            var crewIds = ParseArray<UInt64>(_formatArrayExtractor(_stringValues[9]).ToArray());
            var crews = new List<Crew>(crewIds.Length);
            foreach (var cid in crewIds)
                if (_storage.Get(cid, out Crew crew))
                    crews.Add(crew);
                else return false;
            flight.Crew = crews.ToArray();

            return _storage.Store(flight);
        }

        public bool Visit(Passenger passenger)
        {
            if (_stringValues.Count != 7) throw new Exception("Wrong argument count");

            passenger.Parse(_stringValues[..5]);
            passenger.Class = _stringValues[5];
            passenger.Miles = UInt64.Parse(_stringValues[6]);

            return _storage.Store(passenger);
        }

        public bool Visit(PassengerPlane passengerPlane)
        {
            if (_stringValues.Count != 7) throw new Exception("Wrong argument count");

            passengerPlane.Parse(_stringValues[..4]);
            passengerPlane.FirstClassSize = UInt16.Parse(_stringValues[4]);
            passengerPlane.BusinessClassSize = UInt16.Parse(_stringValues[5]);
            passengerPlane.EconomyClassSize = UInt16.Parse(_stringValues[6]);

            return _storage.Store(passengerPlane);
        }
        
        // ------------------------------
        // Private methods
        // ------------------------------
        
        private ulong[] _retreiveLoadIDs()
            =>  ParseArray<UInt64>(_formatArrayExtractor(_stringValues[10]).ToArray());

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
        // Class private fields
        // ------------------------------

        private Func<string, List<string>> _formatArrayExtractor;
        private List<string> _stringValues;
        private Func<FlightsSystemObject> _creator;
        private IStorage _storage = null!;
    }
}

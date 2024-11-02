using System.Text.Json.Serialization;
using proj.InnerObjects;
using proj.Serialization;

namespace proj.Storage;

/*          SHORT SUMMARY 
 *  Class introduces structured fast and convinient way of storing FlightSystem objects.
 */

public class ObjectsDB : IStorage
{
    // ------------------------------
    // Class interaction
    // ------------------------------
    
    // Performs serialization accordingly to passed Serialization format specified inside the SerializationMachine
    public void SerializeDB(string filename, SerializationMachine machine)
    {
        Consolidate();
        
        lock(this)
            machine.Serialize(this, filename);
    }

    // Simply pushes item into wait, which correctness should be checked during next consolidation
    public void PushItemToWaitList(IStorage.SourcePacket obj)
    {
        lock (_waitList)
            _waitList.Enqueue(obj);
    }

    public void PushUpdateToWaitList(IStorage.Update obj)
    {
        lock (_updateWaitList)
            _updateWaitList.Enqueue(obj);
    }

    // Methods perform consolidation of the database process all items stored inside the wait list and if it is possible
    // simply adds them into the correct dictionary if not, items are pushed back to the wait list.
    public void Consolidate()
    {
        lock (this)
        {
            var objectToConsolidate = _waitList;
            lock (_waitList)
                _waitList = new();

            Queue<IStorage.SourcePacket> failures = new();
        
            // first run
            while (objectToConsolidate.Count != 0)
            {
                var obj = objectToConsolidate.Dequeue();
                
                bool result;
                try
                {
                    result = obj.BuildObject(this);
                }
                catch (Exception exc)
                {
                    Console.Error.WriteLine($"[ ERROR ] During object building inside db encountered error: {exc.Message}!");
                    continue;
                }
                
                if (result == false)
                    failures.Enqueue(obj);
            }
        
            // second run
            while (failures.Count != 0)
            {
                var obj = failures.Dequeue();
            
                bool result;
                try
                {
                    result = obj.BuildObject(this);
                }
                catch (Exception exc)
                {
                    Console.Error.WriteLine($"[ ERROR ] During object building inside db encountered error: {exc.Message}!");
                    continue;
                }

                if (result == false)
                    lock (_waitList)
                        _waitList.Enqueue(obj);
            }
        }
    }

    public void ApplyUpdates()
    {
        lock (this)
        {
            var updates = _updateWaitList;
            lock (_updateWaitList)
                _updateWaitList = new();

            while (updates.Count > 0)
            {
                var update = updates.Dequeue();
                update.Perform(this);
            }
        }
    }

        public IEnumerable<Flight> GetFlights()
            => _flights.Values;

        public IEnumerable<Airport> GetAirports()
            => _airports.Values;
        
        public IEnumerable<CargoPlane> GetCargoPlanes()
            => _cargoPlanes.Values;
        
        public IEnumerable<PassengerPlane> GetPassengerPlanes()
            => _passengerPlanes.Values;

        public IEnumerable<Crew> GetCrew()
            => _crews.Values;
        
        public IEnumerable<Passenger> GetPassengers()
            => _passengers.Values;

        public IEnumerable<Cargo> GetCargos()
            => _cargos.Values;
    
    // ------------------------------
    // IStorage implementation
    // ------------------------------
    
    public bool Store(Airport airport)
        => _airports.TryAdd(airport.ID, airport);

    public bool Store(Cargo cargo)
        => _cargos.TryAdd(cargo.ID, cargo);

    public bool Store(CargoPlane cargoPlane)
        => _cargoPlanes.TryAdd(cargoPlane.ID, cargoPlane);

    public bool Store(Crew crew)
        => _crews.TryAdd(crew.ID, crew);

    public bool Store(Flight flight)
    { 
        bool res = _flights.TryAdd(flight.ID, flight);
        
        if (res)
            OnNewFlightArrival?.Invoke(this, flight);

        return res;
    }

    public bool Store(Passenger passenger)
        => _passengers.TryAdd(passenger.ID, passenger);

    public bool Store(PassengerPlane passengerPlane)
        => _passengerPlanes.TryAdd(passengerPlane.ID, passengerPlane);

    public bool RemoveAirport(ulong ID)
        => _airports.Remove(ID);

    public bool RemoveCargo(ulong ID)
        => _cargos.Remove(ID);

    public bool RemoveCargoPlane(ulong ID)
        => _cargoPlanes.Remove(ID);

    public bool RemoveCrew(ulong ID)
        => _crews.Remove(ID);

    public bool RemoveFlight(ulong ID)
        => _flights.Remove(ID);

    public bool RemovePassenger(ulong ID)
        => _passengers.Remove(ID);

    public bool RemovePassengerPlane(ulong ID)
        => _passengerPlanes.Remove(ID);
    
    public bool Visit(Airport airport)
        => Store(airport);

    public bool Visit(Cargo cargo)
        => Store(cargo);

    public bool Visit(CargoPlane cargoPlane)
        => Store(cargoPlane);

    public bool Visit(Crew crew)
        => Store(crew);

    public bool Visit(Flight flight)
        => Store(flight);

    public bool Visit(Passenger passenger)
        => Store(passenger);

    public bool Visit(PassengerPlane passengerPlane)
        => Store(passengerPlane);
    
    public bool Get(ulong ID, out Airport airport)
        => _airports.TryGetValue(ID, out airport!);

    public bool Get(ulong ID, out Cargo cargo)
        => _cargos.TryGetValue(ID, out cargo!);

    public bool Get(ulong ID, out CargoPlane cargoPlane)
        => _cargoPlanes.TryGetValue(ID, out cargoPlane!);

    public bool Get(ulong ID, out Crew crew)
        => _crews.TryGetValue(ID, out crew!);

    public bool Get(ulong ID, out Flight flight)
        => _flights.TryGetValue(ID, out flight!);

    public bool Get(ulong ID, out Passenger passenger)
        => _passengers.TryGetValue(ID, out passenger!);

    public bool Get (ulong ID, out PassengerPlane passengerPlane)
        => _passengerPlanes.TryGetValue(ID, out passengerPlane!);


    // ------------------------------
    // Private class methods
    // ------------------------------

    // ------------------------------
    // Class fields
    // ------------------------------

    public event EventHandler<Flight>? OnNewFlightArrival; 
    
    private Queue<IStorage.SourcePacket> _waitList = new();
    private Queue<IStorage.Update> _updateWaitList = new();
    
    [JsonInclude]
    private readonly Dictionary<UInt64, Airport> _airports = new();
    [JsonInclude]
    private readonly Dictionary<UInt64, Cargo> _cargos = new();
    [JsonInclude]
    private readonly Dictionary<UInt64, Passenger> _passengers = new();
    [JsonInclude]
    private readonly Dictionary<UInt64, Crew> _crews = new();
    [JsonInclude]
    private readonly Dictionary<UInt64, CargoPlane> _cargoPlanes = new();
    [JsonInclude]
    private readonly Dictionary<UInt64, PassengerPlane> _passengerPlanes = new();
    [JsonInclude]
    private readonly Dictionary<UInt64, Flight> _flights = new ();
}
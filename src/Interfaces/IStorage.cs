using proj.InnerObjects;
using proj.Interfaces;
using proj.Utilities;

namespace proj.Storage;

/*
 *          SHORT SUMMARY
 *
 *  Interface implementing so called visitor pattern, used to correctly match relations between
 *  objects inside the database
 */

public interface IStorage: IVisitor
{
    
    // ------------------------------
    // Interface inner types
    // ------------------------------

    public abstract class SourcePacket
    {
        public abstract bool BuildObject(IStorage storage);
    }
    
    public abstract class Update
    {
        public abstract bool Perform(IStorage storage);
        public abstract bool Reverse(IStorage storage);
        public abstract string Serialize();

        public void Log(bool IsSuccess)
        {
            string result = Serialize();
            Logger.Log($"{(IsSuccess ? "[ SUCCESS ]" : "[ ERROR ]"),12}{result}");
        }
    }
    
    // -----------------------------------
    // Interface main funcionalities
    // -----------------------------------
    
    public bool Store(Airport airport);
    public bool Store(Cargo cargo);
    public bool Store(CargoPlane cargoPlane);
    public bool Store(Crew crew);
    public bool Store(Flight flight);
    public bool Store(Passenger passenger);
    public bool Store(PassengerPlane passengerPlane);
    
    public bool RemoveAirport(UInt64 ID);
    public bool RemoveCargo(UInt64 ID);
    public bool RemoveCargoPlane(UInt64 ID);
    public bool RemoveCrew(UInt64 ID);
    public bool RemoveFlight(UInt64 ID);
    public bool RemovePassenger(UInt64 ID);
    public bool RemovePassengerPlane(UInt64 ID);
    
    public bool Get(UInt64 ID, out Airport airport);
    public bool Get(UInt64 ID, out Cargo cargo);
    public bool Get(UInt64 ID, out CargoPlane cargoPlane);
    public bool Get(UInt64 ID, out Crew crew);
    public bool Get(UInt64 ID, out Flight flight);
    public bool Get(UInt64 ID, out Passenger passenger);
    public bool Get(UInt64 ID, out PassengerPlane passengerPlane);

    public bool Get(UInt64 ID, out FlightsSystemObject obj, 
        out Func<IStorage, UInt64, FlightsSystemObject?> getOut,
        out Func<IStorage, UInt64, bool> removeOut)
    {
        obj = null;
        getOut = null;
        removeOut = null;

        FlightsSystemObject? fo = null;
        foreach (var (getter, remover) in _accessors)
        {
            fo = getter(this, ID);
            if (fo != null)
            {
                obj = fo;
                getOut = getter;
                removeOut = remover;
                break;
            }
        }

        return fo != null;
    }
    
    // ------------------------------
    // Interface Fields
    // ------------------------------

    private static List<
        (Func<IStorage, UInt64, FlightsSystemObject?> get, Func<IStorage, UInt64, bool> remove)
    > _accessors = new()
    {
        ((store, id) => store.Get(id, out Airport o) ? o : null, (store, id) => store.RemoveAirport(id)),
        ((store, id) => store.Get(id, out Cargo o) ? o : null, (store, id) => store.RemoveCargo(id)),
        ((store, id) => store.Get(id, out CargoPlane o) ? o : null, (store, id) => store.RemoveCargoPlane(id)),
        ((store, id) => store.Get(id, out Crew o) ? o : null, (store, id) => store.RemoveCrew(id)),
        ((store, id) => store.Get(id, out Flight o) ? o : null, (store, id) => store.RemoveFlight(id)),
        ((store, id) => store.Get(id, out Passenger o) ? o : null, (store, id) => store.RemovePassenger(id)),
        ((store, id) => store.Get(id, out PassengerPlane o) ? o : null, (store, id) => store.RemovePassengerPlane(id)),
    };
}
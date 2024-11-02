using proj.Interfaces;
using proj.SqlCommands;
using proj.Storage;

namespace proj.InnerObjects;

public class Passenger: Human, Interfaces.IQueryable<Passenger>
{
    // ----------------------------------------
    // FlightsSystemObject implementation
    // ----------------------------------------
    
    public override bool ProcessVisitor(IVisitor visitor)
        => visitor.Visit(this);
    
    public static IEnumerable<Passenger> QueryDB(ObjectsDB db)
        => db.GetPassengers();
    
    public static bool ContainsID(ObjectsDB db, UInt64 ID)
        => db.Get(ID, out Passenger o);
    
    public static bool RemoveID(ObjectsDB db, UInt64 ID)
        => db.RemovePassenger(ID);

    // ---------------------------------
    // Class fields and properties
    // ---------------------------------

    public string Class { get; set; } = string.Empty;
    public UInt64 Miles { get; set; }
    
    public static Dictionary<string, PropertyWrapper<Passenger>> Properties { get; } = new()
    {
        ["ID"] = new NumericWrapper<UInt64, Passenger>(((passenger, val) => passenger.ID = val), passenger => passenger.ID),
        ["Name"] = new StringWrapper<Passenger>(((passenger, s) => passenger.Name = s), passenger => passenger.Name),
        ["Class"] = new StringWrapper<Passenger>(((passenger, s) => passenger.Class = s), passenger => passenger.Class),
        ["Miles"] = new NumericWrapper<UInt64, Passenger>(((passenger, val) => passenger.Miles = val), passenger => passenger.Miles),
        ["Age"] = new NumericWrapper<UInt64, Passenger>(((passenger, val) => passenger.Age = val), passenger => passenger.Age),
        ["Phone"] = new StringWrapper<Passenger>(((passenger, s) => passenger.Phone = s), passenger => passenger.Phone),
        ["Email"] = new StringWrapper<Passenger>(((passenger, s) => passenger.Email = s), passenger => passenger.Email)
    };
}
using proj.Interfaces;
using proj.SqlCommands;
using proj.Storage;

namespace proj.InnerObjects;

public class Cargo: FlightsSystemObject, Interfaces.IQueryable<Cargo>
{
    // ----------------------------------------
    // FlightsSystemObject implementation
    // ----------------------------------------
    
    public override bool ProcessVisitor(IVisitor visitor)
        => visitor.Visit(this);
    
    public static IEnumerable<Cargo> QueryDB(ObjectsDB db)
        => db.GetCargos();
    
    public static bool ContainsID(ObjectsDB db, UInt64 ID)
        => db.Get(ID, out Cargo o);
    
    public static bool RemoveID(ObjectsDB db, UInt64 ID)
        => db.RemoveCargo(ID);

    // ---------------------------------
    // Class fields and properties
    // ---------------------------------

    public Single Weight { get; set; }
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    public static Dictionary<string, PropertyWrapper<Cargo>> Properties { get; } = new()
    {
        ["ID"] = new NumericWrapper<UInt64, Cargo>(((cargo, val) => cargo.ID = val), cargo => cargo.ID),
        ["Code"] = new StringWrapper<Cargo>(((cargo, s) => cargo.Code = s), cargo => cargo.Code),
        ["Description"] = new StringWrapper<Cargo>(((cargo, s) => cargo.Description = s), cargo => cargo.Description),
        ["Weight"] = new NumericWrapper<Single, Cargo>(((cargo, f) => cargo.Weight = f), cargo => cargo.Weight)
    };

}
using proj.Interfaces;
using proj.SqlCommands;
using proj.Storage;

namespace proj.InnerObjects;

public class Crew: Human, Interfaces.IQueryable<Crew>
{
    // ----------------------------------------
    // FlightsSystemObject implementation
    // ----------------------------------------
    
    public override bool ProcessVisitor(IVisitor visitor)
        => visitor.Visit(this);
    
    public static IEnumerable<Crew> QueryDB(ObjectsDB db)
        => db.GetCrew();
    
    public static bool ContainsID(ObjectsDB db, UInt64 ID)
        => db.Get(ID, out Crew o);
    
    public static bool RemoveID(ObjectsDB db, UInt64 ID)
        => db.RemoveCrew(ID);

    // ---------------------------------
    // Class fields and properties
    // ---------------------------------

    public UInt16 Practice{ get; set; }
    public string Role { get; set; } = string.Empty;
    
    public static Dictionary<string, PropertyWrapper<Crew>> Properties { get; } = new()
    {
        ["ID"] = new NumericWrapper<UInt64, Crew>(((crew, val) => crew.ID = val), crew => crew.ID),
        ["Name"] = new StringWrapper<Crew>(((crew, s) => crew.Name = s), crew => crew.Name),
        ["Age"] = new NumericWrapper<UInt64, Crew>(((crew, val) => crew.Age = val), crew => crew.Age),
        ["Phone"] = new StringWrapper<Crew>(((crew, s) => crew.Phone = s), crew => crew.Phone),
        ["Email"] = new StringWrapper<Crew>(((crew, s) => crew.Email = s), crew => crew.Email),
        ["Practice"] = new NumericWrapper<UInt16, Crew>(((crew, arg2) => crew.Practice = arg2), crew => crew.Practice),
        ["Role"] = new StringWrapper<Crew>(((crew, s) => crew.Role = s), crew => crew.Role)
    };
}
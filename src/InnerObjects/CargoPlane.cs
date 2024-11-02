using proj.Interfaces;
using proj.Media_Objects;
using proj.SqlCommands;
using proj.Storage;

namespace proj.InnerObjects;

public class CargoPlane: Plane, IReportable, Interfaces.IQueryable<CargoPlane>
{
    // ----------------------------------------
    // FlightsSystemObject implementation
    // ----------------------------------------
    
    public override bool ProcessVisitor(IVisitor visitor)
        => visitor.Visit(this);
    
    public string AcceptReporter(Reporter reporter)
        => reporter.Report(this);
    
    public static IEnumerable<CargoPlane> QueryDB(ObjectsDB db)
        => db.GetCargoPlanes();
    
    public static bool ContainsID(ObjectsDB db, UInt64 ID)
        => db.Get(ID, out CargoPlane o);
    
    public static bool RemoveID(ObjectsDB db, UInt64 ID)
        => db.RemoveCargoPlane(ID);

    // ---------------------------------
    // Class fields and properties
    // ---------------------------------

    public Single MaxLoad { get; set; }
    
    public static Dictionary<string, PropertyWrapper<CargoPlane>> Properties { get; } = new()
    {
        ["ID"] = new NumericWrapper<UInt64, CargoPlane>(((plane, val) => plane.ID = val), plane => plane.ID),
        ["Model"] = new StringWrapper<CargoPlane>(((plane, s) => plane.Model = s), plane => plane.Model),
        ["CountryCode"] = new StringWrapper<CargoPlane>(((plane, s) => plane.Country = s), plane => plane.Country),
        ["Serial"] = new StringWrapper<CargoPlane>(((plane, s) => plane.Serial = s), plane => plane.Serial),
        ["MaxLoad"] = new NumericWrapper<Single, CargoPlane>(((plane, f) => plane.MaxLoad = f), plane => plane.MaxLoad)
    };
}
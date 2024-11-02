using proj.Interfaces;
using proj.Media_Objects;
using proj.SqlCommands;
using proj.Storage;

namespace proj.InnerObjects;

public class PassengerPlane: Plane, IReportable, Interfaces.IQueryable<PassengerPlane>
{
    // ----------------------------------------
    // FlightsSystemObject implementation
    // ----------------------------------------
    
    public override bool ProcessVisitor(IVisitor visitor)
        => visitor.Visit(this);

    public string AcceptReporter(Reporter reporter)
        => reporter.Report(this);
    
    public static IEnumerable<PassengerPlane> QueryDB(ObjectsDB db)
        => db.GetPassengerPlanes();

    public static bool ContainsID(ObjectsDB db, UInt64 ID)
        => db.Get(ID, out PassengerPlane o);

    public static bool RemoveID(ObjectsDB db, UInt64 ID)
        => db.RemovePassengerPlane(ID);

    // ---------------------------------
    // Class fields and properties
    // ---------------------------------

    public UInt16 FirstClassSize { get; set; }
    public UInt16 BusinessClassSize { get; set; }
    public UInt16 EconomyClassSize { get; set; }

    public UInt32 FullSize => (UInt32)FirstClassSize + BusinessClassSize + EconomyClassSize;

    public static Dictionary<string, PropertyWrapper<PassengerPlane>> Properties { get; } = new()
    {
        ["ID"] = new NumericWrapper<UInt64, PassengerPlane>(((plane, val) => plane.ID = val), plane => plane.ID),
        ["FirstClassSize"] = new NumericWrapper<UInt16,PassengerPlane>(((plane, val) => plane.FirstClassSize = val), plane => plane.FirstClassSize),
        ["BusinessClassSize"] = new NumericWrapper<UInt16,PassengerPlane>(((plane, val) => plane.BusinessClassSize = val), plane => plane.BusinessClassSize),
        ["EconomyClassSize"] = new NumericWrapper<UInt16,PassengerPlane>(((plane, val) => plane.EconomyClassSize = val), plane => plane.EconomyClassSize),
        ["Model"] = new StringWrapper<PassengerPlane>(((plane, s) => plane.Model = s), plane => plane.Model),
        ["CountryCode"] = new StringWrapper<PassengerPlane>(((plane, s) => plane.Country = s), plane => plane.Country),
        ["Serial"] = new StringWrapper<PassengerPlane>(((plane, s) => plane.Serial = s), plane => plane.Serial)
    };
}
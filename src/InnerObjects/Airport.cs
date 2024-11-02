using proj.Interfaces;
using proj.Media_Objects;
using proj.SqlCommands;
using proj.Storage;

namespace proj.InnerObjects;

public class Airport: Traceable, IReportable, Interfaces.IQueryable<Airport>
{
    // ------------------------------
    // ILoadable implementation
    // ------------------------------

    public override bool ProcessVisitor(IVisitor visitor)
        => visitor.Visit(this);
    
    public string AcceptReporter(Reporter reporter)
        => reporter.Report(this);

    public static IEnumerable<Airport> QueryDB(ObjectsDB db)
        => db.GetAirports();
    
    public static bool ContainsID(ObjectsDB db, UInt64 ID)
        => db.Get(ID, out Airport o);
    
    public static bool RemoveID(ObjectsDB db, UInt64 ID)
        => db.RemoveAirport(ID);

    // ---------------------------------
    // Class fields and properties
    // ---------------------------------
    
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public static Dictionary<string, PropertyWrapper<Airport>> Properties { get; } = new()
    {
        ["ID"] = new NumericWrapper<UInt64, Airport>(((airport, val) => airport.ID = val), airport => airport.ID),
        ["Name"] = new StringWrapper<Airport>(((airport, val) => airport.Name = val), airport => airport.Name),
        ["Code"] = new StringWrapper<Airport>(((airport, val) => airport.Code = val), airport => airport.Code),
        ["CountryCode"] = new StringWrapper<Airport>(((airport, val) => airport.Country = val), airport => airport.Country),
        ["AMSL"] = new NumericWrapper<Single, Airport>(((airport, val) => airport.AMSL = val), airport => airport.GeoPos.AMSL),
        ["WorldPosition.Lat"] = new NumericWrapper<Single, Airport>(((airport, val) => airport.Latitude = val), airport => airport.GeoPos.Latitude),
        ["WorldPosition.Long"] = new NumericWrapper<Single, Airport>(((airport, val) => airport.Longitude = val), airport => airport.GeoPos.Longitude),
    };
}
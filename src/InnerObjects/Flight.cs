using System.Text.Json.Serialization;
using System.Xml.Serialization;
using proj.Interfaces;
using proj.SqlCommands;
using proj.Storage;

namespace proj.InnerObjects;

[Serializable]
[JsonDerivedType(typeof(PassengerFlight), "PassengerFlight")]
[JsonDerivedType(typeof(CargoFlight), "CargoFlight")]
[XmlInclude(typeof(PassengerFlight))]
[XmlInclude(typeof(CargoFlight))]
public class Flight: Traceable, Interfaces.IQueryable<Flight>
{
    // ----------------------------------------
    // FlightsSystemObject implementation
    // ----------------------------------------
    
    public override bool ProcessVisitor(IVisitor visitor)
        => visitor.Visit(this);
    
    public static IEnumerable<Flight> QueryDB(ObjectsDB db)
        => db.GetFlights();
    
    public static bool ContainsID(ObjectsDB db, UInt64 ID)
        => db.Get(ID, out Flight o);
    
    public static bool RemoveID(ObjectsDB db, UInt64 ID)
        => db.RemoveFlight(ID);
    
    // ------------------------------
    // Class interaction
    // ------------------------------

    public void UpdatePosition(DateTime time)
    {
        DateTime startTime = _stamp ?? TakeOffTime;
        Single totalTime = (Single)(LandingTime - startTime).TotalSeconds;
        Single actTime = (Single)(time - startTime).TotalSeconds;

        if (actTime < 0)
        {
            GeoPos = OriginAirport.GeoPos;
            return;
        }

        if (actTime > totalTime)
        {
            GeoPos = TargetAirport.GeoPos;
            return;
        }

        Single coef = actTime / totalTime;
        Single dx = TargetAirport.GeoPos.Latitude - GeoPos.Latitude;
        Single dy = TargetAirport.GeoPos.Longitude - GeoPos.Longitude;
        
        GeoPos = new GeographicPosition(
            GeoPos.Latitude + coef*dx,
            GeoPos.Longitude + coef*dy,
            TargetAirport.GeoPos.AMSL
        );
        _stamp = time;
    }

    // ---------------------------------
    // Class fields and properties
    // ---------------------------------

    public DateTime TakeOffTime { get; set; }
    public DateTime LandingTime { get; set; }
    public Crew[] Crew { get; set; } = [];
    
    public Airport OriginAirport { get; set; } = new Airport()!;

    public Airport TargetAirport { get; set; } = new Airport()!;

    private DateTime? _stamp = null;
    
    public static Dictionary<string, PropertyWrapper<Flight>> Properties { get; } = new()
    {
        ["ID"] = new NumericWrapper<UInt64, Flight>(((flight, val) => flight.ID = val), flight => flight.ID),
        ["AMSL"] = new NumericWrapper<Single, Flight>(((flight, val) => flight.AMSL = val), flight => flight.GeoPos.AMSL),
        ["WorldPosition.Lat"] = new NumericWrapper<Single, Flight>(((flight, val) => flight.Latitude = val), flight => flight.GeoPos.Latitude),
        ["WorldPosition.Long"] = new NumericWrapper<Single, Flight>(((flight, val) => flight.Longitude = val), flight => flight.GeoPos.Longitude),
        ["TakeOffTime"] = new DateWrapper<Flight>((flight, time) => flight.TakeOffTime = time, (flight => flight.TakeOffTime)),
        ["LandingTime"] = new DateWrapper<Flight>((flight, time) => flight.LandingTime = time, (flight => flight.LandingTime)),
        ["Origin.ID"] = new NumericWrapper<UInt64, Flight>(((flight, val) => { }), flight => flight.OriginAirport.ID),
        ["Target.ID"] = new NumericWrapper<UInt64, Flight>(((flight, val) => { }), flight => flight.TargetAirport.ID),
        ["Origin.Name"] = new StringWrapper<Flight>(((flight, val) => flight.OriginAirport.Name = val), flight => flight.OriginAirport.Name),
        ["Target.Name"] = new StringWrapper<Flight>(((flight, val) => flight.TargetAirport.Name = val), flight => flight.TargetAirport.Name),
        ["Origin.Code"] = new StringWrapper<Flight>(((flight, val) => flight.OriginAirport.Code = val), flight => flight.OriginAirport.Code),
        ["Target.Code"] = new StringWrapper<Flight>(((flight, val) => flight.TargetAirport.Code = val), flight => flight.TargetAirport.Code),
        ["Origin.CountryCode"] = new StringWrapper<Flight>(((flight, val) => flight.OriginAirport.Country = val), flight => flight.OriginAirport.Country), 
        ["Target.CountryCode"] = new StringWrapper<Flight>(((flight, val) => flight.TargetAirport.Country = val), flight => flight.TargetAirport.Country),
        ["Origin.AMSL"] = new NumericWrapper<Single, Flight>(((flight, val) => flight.OriginAirport.AMSL = val), flight => flight.OriginAirport.GeoPos.AMSL),
        ["Target.AMSL"] = new NumericWrapper<Single, Flight>(((flight, val) => flight.TargetAirport.AMSL = val), flight => flight.TargetAirport.GeoPos.AMSL),
        ["Origin.WorldPosition.Lat"] = new NumericWrapper<Single, Flight>(((flight, val) => flight.OriginAirport.Latitude = val), flight => flight.OriginAirport.GeoPos.Latitude),
        ["Origin.WorldPosition.Long"] = new NumericWrapper<Single, Flight>(((flight, val) => flight.OriginAirport.Longitude = val), flight => flight.OriginAirport.GeoPos.Longitude),
        ["Target.WorldPosition.Lat"] = new NumericWrapper<Single, Flight>(((flight, val) => flight.TargetAirport.Latitude = val), flight => flight.TargetAirport.GeoPos.Latitude),
        ["Target.WorldPosition.Long"] = new NumericWrapper<Single, Flight>(((flight, val) => flight.TargetAirport.Longitude = val), flight => flight.TargetAirport.GeoPos.Longitude),
    };
}
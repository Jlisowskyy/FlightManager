namespace proj.InnerObjects;

public abstract class Traceable: FlightsSystemObject
{
    public GeographicPosition GeoPos { get; set; }

    public Single AMSL
    {
        set => GeoPos = new GeographicPosition(GeoPos.Latitude, GeoPos.Longitude, value);
    }
    
    public Single Latitude
    {
        set => GeoPos = new GeographicPosition(value, GeoPos.Longitude, GeoPos.AMSL);
    }
    
    public Single Longitude
    {
        set => GeoPos = new GeographicPosition(GeoPos.Latitude, value, GeoPos.AMSL);
    }
}
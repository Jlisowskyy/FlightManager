namespace proj.InnerObjects;

public struct GeographicPosition
{
    public GeographicPosition(Single latitude, Single longitude, Single amsl)
    {
        Latitude = latitude;
        Longitude = longitude;
        AMSL = amsl;
        IsKnown = true;
    }
    
    
    public void Parse(List<string> stringValues)
    {
        IsKnown = true;
        Longitude = Single.Parse(stringValues[0]);
        Latitude = Single.Parse(stringValues[1]);
        AMSL = Single.Parse(stringValues[2]);
    }

    public byte[] Parse(byte[] bytes)
    {
        IsKnown = true;
        Longitude = BitConverter.ToSingle(bytes[..4]);
        Latitude = BitConverter.ToSingle(bytes[4..8]);
        AMSL = BitConverter.ToSingle(bytes[8..12]);
        return bytes[12..];
    }

    public bool IsKnown { get; set; }
    public Single Longitude { get; set; }
    public Single Latitude { get; set; }
    public Single AMSL { get; set; }
}
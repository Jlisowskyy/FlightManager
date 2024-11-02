using proj.InputParsing;

namespace proj.InnerObjects;

public abstract class Plane: FlightsSystemObject
{
    public void Parse(List<string> stringValues)
    {
        ID = UInt64.Parse(stringValues[0]);
        Serial = stringValues[1];
        Country = stringValues[2];
        Model = stringValues[3];
    }

    public byte[] Parse(byte[] bytes)
    {
        ID = BitConverter.ToUInt64(bytes[..8]);
        Serial = BinaryPackedFlight.ConvToStr(bytes[8..18]);
        Country = BinaryPackedFlight.ConvToStr(bytes[18..21]);
        UInt16 ml = BitConverter.ToUInt16(bytes[21..23]);
        Model = BinaryPackedFlight.ConvToStr(bytes[23..(23 + ml)]);
        return bytes[(23 + ml)..];
    }

    public string Serial { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;

    public string Name => $"{Serial} - {Model}";
}
using proj.InputParsing;

namespace proj.InnerObjects;

public abstract class Human: FlightsSystemObject
{
    public void Parse(List<string> stringValues)
    {
        ID = UInt64.Parse(stringValues[0]);
        Name = stringValues[1];
        Age = UInt64.Parse(stringValues[2]);
        Phone = stringValues[3];
        Email = stringValues[4];
    }

    public byte[] Parse(byte[] bytes)
    {
        ID = BitConverter.ToUInt64(bytes[..8]);
        UInt16 nl = BitConverter.ToUInt16(bytes[8..10]);
        Name = BinaryPackedFlight.ConvToStr(bytes[10..(10+nl)]);
        Age = BitConverter.ToUInt16(bytes[(10 + nl)..(12 + nl)]);
        Phone = BinaryPackedFlight.ConvToStr(bytes[(12 + nl)..(24 + nl)]);
        UInt16 el = BitConverter.ToUInt16(bytes[(24 + nl)..(26 + nl)]);
        Email = BinaryPackedFlight.ConvToStr(bytes[(26 + nl)..(26 + nl + el)]);
        return bytes[(26 + nl + el)..];
    }
    
    public string Name { get; set; } = string.Empty;
    public UInt64 Age { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
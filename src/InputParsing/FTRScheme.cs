using proj.InnerObjects;

namespace proj.InputParsing;

public class FtrScheme: ParserScheme
{
    // ------------------------------------
    // Parser Scheme methods override
    // ------------------------------------
    
    public override (string, List<string>) GetRecordsAndIdent(string cleanedLine)
    {
        var records = new List<string>(
            cleanedLine.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));

        // in case split returned one element records[1..] will become string[0];
        return (records[0], records[1..]);
    }
    
    public override List<string> ParseFormatArray(string array)
    {
        // correctness checking
        if (array[0] != '[' || array[^1] != ']')
            throw new Exception("Received invalid array");

        // cleaning beginning and ending bracket
        array = array[1..^1];

        return [..array.Split(';')];
    }
    
    // ------------------------------
    // Class fields
    // ------------------------------

    public override Dictionary<string, Func<FlightsSystemObject>> FactoryMethods => new(_factoryMethods);
    
    private readonly Dictionary<string, Func<FlightsSystemObject>> _factoryMethods = new()
    {
        { "C", () => new Crew() },
        { "P", () => new Passenger() },
        { "CA", () => new Cargo() },
        { "CP", () => new CargoPlane() },
        { "PP", () => new PassengerPlane() },
        { "AI", () => new Airport() },
        { "FL", () => new Flight() }
    };
}
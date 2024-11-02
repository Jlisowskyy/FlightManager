using System.Globalization;
using proj.InnerObjects;
using proj.Storage;

namespace proj.InputParsing;

/*
 * Parser expects to receive ParserScheme during construction
 * to process files in valid way. 
 */


public partial class Parser
{
    // ------------------------------
    // Class creation
    // ------------------------------

    // "scheme" determines desired format to be parsed,
    // when "reportEvents" is true during parsing errors are reported to the stdout
    public Parser(ParserScheme scheme, bool reportEvents = false)
    {
        _scheme = scheme;
        _reportEvents = reportEvents;
        _factoryMethods = scheme.FactoryMethods;
    } 
    
    // ------------------------------
    // Class interaction
    // ------------------------------

    public IEnumerable<IStorage.SourcePacket> Parse(string filename)
        => _parseFile(filename);
    
    // Simply parsers list of string into desired type list
    public static T[] ParseArray<T>(string[] strings) where T : IParsable<T>
    {
        var outArray = new T[strings.Length];

        for (long i = 0; i < strings.Length; ++i)
            outArray[i] = T.Parse(strings[i], CultureInfo.InvariantCulture);

        return outArray;
    }
    

    // ------------------------------
    // Private class methods
    // ------------------------------

    private IEnumerable<IStorage.SourcePacket> _parseFile(string filename)
    {
        using var inputReader = new StreamReader(filename);
        long line = 0;
        
        // main parsing loop - simply reads line after line until EOF is met
        for (string? lineBuffer ; (lineBuffer = inputReader.ReadLine()) != null ;)
        {
            line++;
            var lineResult = _processLine(lineBuffer, line, filename);

            // skipping invalid or empty line
            if (lineResult == null) continue;

            yield return lineResult;
        }
    }

    private StringPackedFlight? _processLine(string lineBuffer, long lineCount, string filename)
    {
        string cleanedLine = lineBuffer.Trim();
            
        // skipping empty line
        if (cleanedLine == string.Empty) return null;

        var (objectIdentifier, records) = _scheme.GetRecordsAndIdent(cleanedLine);
        
        // unrecognized data type or invalid line
        if (_factoryMethods.ContainsKey(objectIdentifier) == false)
        {
            if (_reportEvents)
                Console.Error.WriteLine($"[ WARN ] Line {lineCount}, which is contained inside {filename} file," +
                                    $" contains unrecognized identifier: {objectIdentifier}\n");
            return null;
        }
        
        return new StringPackedFlight(_scheme.ParseFormatArray, records, _factoryMethods[objectIdentifier]);
    }
    
    // ------------------------------
    // Class fields
    // ------------------------------

    private readonly ParserScheme _scheme;
    private readonly bool _reportEvents;
    private readonly Dictionary<string, Func<FlightsSystemObject>> _factoryMethods;
}
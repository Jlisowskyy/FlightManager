using proj.InnerObjects;

namespace proj.InputParsing;

/*
 *  Abstract class, which defines how specific format works.
 *  Is used as pattern to process files inside Parser.
 */

public abstract class ParserScheme
{
    // Returns object inner format identifier and list of sequenced records inside the line.
    public abstract (string, List<string>) GetRecordsAndIdent(string cleanedLine);
    
    // Returns copy of factory methods dictionary which associates them with their identifiers.
    public abstract Dictionary<string, Func<FlightsSystemObject>> FactoryMethods { get; }
    
    // Method used to parser inner format array representation.
    public abstract List<string> ParseFormatArray(string array);
}
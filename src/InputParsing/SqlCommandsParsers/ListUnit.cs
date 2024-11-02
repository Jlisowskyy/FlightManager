namespace proj.InputParsing.SqlCommandsParsers;

public class ListUnit : SqlSyntaxUnit
{
// ------------------------------
// Class creation
// ------------------------------

    public ListUnit(List<string> output, SqlSyntaxUnit nextUnit) : base(nextUnit)
    {
        if (nextUnit == null || output == null)
            throw new ArgumentException("Null value is not allowed");

        _output = output;
    }

// ------------------------------
// Class interaction
// ------------------------------

    public override void Parse(string[] tokens)
    {
        int ind;
        int expect = 0;
        
        for (ind = 0; ind < tokens.Length; ind++)
        {
            // expect the token to be an identifier
            if (expect == 0)
            {
                if (SqlParser.IsSpecial(tokens[ind]))
                    throw new Exception($"Expected identifier, got special character: {tokens[ind]}");
                
                // add the token to the output
                _output.Add(tokens[ind]);
            }
            else if (tokens[ind] != ",")
                break;
            
            expect ^= 1;
        }
        
        if (expect == 0)
            throw new Exception("Invalid list format");
        
        _nextUnit?.Parse(tokens[ind..]);
    }
    
// ------------------------------
// CLass Fields
// ------------------------------

    private readonly List<string> _output;
}
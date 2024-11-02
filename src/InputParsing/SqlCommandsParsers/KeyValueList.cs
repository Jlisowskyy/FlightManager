namespace proj.InputParsing.SqlCommandsParsers;

public class KeyValueList : SqlSyntaxUnit
{
// ------------------------------
// Class creation
// ------------------------------
 
    public KeyValueList(List<(string, string)> output, SqlSyntaxUnit? nextUnit) : base(nextUnit)
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
        
        if (tokens[0] != "(")
            throw new Exception($"Expected '(', got: {tokens[0]}");
        
        for (ind = 1; ind < tokens.Length; ind++)
        {
            // expect the tokens to be a key-value pair
            if (expect == 0)
            {
                if (!_doesFit(ind, tokens))
                    throw new Exception("Expected key-value pair, got end of input");
                
                if (tokens[ind+1] != "=")
                    throw new Exception($"Expected '=', got {tokens[ind+1]}");
                
                if (SqlParser.IsSpecial(tokens[ind]) || SqlParser.IsSpecial(tokens[ind + 2]))
                    throw new Exception($"Expected identifier, got special character: {
                        (SqlParser.IsSpecial(tokens[ind]) ? tokens[ind] : tokens[ind + 2])
                    }");
                
                // add the key-value pair to the output
                _output.Add((tokens[ind], tokens[ind+2]));
                ind += 2;
            }
            else if (tokens[ind] != ",")
                break;
            
            expect ^= 1;
        }
        
        if (expect == 0)
            throw new Exception("Invalid list format");
        
        if (ind == tokens.Length || tokens[ind] != ")")
            throw new Exception("Expected ')' at the end of the list");
        
        _nextUnit?.Parse(tokens[++ind..]);
    }
    
// ------------------------------
// private methods
// ------------------------------

    private static bool _doesFit(int ind, string[] tokens)
        => (tokens.Length - ind) >= 3;
    
// ------------------------------
// CLass Fields
// ------------------------------
    
    private readonly List<(string, string)> _output;
}
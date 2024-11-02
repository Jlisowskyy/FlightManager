namespace proj.InputParsing.SqlCommandsParsers;

public class ConditionalUnit: SqlSyntaxUnit
{
// ------------------------------
// Inner types
// ------------------------------

    public enum ConditionalLogic
    {
        And,
        Or
    }
    
    public enum ConditionalOperator
    {
        Equal, // '='
        NotEqual, // '!='
        Greater, // '>'
        GreaterEqual, // '>='
        Less, // '<' 
        LessEqual // '<='
    }

// ------------------------------
// Class creation
// ------------------------------
    public ConditionalUnit(List<(string, string, ConditionalOperator)> conditions,
        List<ConditionalLogic> logicalOperators,
        SqlSyntaxUnit? nextUnit) : base(nextUnit)
    {
        if (nextUnit == null || conditions == null || logicalOperators == null)
            throw new ArgumentException("Null value is not allowed");

        _conditions = conditions;
        _logicalOperators = logicalOperators;
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
            // expect the tokens to be a key-value pair
            if (expect == 0)
                ind = _processCondition(ind, tokens);
            else if (_processLogicalOperator(ind, tokens))
                break;
            
            expect ^= 1;
        }
        
        if (expect == 0)
            throw new Exception("Invalid list format");
        
        _nextUnit?.Parse(tokens[ind..]);
    }
    
// ------------------------------
// Private methods
// ------------------------------

    int _processCondition(int ind, string[] tokens)
    {
        if (!_doesFit(ind, tokens))
            throw new Exception("Expected key-value pair, got end of input");
        
        if (SqlParser.IsSpecial(tokens[ind]))
            throw new Exception($"Expected identifier, got special character: {tokens[ind]}");

        var tkn1 = tokens[ind++];

        if (!SqlParser.IsSpecial(tokens[ind]))
            throw new Exception($"Expected conditional operator got {tokens[ind]}");
        
        var oper = tokens[ind++];
        
        if (SqlParser.IsSpecial(tokens[ind]))
        {
            oper += tokens[ind++];
            
            if (ind == tokens.Length)
                throw new Exception("Expected identifier, got end of input");
            
            if (SqlParser.IsSpecial(tokens[ind]))
                throw new Exception($"Expected identifier, got special character: {tokens[ind]}");
        }
        
        var tkn2 = tokens[ind];
        
        if (ConditionalOperators.TryGetValue(oper, out var operT))
            _conditions.Add((tkn1, tkn2, operT));
        else
            throw new Exception($"Unknown conditional operator: {oper}");

        return ind;
    }
    
    // true - abort the loop, false - continue
    bool _processLogicalOperator(int ind, string[] tokens)
    {
        if (LogicDescriptors.TryGetValue(tokens[ind], out var logic))
        {
            _logicalOperators.Add(logic);
            return false;
        }

        return true;
    }
    
    private static bool _doesFit(int ind, string[] tokens)
        => (tokens.Length - ind) >= 3;
    
// ------------------------------
// CLass Fields
// ------------------------------

    private readonly List<(string, string, ConditionalOperator)> _conditions;
    private readonly List<ConditionalLogic> _logicalOperators;

    private static readonly Dictionary<string, ConditionalLogic> LogicDescriptors = new() { 
        ["and"] = ConditionalLogic.And,
        ["or"] = ConditionalLogic.Or
    };
    
    private static readonly Dictionary<string, ConditionalOperator> ConditionalOperators = new() {
        ["="] = ConditionalOperator.Equal,
        ["!="] = ConditionalOperator.NotEqual,
        [">"] = ConditionalOperator.Greater,
        [">="] = ConditionalOperator.GreaterEqual,
        ["<"] = ConditionalOperator.Less,
        ["<="] = ConditionalOperator.LessEqual
    };
}
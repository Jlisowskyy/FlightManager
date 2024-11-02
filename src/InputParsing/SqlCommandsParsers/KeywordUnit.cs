namespace proj.InputParsing.SqlCommandsParsers;

public class KeywordUnit : SqlSyntaxUnit
{
// ------------------------------
// Class creation
// ------------------------------

    public KeywordUnit(string keyword, bool isOptional, SqlSyntaxUnit nextUnit) : base(nextUnit)
    {
        if (nextUnit == null)
            throw new ArgumentException("Null value is not allowed");
        
        _isOptional = isOptional;
        _keyword = keyword;
    }
    
// ------------------------------
// Class interaction
// ------------------------------

    public override void Parse(string[] tokens)
    {
        if (tokens.Length == 0)
        {
            if (!_isOptional)
                throw new Exception($"Expected a keyword: {_keyword}");
            return;
        }

        if (tokens[0] != _keyword)
            throw new Exception($"Expected a keyword: {_keyword}, but received: {tokens[0]}");

        _nextUnit!.Parse(tokens[1..]);
    }
    
// ------------------------------
// CLass Fields
// ------------------------------
    
    private readonly string _keyword;
    private readonly bool _isOptional;
}
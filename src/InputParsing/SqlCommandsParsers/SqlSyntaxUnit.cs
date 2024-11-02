namespace proj.InputParsing.SqlCommandsParsers;

public abstract class SqlSyntaxUnit
{
// ------------------------------
// Class creation
// ------------------------------

    protected SqlSyntaxUnit(SqlSyntaxUnit? nextUnit)
        => _nextUnit = nextUnit;
    
// ------------------------------
// Class interaction
// ------------------------------

    public abstract void Parse(string[] tokens);

// ------------------------------
// CLass Fields
// ------------------------------

    protected readonly SqlSyntaxUnit? _nextUnit;
}
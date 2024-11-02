namespace proj.InputParsing.SqlCommandsParsers;

public class CommandEndUnit : SqlSyntaxUnit
{
    public CommandEndUnit() : base(null)
    {}

    public override void Parse(string[] tokens)
    {
        if (tokens.Length != 0)
            throw new ArgumentException("End of command expected");
    }
}
using proj.InputParsing.SqlCommandsParsers;
using proj.Storage;

namespace proj.SqlCommands;

public abstract class SqlCommand
{
    public abstract void Execute(ObjectsDB db);
    public abstract void Build();
    public abstract SqlSyntaxUnit GetParser();
}
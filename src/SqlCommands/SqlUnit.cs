using proj.Storage;

namespace proj.SqlCommands;

public abstract class SqlUnit
{
    public abstract void Execute(ObjectsDB db);

    public virtual void Finalize(ObjectsDB db)
    {
    }
}
using proj.InnerObjects;
using proj.Storage;

namespace proj.SqlCommands;

public abstract class ActionUnit<T> where T : FlightsSystemObject, Interfaces.IQueryable<T>, new()
{
    public abstract bool Act(T obj);
    public virtual void Finalize(ObjectsDB db) {}
}
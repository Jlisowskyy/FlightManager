using proj.InnerObjects;
using proj.Storage;

namespace proj.SqlCommands;

public class DeleteActionUnit<T>(List<UInt64> ids): ActionUnit<T>
    where T : FlightsSystemObject, Interfaces.IQueryable<T>, new()
{
    public override bool Act(T obj)
    {
        ids.Add(obj.ID);
        return true;
    }

    public override void Finalize(ObjectsDB db)
    {
        foreach (UInt64 id in ids)
            T.RemoveID(db, id);
    }
}
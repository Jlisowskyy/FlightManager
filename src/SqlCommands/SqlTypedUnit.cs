using proj.InnerObjects;
using proj.Storage;

namespace proj.SqlCommands;

public class SqlTypedUnit<T>(FilterUnit<T>? filter, ActionUnit<T> action) : SqlUnit
    where T : FlightsSystemObject, Interfaces.IQueryable<T>, new()
{
// ------------------------------
// Class interaction
// ------------------------------

    public sealed override void Execute(ObjectsDB db)
    {
        var objects = T.QueryDB(db);

        if (filter == null)
            foreach (var obj in objects)
                action.Act(obj);
        else
            foreach (var obj in objects)
                if (filter.Filter(obj))
                    action.Act(obj);
    }

    public sealed override void Finalize(ObjectsDB db)
        => action.Finalize(db);
}
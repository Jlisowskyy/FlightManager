using proj.InnerObjects;
using proj.Storage;

namespace proj.SqlCommands;

public class SqlUpdateTypedUnit<T>(FilterUnit<T>? filter, List<(string, string)> keyValuePairs) : SqlUnit
    where T : FlightsSystemObject, Interfaces.IQueryable<T>, Interfaces.IVisitable, new()
{
// ------------------------------
// Class interaction
// ------------------------------

    public sealed override void Execute(ObjectsDB db)
    {
        var objects = T.QueryDB(db);
        bool checkForIDIntegrity = false;
        
        // Check whether all given fields exists
        foreach (var kp in keyValuePairs)
        {
            checkForIDIntegrity |= kp.Item1 == "ID";
            if (!T.Properties.ContainsKey(kp.Item1))
                throw new Exception($"Given class does not contain field: {kp.Item1}");
        }
        
        // Move id request to front
        if (checkForIDIntegrity)
        {
            using var enumerator = objects.GetEnumerator();

            int ind = keyValuePairs.FindIndex((s) => s.Item1 == "ID");
            var prev = keyValuePairs[ind];
            keyValuePairs.RemoveAt(ind);
            keyValuePairs.Insert(0, prev);

            List<T> readyItems;
            if (filter == null)
                readyItems = new(objects.ToArray());
            else
            {
                readyItems = new();

                foreach (var obj in objects)
                    if (filter.Filter(obj))
                        readyItems.Add(obj);
            }
            

            if (!_processWithID(readyItems.GetEnumerator(), db))
                throw new Exception("Not able to process requested update due to braking dependencies!");
        }
        else
            _processWoutID(objects);

    }
    
// ------------------------------
// private methods
// ------------------------------

    private bool _processWithID(IEnumerator<T> enumerator, ObjectsDB db)
    {
        while (enumerator.MoveNext())
        {
            var obj = enumerator.Current;
            if (filter == null || filter.Filter(obj))
            {
                // In starting function assumed convention
                UInt64 nID = UInt64.Parse(keyValuePairs[0].Item2);
                if (T.ContainsID(db, nID))
                    return false;

                // Applying ID changes
                UInt64 oID = obj.ID;
                obj.ID = nID;

                // Applying db changes
                T.RemoveID(db, oID);
                obj.ProcessVisitor(db);
                
                if (!_processWithID(enumerator, db))
                {
                    // reversing changes
                    obj.ID = oID;
                    T.RemoveID(db, nID);
                    obj.ProcessVisitor(db);
                    
                    return false;
                }
                
                // processing other changes
                for (int i = 1; i < keyValuePairs.Count; ++i)
                    T.Properties[keyValuePairs[i].Item1].Set(obj, keyValuePairs[i].Item2);
                return true;
            }   
        }

        return true;
    }

    private void _processWoutID(IEnumerable<T> enumerable)
    {
        foreach (var item in enumerable)
            if (filter == null || filter.Filter(item))
                foreach (var kp in keyValuePairs)
                    T.Properties[kp.Item1].Set(item, kp.Item2);
    }

}
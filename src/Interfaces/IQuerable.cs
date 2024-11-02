using proj.InnerObjects;
using proj.SqlCommands;
using proj.Storage;

namespace proj.Interfaces;

public interface IQueryable<T> where T: FlightsSystemObject, new() 
{
    public static abstract Dictionary<string, PropertyWrapper<T>> Properties { get; }
    public static abstract IEnumerable<T> QueryDB(ObjectsDB db);

    public static abstract bool ContainsID(ObjectsDB db, UInt64 ID);

    public static abstract bool RemoveID(ObjectsDB db, UInt64 ID);
}
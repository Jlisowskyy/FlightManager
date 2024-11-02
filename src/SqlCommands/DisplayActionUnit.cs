using proj.CLI;
using proj.InnerObjects;

namespace proj.SqlCommands;

public class DisplayActionUnit<T> : ActionUnit<T>
    where T : FlightsSystemObject, Interfaces.IQueryable<T>, new()
{
// ------------------------------
// Class creation
// ------------------------------

    public DisplayActionUnit(DisplayTable table, params string[] fieldsToDisplay)
    {
        _table = table;
        _fieldsToDisplay = new PropertyWrapper<T>[fieldsToDisplay.Length];
        for (int i = 0; i < fieldsToDisplay.Length; i++)
        {
            if (!T.Properties.TryGetValue(fieldsToDisplay[i], out var property))
                throw new Exception($"Property {fieldsToDisplay[i]} not found in {typeof(T)}");
            
            _fieldsToDisplay[i] = property;
        }
    }

// ------------------------------
// Class interaction
// ------------------------------

    public override bool Act(T obj)
    {
        List<string> values = new();

        foreach (var property in _fieldsToDisplay)
            values.Add(property.Get(obj));
        
        _table.AddRows(values.ToArray());

        return true;
    }

// ------------------------------
// Private class methods
// ------------------------------

// ------------------------------
// Class fields
// ------------------------------

    private readonly DisplayTable _table;
    private PropertyWrapper<T>[] _fieldsToDisplay;
}
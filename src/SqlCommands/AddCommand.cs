using proj.InnerObjects;
using proj.InputParsing.SqlCommandsParsers;
using proj.Storage;

namespace proj.SqlCommands;

public class AddCommand : SqlCommand
{
// ------------------------------
// Class inner types
// ------------------------------

    private class SqlAddUnit<T>(List<(string, string)> keyValuePairs) : SqlUnit
        where T : FlightsSystemObject, Interfaces.IQueryable<T>, new()
    {
        
// ------------------------------
// Class interaction
// ------------------------------

        public sealed override void Execute(ObjectsDB db)
        {
            T obj = new();

            var fieldsToFill = new Dictionary<string, PropertyWrapper<T>>(T.Properties);
            foreach (var kp in keyValuePairs)
            {
                if (!T.Properties.TryGetValue(kp.Item1, out var property))
                    throw new Exception($"Given property {kp.Item1} does not exists inside the {typeof(T)} class!");
                
                property.Set(obj, kp.Item2);
                fieldsToFill.Remove(kp.Item1);
            }

            if (fieldsToFill.Count != 0)
                throw new Exception($"Not all fields: {string.Join(", ", fieldsToFill.Keys)} of the class: {typeof(T)} were filled!");

            if (!obj.ProcessVisitor(db))
                throw new Exception("Object of given class with given ID already exists in the db!");
        }
    }

// ------------------------------
// Class interaction
// ------------------------------

    public override void Execute(ObjectsDB db)
    {
        if (_unit == null)
            throw new Exception("Command should be assembled by .Build method before executing!");
        
        _unit.Execute(db);
    }

    public override void Build()
    {
        if (_classes.Count != 1)
            throw new Exception("Inside the add command only 1 class is allowed, no less no more!");

        if (!Units.ContainsKey(_classes[0]))
            throw new Exception("Given class type does not exists!");

        _unit = Units[_classes[0]](_keyValuePairs);
    }   

    public override SqlSyntaxUnit GetParser()
        => new KeywordUnit("add", false, 
            new ListUnit(_classes, 
            new KeywordUnit("new", false,     
            new KeyValueList(_keyValuePairs, new CommandEndUnit() 
        ))));

// ------------------------------
// Private class methods
// ------------------------------

// ------------------------------
// Class fields
// ------------------------------

    private SqlUnit? _unit;
    private readonly List<(string, string)> _keyValuePairs = new();
    private readonly List<string> _classes = new();
    
    private static readonly Dictionary<string, Func<List<(string, string)>, SqlUnit>> Units = new()
    {
        ["Airport"] = (f) =>new SqlAddUnit<Airport>(f),
        ["Crew"] = (f) =>new SqlAddUnit<Crew>(f),
        ["Passenger"] = (f) =>new SqlAddUnit<Passenger>(f),
        ["Cargo"] = (f) =>new SqlAddUnit<Cargo>(f),
        ["CargoPlane"] = (f) =>new SqlAddUnit<CargoPlane>(f),
        ["PassengerPlane"] = (f) =>new SqlAddUnit<PassengerPlane>(f),
        ["Flight"] = (f) =>new SqlAddUnit<Flight>(f),
    };
}
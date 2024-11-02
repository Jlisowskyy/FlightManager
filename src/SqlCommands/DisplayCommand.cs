using proj.CLI;
using proj.InnerObjects;
using proj.InputParsing.SqlCommandsParsers;
using proj.Storage;

namespace proj.SqlCommands;

public class DisplayCommand : SqlCommand
{
// ------------------------------
// Inner class Types
// ------------------------------

    private abstract class DisplayBuilder
    {
        public abstract SqlUnit Build(DisplayTable table);
        public abstract SqlUnit BuildWithAllFields(out DisplayTable table);
    }

    private class DisplayTypedBuilder<T>(
        List<string> fields,
        List<(string, string, ConditionalUnit.ConditionalOperator)> conditions,
        List<ConditionalUnit.ConditionalLogic> logicalOperators
    ) : DisplayBuilder where T : FlightsSystemObject, Interfaces.IQueryable<T>, new()
    {
        public sealed override SqlUnit Build(DisplayTable table)
        {
            var filterBuilder = new FilterBuilder<T>(conditions, logicalOperators);
            
            var filter = filterBuilder.Build();
            var action = new DisplayActionUnit<T>(table, fields.ToArray());
            
            return new SqlTypedUnit<T>(filter, action);
        }

        public sealed override SqlUnit BuildWithAllFields(out DisplayTable table)
        {
            table = new DisplayTable(T.Properties.Keys.ToArray());
            fields = [..T.Properties.Keys.ToArray()];
            return Build(table);
        }
    }

// ------------------------------
// Class interaction
// ------------------------------

    public sealed override void Execute(ObjectsDB db)
    {
        if (_execUnits == null)
            throw new Exception("Command should be assembled by .Build method before executing!");
        
        foreach(var unit in _execUnits)
            unit.Execute(db);
        
        Console.WriteLine(_table);
    }

    public sealed override SqlSyntaxUnit GetParser()
        => new KeywordUnit("display", false, 
           new ListUnit(_fields, 
           new KeywordUnit("from", false,     
           new ListUnit(_classes, 
           new KeywordUnit("where", true,
           new ConditionalUnit(_conditions, _logicalOperators, 
           new CommandEndUnit()
        ))))));

    public sealed override void Build()
    {
        _execUnits = new();
        
        // Build table with all fields
        if (_fields.Count == 1 && _fields[0] == "*")
        {
            if (_classes.Count != 1)
                throw new Exception("* operator is allowed only for single class not a class list!");
            
            if (!Builders.TryGetValue(_classes[0], out var builder))
                throw new Exception($"Invalid class {_classes[0]}");
            
            _execUnits.Add(builder(_fields, _conditions, _logicalOperators).BuildWithAllFields(out DisplayTable table));
            _table = table;
            return;
        }
        
        _table = new DisplayTable(_fields.ToArray());
        foreach (var classT in _classes)
        {
            if (!Builders.TryGetValue(classT, out var builder))
                throw new Exception($"Invalid class {classT}");
            
            _execUnits.Add(builder(_fields, _conditions, _logicalOperators).Build(_table));
        }
    }

// ------------------------------
// Private class methods
// ------------------------------

// ------------------------------
// Class fields
// ------------------------------

    /*
     *  Full flow: Parse -> Build -> Execute
     *
     * Parse: Command chain
     * Build: Builder builds Command chain
     * Execute: Command chain
     *
     * Build details:
     * 1. Iterate through types, build chain for each type
     * 2. For given type iterate through fields and acquire property accessors needed for execution
     * 3. Prepare filters if necessary
     * 4. Prepare final action
     *
     * Execute details:
     * 1. Simply hit execute
     */

    private List<SqlUnit>? _execUnits;
    private DisplayTable? _table;
    private readonly List<string> _fields = new();
    private readonly List<string> _classes = new();
    private readonly List<(string, string, ConditionalUnit.ConditionalOperator)> _conditions = new();
    private readonly List<ConditionalUnit.ConditionalLogic> _logicalOperators = new();

    private static readonly Dictionary<string, Func<List<string>, List<(string, string, ConditionalUnit.ConditionalOperator)>, List<ConditionalUnit.ConditionalLogic>, DisplayBuilder>> Builders = new()
    {
        ["Airport"] = (f, c, l) =>new DisplayTypedBuilder<Airport>(f,c,l),
        ["Crew"] = (f, c, l) =>new DisplayTypedBuilder<Crew>(f,c,l),
        ["Passenger"] = (f, c, l) =>new DisplayTypedBuilder<Passenger>(f,c,l),
        ["Cargo"] = (f, c, l) =>new DisplayTypedBuilder<Cargo>(f,c,l),
        ["CargoPlane"] = (f, c, l) =>new DisplayTypedBuilder<CargoPlane>(f,c,l),
        ["PassengerPlane"] = (f, c, l) =>new DisplayTypedBuilder<PassengerPlane>(f,c,l),
        ["Flight"] = (f, c, l) =>new DisplayTypedBuilder<Flight>(f,c,l),
    };
}
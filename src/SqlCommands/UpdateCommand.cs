using proj.CLI;
using proj.InnerObjects;
using proj.InputParsing.SqlCommandsParsers;
using proj.Storage;

namespace proj.SqlCommands;

public class UpdateCommand : SqlCommand
{
// ------------------------------
// Class inner types
// ------------------------------

    private abstract class UpdateBuilder
    {
        public abstract SqlUnit Build();
    }

    private class UpdateTypedBuilder<T>(
        List<(string, string)> keyValuePairs,
        List<(string, string, ConditionalUnit.ConditionalOperator)> conditions,
        List<ConditionalUnit.ConditionalLogic> logicalOperators
    ) : UpdateBuilder where T : FlightsSystemObject, Interfaces.IQueryable<T>, new()
    {
        public sealed override SqlUnit Build()
        {
            var filterBuilder = new FilterBuilder<T>(conditions, logicalOperators);
            var filter = filterBuilder.Build();
            
            return new SqlUpdateTypedUnit<T>(filter, keyValuePairs);
        }
        
    }

// ------------------------------
// Class interaction
// ------------------------------

    public override void Execute(ObjectsDB db)
    {
        if (_execUnits == null)
            throw new Exception("Command should be assembled by .Build method before executing!");
        
        foreach(var unit in _execUnits)
            unit.Execute(db);
    }

    public override void Build()
    {
        _execUnits = new();
        
        foreach (var classT in _classes)
        {
            if (!Builders.TryGetValue(classT, out var builder))
                throw new Exception($"Invalid class {classT}");
            
            _execUnits.Add(builder(_keyValuePairs, _conditions, _logicalOperators).Build());
        }
    }

    public override SqlSyntaxUnit GetParser()
        => new KeywordUnit("update", false, 
            new ListUnit(_classes, 
            new KeywordUnit("set", false,     
        new KeyValueList(_keyValuePairs, 
            new KeywordUnit("where", true,
            new ConditionalUnit(_conditions, _logicalOperators, 
            new CommandEndUnit()
        ))))));

// ------------------------------
// Private class methods
// ------------------------------

// ------------------------------
// Class fields
// ------------------------------

    private List<SqlUnit>? _execUnits;
    private readonly List<string> _classes = new();
    private readonly List<(string, string, ConditionalUnit.ConditionalOperator)> _conditions = new();
    private readonly List<ConditionalUnit.ConditionalLogic> _logicalOperators = new();
    private readonly List<(string, string)> _keyValuePairs = new();
    
    private static Dictionary<string, Func<List<(string, string)>, List<(string, string, ConditionalUnit.ConditionalOperator)>, List<ConditionalUnit.ConditionalLogic>, UpdateBuilder>> Builders = new()
    {
        ["Airport"] = (f, c, l) =>new UpdateTypedBuilder<Airport>(f,c,l),
        ["Crew"] = (f, c, l) =>new UpdateTypedBuilder<Crew>(f,c,l),
        ["Passenger"] = (f, c, l) =>new UpdateTypedBuilder<Passenger>(f,c,l),
        ["Cargo"] = (f, c, l) =>new UpdateTypedBuilder<Cargo>(f,c,l),
        ["CargoPlane"] = (f, c, l) =>new UpdateTypedBuilder<CargoPlane>(f,c,l),
        ["PassengerPlane"] = (f, c, l) =>new UpdateTypedBuilder<PassengerPlane>(f,c,l),
        ["Flight"] = (f, c, l) =>new UpdateTypedBuilder<Flight>(f,c,l),
    };
}
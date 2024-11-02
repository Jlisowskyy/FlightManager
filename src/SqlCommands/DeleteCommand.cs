using proj.CLI;
using proj.InnerObjects;
using proj.InputParsing.SqlCommandsParsers;
using proj.Storage;

namespace proj.SqlCommands;

public class DeleteCommand : SqlCommand
{
// ------------------------------
// Class inner types
// ------------------------------

    private abstract class DeleteBuilder
    {
        public abstract SqlUnit Build();
    }

    private class DeleteTypedBuilder<T>(
        List<(string, string, ConditionalUnit.ConditionalOperator)> conditions,
        List<ConditionalUnit.ConditionalLogic> logicalOperators
    ) : DeleteBuilder where T : FlightsSystemObject, Interfaces.IQueryable<T>, new()
    {
        public sealed override SqlUnit Build()
        {
            var filterBuilder = new FilterBuilder<T>(conditions, logicalOperators);
            
            var filter = filterBuilder.Build();
            var action = new DeleteActionUnit<T>(new List<ulong>());
            
            return new SqlTypedUnit<T>(filter, action);
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

        foreach (var unit in _execUnits)
            unit.Finalize(db);
    }

    public override void Build()
    {
        _execUnits = new();
        
        foreach (var classT in _classes)
        {
            if (!Builders.TryGetValue(classT, out var builder))
                throw new Exception($"Invalid class {classT}");
            
            _execUnits.Add(builder(_conditions, _logicalOperators).Build());
        }
    }

    public override SqlSyntaxUnit GetParser()
        =>  new KeywordUnit("delete", false, 
            new ListUnit(_classes, 
            new KeywordUnit("where", true,
            new ConditionalUnit(_conditions, _logicalOperators, 
            new CommandEndUnit()
        ))));

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
    
    private static readonly Dictionary<string, Func<List<(string, string, ConditionalUnit.ConditionalOperator)>, List<ConditionalUnit.ConditionalLogic>, DeleteBuilder>> Builders = new()
    {
        ["Airport"] = ( c, l) =>new DeleteTypedBuilder<Airport>(c,l),
        ["Crew"] = ( c, l) =>new DeleteTypedBuilder<Crew>(c,l),
        ["Passenger"] = ( c, l) =>new DeleteTypedBuilder<Passenger>(c,l),
        ["Cargo"] = ( c, l) =>new DeleteTypedBuilder<Cargo>(c,l),
        ["CargoPlane"] = ( c, l) =>new DeleteTypedBuilder<CargoPlane>(c,l),
        ["PassengerPlane"] = ( c, l) =>new DeleteTypedBuilder<PassengerPlane>(c,l),
        ["Flight"] = ( c, l) =>new DeleteTypedBuilder<Flight>(c,l),
    };
}
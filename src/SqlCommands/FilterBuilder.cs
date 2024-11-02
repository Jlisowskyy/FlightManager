using proj.InnerObjects;
using proj.InputParsing.SqlCommandsParsers;

namespace proj.SqlCommands;

public class FilterBuilder<T>(List<(string, string, ConditionalUnit.ConditionalOperator)> conditions, List<ConditionalUnit.ConditionalLogic> logicalOperators)
    where T : FlightsSystemObject, Interfaces.IQueryable<T>, new()
{
// ------------------------------
// Class interaction
// ------------------------------

    public FilterUnit<T>? Build()
    {
        if (conditions.Count == 0)
            return null;

        FilterUnit<T> firstFilter =
            new FilterUnit<T>(GetProperty(conditions[^1].Item1), conditions[^1].Item3, conditions[^1].Item2);
        
        for (int i = conditions.Count - 2; i >= 0; i--)
        {
            FilterUnit<T> nextFilter = new FilterUnit<T>(GetProperty(conditions[i].Item1), conditions[i].Item3, conditions[i].Item2);
            nextFilter.SetNext(firstFilter, logicalOperators[i]);

            firstFilter = nextFilter;
        }

        return firstFilter;
    }
    
// ------------------------------
// private methods
// ------------------------------

    private PropertyWrapper<T> GetProperty(string propertyName)
    {
        if (!T.Properties.TryGetValue(propertyName, out var property))
            throw new Exception($"Property {propertyName} not found in {typeof(T)}");
        
        return property;
    }

}
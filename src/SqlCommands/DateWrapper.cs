using System.Globalization;
using proj.InnerObjects;
using proj.InputParsing.SqlCommandsParsers;

namespace proj.SqlCommands;

public class DateWrapper<ObjT>(Action<ObjT, DateTime> setter, Func<ObjT, DateTime> getter) : PropertyWrapper<ObjT>
    where ObjT : FlightsSystemObject, new()
{
// ------------------------------
// Class interaction
// ------------------------------

    public sealed override void Set(ObjT obj, string value)
        => setter(obj, DateTime.Parse(value, CultureInfo.InvariantCulture));

    public sealed override string Get(ObjT obj)
        => getter(obj).ToString(CultureInfo.InvariantCulture) ?? throw new InvalidOperationException();

    public sealed override bool Compare(ObjT obj, string value, ConditionalUnit.ConditionalOperator operation)
        => operation switch
        {
            ConditionalUnit.ConditionalOperator.Equal =>
                DateTime.Parse(value, CultureInfo.InvariantCulture) == getter(obj),
            ConditionalUnit.ConditionalOperator.NotEqual => DateTime.Parse(value, CultureInfo.InvariantCulture) !=
                                                            getter(obj),
            ConditionalUnit.ConditionalOperator.Greater => getter(obj) > 
                                                           DateTime.Parse(value, CultureInfo.InvariantCulture),
            ConditionalUnit.ConditionalOperator.GreaterEqual => getter(obj) >=
                                                                DateTime.Parse(value, CultureInfo.InvariantCulture),
            ConditionalUnit.ConditionalOperator.Less => getter(obj) < DateTime.Parse(value, CultureInfo.InvariantCulture),
            ConditionalUnit.ConditionalOperator.LessEqual => getter(obj) <=
                                                             DateTime.Parse(value, CultureInfo.InvariantCulture),
            _ => throw new Exception("Passed invalid compare operation to the DateTime Wrapper")
        };
}
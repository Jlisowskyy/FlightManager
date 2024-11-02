using System.Globalization;
using System.Numerics;
using proj.InnerObjects;
using proj.InputParsing.SqlCommandsParsers;

namespace proj.SqlCommands;

public class NumericWrapper<NumT, ObjT>(Action<ObjT, NumT> setter, Func<ObjT, NumT> getter) : PropertyWrapper<ObjT>
    where NumT : INumber<NumT>, IParsable<NumT>
    where ObjT : FlightsSystemObject, new()
{
// ------------------------------
// Class interaction
// ------------------------------

    public sealed override void Set(ObjT obj, string value)
        => setter(obj, NumT.Parse(value, CultureInfo.InvariantCulture));

    public sealed override string Get(ObjT obj)
        => getter(obj).ToString() ?? throw new InvalidOperationException();

    public sealed override bool Compare(ObjT obj, string value, ConditionalUnit.ConditionalOperator operation)
        => operation switch
        {
            ConditionalUnit.ConditionalOperator.Equal =>
                NumT.Parse(value, CultureInfo.InvariantCulture) == getter(obj),
            ConditionalUnit.ConditionalOperator.NotEqual => NumT.Parse(value, CultureInfo.InvariantCulture) !=
                                                            getter(obj),
            ConditionalUnit.ConditionalOperator.Greater => getter(obj) >
                                                           NumT.Parse(value, CultureInfo.InvariantCulture),
            ConditionalUnit.ConditionalOperator.GreaterEqual =>  getter(obj) >=
                                                                 NumT.Parse(value, CultureInfo.InvariantCulture),
            ConditionalUnit.ConditionalOperator.Less => getter(obj) < NumT.Parse(value, CultureInfo.InvariantCulture),
            ConditionalUnit.ConditionalOperator.LessEqual => getter(obj) <=
                                                             NumT.Parse(value, CultureInfo.InvariantCulture),
            _ => throw new Exception("Passed invalid compare operation to the Numeric Wrapper")
        };
}
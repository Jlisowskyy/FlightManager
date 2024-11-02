using proj.InnerObjects;
using proj.InputParsing.SqlCommandsParsers;

namespace proj.SqlCommands;

public class StringWrapper<ObjT>(Action<ObjT, string> setter, Func<ObjT, string> getter) : PropertyWrapper<ObjT>
    where ObjT : FlightsSystemObject, new()
{
// ------------------------------
// Class interaction
// ------------------------------

    public sealed override void Set(ObjT obj, string value)
        => setter(obj, value);

    public sealed override string Get(ObjT obj)
        => getter(obj);

    public sealed override bool Compare(ObjT obj, string value, ConditionalUnit.ConditionalOperator operation)
        => operation switch
        {
            ConditionalUnit.ConditionalOperator.Equal => value == getter(obj),
            ConditionalUnit.ConditionalOperator.NotEqual => value != getter(obj),
            _ => throw new Exception("Passed invalid operation to the String Wrapper")
        };
}
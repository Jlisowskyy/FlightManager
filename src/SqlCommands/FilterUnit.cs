using proj.InnerObjects;
using proj.InputParsing.SqlCommandsParsers;

namespace proj.SqlCommands;

public class FilterUnit<ObjT>(PropertyWrapper<ObjT> property, ConditionalUnit.ConditionalOperator op, string value)
    where ObjT : FlightsSystemObject, new()
{
// ------------------------------
// Class interaction
// ------------------------------

    public void SetNext(FilterUnit<ObjT> next, ConditionalUnit.ConditionalLogic logic)
    {
        NextFilter = next;
        _logic = logic;
    }

    public bool Filter(ObjT obj)
        => NextFilter == null
            ? property.Compare(obj, value, op)
            : _logic switch
            {
                ConditionalUnit.ConditionalLogic.And => property.Compare(obj, value, op) & NextFilter.Filter(obj),
                ConditionalUnit.ConditionalLogic.Or => property.Compare(obj, value, op) | NextFilter.Filter(obj),
                _ => throw new Exception("Received unknown logic operator in FilterCommand")
            };

// ------------------------------
// Class fields
// ------------------------------

    public FilterUnit<ObjT>? NextFilter { private get; set; }
    private ConditionalUnit.ConditionalLogic _logic = ConditionalUnit.ConditionalLogic.And;
}
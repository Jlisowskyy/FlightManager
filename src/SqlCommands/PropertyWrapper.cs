using proj.InnerObjects;
using proj.InputParsing.SqlCommandsParsers;

namespace proj.SqlCommands;

public abstract class PropertyWrapper<ObjT> where ObjT : FlightsSystemObject, new()
{
    public abstract void Set(ObjT obj, string value);
    public abstract string Get(ObjT obj);
    public abstract bool Compare(ObjT obj, string value, ConditionalUnit.ConditionalOperator operation);
}
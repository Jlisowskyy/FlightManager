using proj.InputParsing;
using proj.Storage;

namespace proj.ObjectsSources;

public class FileSource : ObjectSource
{
    // ------------------------------
    // Class creation
    // ------------------------------

    public FileSource(ObjectsDB db, string filepath, ParserScheme scheme) : base(db)
    {
        _parser = new Parser(scheme, true);
        _path = filepath;
    }

    // ------------------------------
    // Class interaction
    // ------------------------------

    public override void CloseSource()
    {
        _workerSource.Join();
    }

    // ------------------------------
    // Private class methods
    // ------------------------------

    protected override void _workerSourceJob()
    {
        var result = _parser.Parse(_path);
        
        foreach (var obj in result)
            AddObjectToProductionLine(obj);
    }
    
    // ------------------------------
    // Class fields
    // ------------------------------

    private readonly Parser _parser;
    private readonly string _path;
}
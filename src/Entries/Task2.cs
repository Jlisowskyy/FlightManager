using proj.CLI;
using proj.ObjectsSources;
using proj.Storage;

namespace proj.Entries;

public class Task2: EntryPoint
{
    public override void Run(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine($"[ ERROR ] There should be exactly one argument: path to desired .ftr formatted file");
            return;
        }
        
        ObjectsDB db = new();
        ObjectSource source = new TCPSimulatorSource(db, args[0], 1, 1);
        SimpleCLIParser parser = new();
        
        source.OpenSource();
        parser.ParseStream(Console.In, db);
        source.CloseSource();
    }
}
using System.Diagnostics;
using proj.InputParsing;
using proj.ObjectsSources;
using proj.Serialization;
using proj.Storage;

namespace proj.Entries;

public class Task1: EntryPoint
{
    // Performs basic tests of stage 1 functionality. Take program arguments as parameter.
    // Where args[0] is used as ftr source file and args[1] is used as serialization save file.

    public override void Run(string[] args)
    {
        Stopwatch sw = new Stopwatch();
        
        if (args.Length != 2)
        {
            Console.Error.WriteLine($"[ ERROR ] Passed not enough arguments: {args.Length}!\nShould be 2: inputFile, outputFile");
            return;
        }

        ObjectsDB db = new();
        
        // Parsing test
        sw.Start();
        FileSource source = new(db, args[0], new FtrScheme());
        source.OpenSource();
        source.CloseSource();
        sw.Stop();
        Console.WriteLine($"Program spent {sw.ElapsedMilliseconds}ms to load the file: {args[0]}");
        
        // Serializing test
        sw.Start();
        db.SerializeDB(args[1], new JsonSerializationMachine());
        sw.Stop();
        
        Console.WriteLine($"Program spent {sw.ElapsedMilliseconds}ms to serialize whole collection into json!\n");
    }
}
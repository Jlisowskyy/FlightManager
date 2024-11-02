using System.Globalization;
using proj.Entries;
using proj.Utilities;

namespace proj;

class Program
{
    static void Main(string[] args)
    {
        // IMPORTANT 
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        
        // initializing log class
        while (Logger.IsWorking == false)
        {
            Console.WriteLine("[ INFO ] Logger failed to initialize provide custom directory to store logs:");
            string dir = Console.ReadLine();
            Logger.SetupLogDir(dir);
        }
        
        // Setting additional logging parameters
        Logger.LogToStdOut = true;
        
        // Choosing stage entry point
        EntryPoint entry = new Task6(1);
        
        // Basic functionality tests
        entry.Run(args);
        
        Logger.Sync();
    }
}
using proj.InputParsing;
using proj.Interfaces;
using proj.Media_Objects;
using proj.Serialization;
using proj.Storage;

namespace proj.CLI;

public class SimpleCLIParser
{
    // ------------------------------
    // Class interaction
    // ------------------------------
    
    public void ParseStream(TextReader source, ObjectsDB db)
    {
        var sqlParser = new SqlParser(db);
        
        for (string? line; (line = source.ReadLine()) != null;)
        {
            line = line.Trim();

            if (line == "exit")
                break;
            
            if (line == "print")
                _processSnapshot(db);
            else if (line == "report")
                _processReport(db);
            else
                sqlParser.Parse(line);
        }
    }
    
    // ------------------------------
    // Private class methods
    // ------------------------------

    private void _processReport(ObjectsDB db)
    {
        var reportables = new List<IReportable>(db.GetAirports());
        reportables.AddRange(db.GetCargoPlanes());
        reportables.AddRange(db.GetPassengerPlanes());

        List<Reporter> reporters = [ 
            new Television("Telewizja Abelowa"), new Television("Kanał TV-Tensor"),
            new Radio("Radio Kwantyfikator"), new Radio("Radio Shmem"),
            new Newspaper("Gazeta Kategoryczna"), new Newspaper("Dziennik Politechniczny")
        ];
        
        NewsGenerator ng = new(reportables, reporters);
        for (string? msg = ng.GenerateNextNews(); msg != null; msg = ng.GenerateNextNews())
            Console.WriteLine($"Generated another news: {msg}");
    }
    
    private void _processSnapshot(ObjectsDB db)
    {
        var filename = $"snapshot_{DateTime.Now:hh_mm_ss}.json";
        db.SerializeDB(filename, new JsonSerializationMachine());
    }
}
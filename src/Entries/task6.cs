using proj.CLI;
using proj.InputParsing;
using proj.ObjectsSources;
using proj.Storage;

namespace proj.Entries;

public class Task6: EntryPoint
{
    public Task6(int timeSpeedup = 60, int startHour = 15)
    {
        _timeCoef = timeSpeedup;
        _startHour = startHour;

        _db = new();
        _oper = new(_db);
    }
    public override void Run(string[] args)
    {
        // entry sanity check
        if (args.Length != 2)
        {
            Console.Error.WriteLine("[ ERROR ] There should be exactly two arguments:\n" +
                                    "\t-path to desired .ftr formatted file - source of records\n" +
                                    "\t-path to .ftre formatted file - source of record updates\n");
            return;
        }
        
        
        // preparing record sources
        ObjectSource source = new FileSource(_db, args[0], new FtrScheme());
        
        // reading whole ftr file
        source.OpenSource();
        source.CloseSource();
        
        // consilidating db
        _db.Consolidate();
        
        var sim = new Thread(_runFlightSimulation);
        var gui = new Thread(_runGui);
        var cli = new Thread(_runCLI);
        
        sim.Start();
        // gui.Start();
        cli.Start();

        // var updateSource = new UpdateSource(_db, args[1], 100, 500);
        // updateSource.OpenSource();
        
        // gui.Join();

        _isSimulationRunning = false;
        sim.Join();
        cli.Join();
    }

    private static void _runGui()
        => FlightTrackerGUI.Runner.Run();

    private void _runCLI()
    {
        // parsing stream
        SimpleCLIParser parser = new();
        parser.ParseStream(Console.In, _db);
    }

    private void _runFlightSimulation()
    {
        
        // preparing start date
        var today = DateTime.Now;
        var startDate = new DateTime(today.Year, today.Month, today.Day, _startHour, 0, 0);
        // stardate offset
        long passedSeconds = 0;
        
        while (_isSimulationRunning)
        {
            // performing update on objects
            var tStart = DateTime.Now;
            _db.ApplyUpdates();
            _oper.UpdatePositions(startDate.AddSeconds(passedSeconds));
            var tStop = DateTime.Now;

            // sleeping with exception on updating spent time
            var msDiff = (tStop - tStart).Milliseconds;
            if (msDiff < 1000)
                Thread.Sleep(1000 - msDiff);

            passedSeconds += _timeCoef;
        }
    }

    private readonly long _timeCoef;
    private readonly int _startHour;
    private bool _isSimulationRunning = true;
    private ObjectsDB _db;
    private GUIOperator _oper;
}
using proj.CLI;
using proj.ObjectsSources;
using proj.Storage;

namespace proj.Entries;

public class Task3: EntryPoint
{
    public Task3(int timeSpeedup = 60, int startHour = 15)
    {
        _timeCoef = timeSpeedup;
        _startHour = startHour;

        _db = new();
        _oper = new(_db);
    }
    public override void Run(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine($"[ ERROR ] There should be exactly one argument: path to desired .ftr formatted file");
            return;
        }

        var sim = new Thread(_runFlightSimulation);
        var gui = new Thread(_runGui);
        var cli = new Thread(()=>_runCLI(args[0]));
        
        sim.Start();
        gui.Start();
        cli.Start();

        gui.Join();

        _isSimulationRunning = false;
        sim.Join();
        cli.Join();
    }

    private static void _runGui()
        => FlightTrackerGUI.Runner.Run();

    private void _runCLI(string filename)
    {
        ObjectSource source = new TCPSimulatorSource(_db, filename, 1, 1);
        SimpleCLIParser parser = new();
        
        source.OpenSource();
        parser.ParseStream(Console.In, _db);
        source.CloseSource();
    }

    private void _runFlightSimulation()
    {
        
        var today = DateTime.Now;
        var startDate = new DateTime(today.Year, today.Month, today.Day, _startHour, 0, 0);
        long passedSeconds = 0;
        
        while (_isSimulationRunning)
        {
            var tStart = DateTime.Now;
            _db.Consolidate();
            _oper.UpdatePositions(startDate.AddSeconds(passedSeconds));
            var tStop = DateTime.Now;

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
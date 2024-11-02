using proj.InputParsing;
using proj.Storage;

namespace proj.ObjectsSources;

public class TCPSimulatorSource:ObjectSource
{
    // ------------------------------
    // Class creation
    // ------------------------------

    public TCPSimulatorSource(ObjectsDB db, string filepath, int minOff, int maxOff) : base(db)
       => _simulator = new NetworkSourceSimulator.NetworkSourceSimulator(filepath, minOff, maxOff);

    // ------------------------------
    // Private class methods
    // ------------------------------

    protected override void _workerSourceJob()
    {
        _simulator.OnNewDataReady += (sender, args) =>
            AddObjectToProductionLine(new BinaryPackedFlight(_simulator.GetMessageAt(args.MessageIndex).MessageBytes));
        
        _simulator.Run();
    }

    public override void CloseSource()
    { }

    // ------------------------------
    // Class fields
    // ------------------------------
    
    private NetworkSourceSimulator.NetworkSourceSimulator _simulator;
}
using proj.Storage;

namespace proj.ObjectsSources;

public class UpdateSource: ObjectSource
{
    // ------------------------------
    // Class creation
    // ------------------------------

    public UpdateSource(ObjectsDB db, string filepath, int minOff, int maxOff) : base(db)
        => _simulator = new NetworkSourceSimulator.NetworkSourceSimulator(filepath, minOff, maxOff);
    
    
    // ------------------------------
    // Class interaction
    // ------------------------------
    
    public override void CloseSource()
    { }

    // ------------------------------
    // Private class methods
    // ------------------------------

    protected override void _workerSourceJob()
    {
        _simulator.OnContactInfoUpdate += (sender, args) => _db.PushUpdateToWaitList(new ContactsUpdate(args));
        _simulator.OnPositionUpdate += (sender, args) => _db.PushUpdateToWaitList(new LocationUpdate(args));
        _simulator.OnIDUpdate += (sender, args) => _db.PushUpdateToWaitList(new IDUpdate(args));
        
        _simulator.Run();
    }

    // ------------------------------
    // Class fields
    // ------------------------------
    
    private NetworkSourceSimulator.NetworkSourceSimulator _simulator;
}
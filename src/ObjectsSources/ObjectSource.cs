using proj.Storage;

namespace proj.ObjectsSources;

/*      Base class that should be used to implement any sources object receiving
 *  and creation inside the application.
 */

public abstract class ObjectSource
{
    // ------------------------------
    // Class creation
    // ------------------------------

    protected ObjectSource(ObjectsDB db)
    {
        _db = db;
        _workerSource = new Thread(_workerSourceJob);
        _workerSource.IsBackground = true;
    }
    
    // ------------------------------
    // abstract methods
    // ------------------------------

    // If possible this function should correctly close working threads
    public abstract void CloseSource();

    // This methods implements logic of some source and is run in external thread
    protected abstract void _workerSourceJob();

    // ------------------------------
    // Class interaction
    // ------------------------------

    protected void AddObjectToProductionLine(IStorage.SourcePacket packet)
        => _db.PushItemToWaitList(packet);

    public void OpenSource()
    {
        _workerSource.Start();
    }
    
    // ------------------------------
    // Class fields
    // ------------------------------

    protected ObjectsDB _db;
    protected Thread _workerSource;
}
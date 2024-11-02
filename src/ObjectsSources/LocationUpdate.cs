using System.Text.Json;
using NetworkSourceSimulator;
using proj.InnerObjects;
using proj.Storage;

namespace proj.ObjectsSources;

public class LocationUpdate(PositionUpdateArgs args) : IStorage.Update
{
    // ------------------------------
    // Class interaction
    // ------------------------------
    
    public override bool Perform(IStorage storage)
    {
        bool isSuccess = false;
        
        if (storage.Get(UpdateArgs.ObjectID, out Flight flight))
        {
            PrevState = flight.UpdatePosition(UpdateArgs);
            _dbGetter = (stor, id) => stor.Get(id, out Flight f) ? f : null;
            isSuccess = true;
        }
        else if (storage.Get(UpdateArgs.ObjectID, out Airport airport))
        {
            PrevState = airport.UpdatePosition(UpdateArgs);
            _dbGetter = (stor, id) => stor.Get(id, out Airport a) ? a : null;
            isSuccess = true;
        }
            
        Log(isSuccess);
        return isSuccess;
    }

    public override bool Reverse(IStorage storage)
    {
        if (PrevState == null) return false;
        Traceable? prevObj = _dbGetter?.Invoke(storage, PrevState.ObjectID);
        prevObj?.UpdatePosition(PrevState);

        return prevObj != null;
    }

    public override string Serialize() => JsonSerializer.Serialize(this);
    
    // ------------------------------
    // Class fields
    // ------------------------------

    public PositionUpdateArgs UpdateArgs { get; } = args;
    public PositionUpdateArgs? PrevState { get; private set; }

    private Func<IStorage, UInt64, Traceable?>? _dbGetter;
}
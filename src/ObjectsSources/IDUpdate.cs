using System.Text.Json;
using NetworkSourceSimulator;
using proj.InnerObjects;
using proj.Storage;

namespace proj.ObjectsSources;

public class IDUpdate(IDUpdateArgs args) : IStorage.Update
{
    // ------------------------------
    // Class interaction
    // ------------------------------   
    
    public override bool Perform(IStorage storage)
    {
        bool isSuccess = false;

        if (storage.Get(UpdateArgs.ObjectID, out FlightsSystemObject fo,
                out Func<IStorage, UInt64, FlightsSystemObject?> getter,
                out Func<IStorage, UInt64, bool> remover))
        {
            PrevState = fo.UpdateID(UpdateArgs);
            if (fo.ProcessVisitor(storage))
            {
                remover(storage, UpdateArgs.ObjectID);
                _dbGetter = getter;
                _remover = remover;
                isSuccess = true;
            }
            else
            {
                fo.UpdateID(PrevState);
                PrevState = null;
            }
        }
        
        Log(isSuccess);
        return isSuccess;
    }

    public override bool Reverse(IStorage storage)
    {
        if (PrevState == null) return false;
        FlightsSystemObject? prevObj = _dbGetter?.Invoke(storage, PrevState.ObjectID);
        prevObj?.UpdateID(PrevState);

        return prevObj != null;
    }
    
    public override string Serialize() => JsonSerializer.Serialize(this);
    
    // ------------------------------
    // Class fields
    // ------------------------------

    public IDUpdateArgs UpdateArgs { get; } = args;
    public IDUpdateArgs? PrevState { get; private set; }

    private Func<IStorage, UInt64, FlightsSystemObject?>? _dbGetter;
    private Func<IStorage, UInt64, bool>? _remover;
}
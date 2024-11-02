using System.Text.Json;
using NetworkSourceSimulator;
using proj.InnerObjects;
using proj.Storage;

namespace proj.ObjectsSources;

[Serializable]
public class ContactsUpdate(ContactInfoUpdateArgs args) : IStorage.Update
{
    // ------------------------------
    // Class interaction
    // ------------------------------
    
    public override bool Perform(IStorage storage)
    {
        bool isSuccess = false;
        
        if (storage.Get(UpdateArgs.ObjectID, out Crew crew))
        {
            PrevState = crew.UpdateContact(UpdateArgs);
            _dbGetter = (stor, id) => stor.Get(id, out Crew c) ? c : null;
            isSuccess = true;
        }
        else if (storage.Get(UpdateArgs.ObjectID, out Passenger pass))
        {
            PrevState = pass.UpdateContact(UpdateArgs);
            _dbGetter = (stor, id) => stor.Get(id, out Passenger p) ? p : null;
            isSuccess = true;
        }
            
        Log(isSuccess);
        return isSuccess;
    }

    public override bool Reverse(IStorage storage)
    {
        if (PrevState == null) return false;
        Human? prevObj = _dbGetter?.Invoke(storage, PrevState.ObjectID);
        prevObj?.UpdateContact(PrevState);

        return prevObj != null;
    }

    public override string Serialize() => JsonSerializer.Serialize(this);
    
    // ------------------------------
    // Class fields
    // ------------------------------

    public ContactInfoUpdateArgs UpdateArgs { get; private init; } = args;
    public ContactInfoUpdateArgs? PrevState { get; private set; }

    private Func<IStorage, UInt64, Human?>? _dbGetter;
}
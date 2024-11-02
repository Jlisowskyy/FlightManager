namespace proj.Serialization;

public abstract class SerializationMachine
{
    
    // Returns whether operation succeeded or not
    public abstract bool Serialize<T>(T serializedType, string filename);
    
    // Returns deserialized object. In case operation failed returns null.
    public abstract T? DeserializeCollection<T>(string filename);
}
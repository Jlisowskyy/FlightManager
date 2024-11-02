using System.Text.Json;
using System.Text.Json.Serialization;

namespace proj.Serialization;

public class JsonSerializationMachine: SerializationMachine
{
    // ------------------------------
    // Class interaction
    // ------------------------------

    public override T? DeserializeCollection<T>(string filename) where T : default
    {
        throw new NotImplementedException();
    }

    public override bool Serialize<T>(T serializedType, string filename)
    {
        try
        {
            _serialize(serializedType, filename);
            return true;
        }
        catch (Exception exc)
        {
            Console.WriteLine($"[ ERROR ] During serialization error occured:\n\t{exc.Message}!\n");
            return false;
        }
    }
    
    // ------------------------------
    // Private methods
    // ------------------------------

    private static void _serialize<T>(T serializedType, string filename)
    {
        using var stream = new StreamWriter(filename);
        var opt = new JsonSerializerOptions(JsonSerializerOptions.Default);
        opt.WriteIndented = true;
        opt.ReferenceHandler = ReferenceHandler.Preserve;
        
        stream.Write(JsonSerializer.Serialize(serializedType, opt));
    }
}
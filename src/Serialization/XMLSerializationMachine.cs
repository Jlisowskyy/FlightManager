using System.Xml.Serialization;
using proj.InnerObjects;

namespace proj.Serialization;

public class XmlSerializationMachine: SerializationMachine
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
        using var stream = new FileStream(filename, FileMode.Create);
        var serializer = new XmlSerializer(typeof(List<FlightsSystemObject>));
        serializer.Serialize(stream, serializedType);
    }
}
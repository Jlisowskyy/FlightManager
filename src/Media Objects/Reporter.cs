using proj.InnerObjects;

namespace proj.Media_Objects;

public abstract class Reporter(string name)
{
// ------------------------------
// Class interaction
// ------------------------------

    public abstract string Report(Airport airport);
    public abstract string Report(CargoPlane cargoPlane);
    public abstract string Report(PassengerPlane passengerPlane);

// ------------------------------
// Class fields
// ------------------------------

    public string Name { get; set; } = name;
}
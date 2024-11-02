using proj.InnerObjects;

namespace proj.Media_Objects;

public class Television(string name) : Reporter(name)
{
// ------------------------------
// Class interaction
// ------------------------------
    public override string Report(Airport airport)
        => $"<An image of {airport.Name} airport>";

    public override string Report(CargoPlane cargoPlane)
        => $"<An image of {cargoPlane.Name} cargo plane>";

    public override string Report(PassengerPlane passengerPlane)
        => $"<An image of {passengerPlane.Name} passenger plane>";
}
using proj.InnerObjects;

namespace proj.Media_Objects;

public class Radio(string name) : Reporter(name)
{
// ------------------------------
// Class interaction
// ------------------------------

    public override string Report(Airport airport)
        => $"<Reporting for {Name}, Ladies and gentelmen, we are at the {airport.Name} airport.";

    public override string Report(CargoPlane cargoPlane)
        => $"Reporting for {Name}, Ladies and gentelmen, we are seeing the {cargoPlane.Serial} aircraft fly above us.";

    public override string Report(PassengerPlane passengerPlane)
        => $"Reporting for {Name}, Ladies and gentelmen, we’ve just witnessed {passengerPlane.Serial} take off.";
}

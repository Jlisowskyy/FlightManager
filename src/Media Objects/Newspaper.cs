using proj.InnerObjects;

namespace proj.Media_Objects;

public class Newspaper(string name) : Reporter(name)
{
// ------------------------------
// Class interaction
// ------------------------------

    public override string Report(Airport airport)
        => $"{Name} - A report from the {airport.Name} airport, {airport.Country}.";

    public override string Report(CargoPlane cargoPlane)
        => $"{Name} - An interview with the crew of {cargoPlane.Serial}.";

    public override string Report(PassengerPlane passengerPlane)
        => $"{Name} -Breaking news! {passengerPlane.Model} aircraft loses EASA f" +
           $"ails certification after inspection of {passengerPlane.Serial}.";
}
using proj.Interfaces;

namespace proj.InnerObjects;

public class CargoFlight: Flight
{
    // ----------------------------------------
    // FlightsSystemObject implementation
    // ----------------------------------------
    
    public override bool ProcessVisitor(IVisitor visitor)
        => visitor.Visit(this);

    // ---------------------------------
    // Class fields and properties
    // ---------------------------------

    public CargoPlane Plane { get; set; }
    public Cargo[] Load { get; set; }
}
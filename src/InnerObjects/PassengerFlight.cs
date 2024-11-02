using proj.Interfaces;

namespace proj.InnerObjects;

public class PassengerFlight: Flight
{
    // ----------------------------------------
    // FlightsSystemObject implementation
    // ----------------------------------------
    
    public override bool ProcessVisitor(IVisitor visitor)
        => visitor.Visit(this);

    // ---------------------------------
    // Class fields and properties
    // ---------------------------------

    public PassengerPlane Plane { get; set; }
    public Passenger[] Load { get; set; }
}
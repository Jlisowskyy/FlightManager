using System.Text.Json.Serialization;
using System.Xml.Serialization;
using proj.Interfaces;

namespace proj.InnerObjects;

[Serializable]
[XmlInclude(typeof(Airport))]
[XmlInclude(typeof(Cargo))]
[XmlInclude(typeof(CargoPlane))]
[XmlInclude(typeof(Crew))]
[XmlInclude(typeof(Flight))]
[XmlInclude(typeof(Passenger))]
[XmlInclude(typeof(PassengerPlane))]
[XmlInclude(typeof(PassengerFlight))]
[XmlInclude(typeof(CargoFlight))]
[JsonDerivedType(typeof(Airport), "Airport")]
[JsonDerivedType(typeof(Cargo), "Cargo")]
[JsonDerivedType(typeof(CargoPlane), "CargoPlane")]
[JsonDerivedType(typeof(Crew), "Crew")]
[JsonDerivedType(typeof(Flight), "Flight")]
[JsonDerivedType(typeof(Passenger), "Passenger")]
[JsonDerivedType(typeof(PassengerPlane), "PassengerPlane")]
[JsonDerivedType(typeof(PassengerFlight), "PassengerFlight")]
[JsonDerivedType(typeof(CargoFlight), "CargoFlight")]

public abstract class FlightsSystemObject : IVisitable
{
    public UInt64 ID { get; set; }

    public virtual bool ProcessVisitor(IVisitor visitor)
        => false;
}
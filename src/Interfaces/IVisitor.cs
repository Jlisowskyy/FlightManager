using proj.InnerObjects;

namespace proj.Interfaces;

public interface IVisitor
{
    public bool Visit(Airport airport);
    public bool Visit(Cargo cargo);
    public bool Visit(CargoPlane cargoPlane);
    public bool Visit(Crew crew);
    public bool Visit(Flight flight);
    public bool Visit(Passenger passenger);
    public bool Visit(PassengerPlane passengerPlane);
}
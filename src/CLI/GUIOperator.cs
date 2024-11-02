using FlightTrackerGUI;
using Mapsui.Projections;
using proj.InnerObjects;
using proj.Storage;

namespace proj.CLI;

/*
 *      SHORT SUMMARRY
 *
 *  Class collects simple methods to convert inner application types into
 * gui-compatible ones. 
 * 
 */

public class GUIOperator
{

    private class Adapter : FlightsGUIData
    {
        // ------------------------------
        // Inner types
        // ------------------------------


        public Adapter(List<Flight> flights, List<double> rots)
        {
            _flights = flights;
            _rotations = rots;
        }
        
        public override WorldPosition GetPosition(int index) =>
            new(_flights[index].GeoPos.Latitude, _flights[index].GeoPos.Longitude);

        public override double GetRotation(int index)
            => _rotations[index];

        public override int GetFlightsCount()
            => _flights.Count();

        public override ulong GetID(int index)
            => _flights[index].ID;

        private List<Flight> _flights;
        private List<double> _rotations;
    }
    
    // ------------------------------
    // Class creation
    // ------------------------------

    public GUIOperator(ObjectsDB db)
    {
        _db = db;
        _db.OnNewFlightArrival += _addFlight;
        _gui = new Adapter(_flights, _rotations);
    }

    // ------------------------------
    // Class interaction
    // ------------------------------

    // Methods simply adds to the gui flights which are either started or ended and unstarted ones.
    // Also performs update of the objects stored inside the data base accordingly to passed 'time'
    public void UpdatePositions(DateTime time)
    {
        lock (_db)
        {
            for (int i = 0; i < _flights.Count; ++i)
            {
                var oldPos = _flights[i].GeoPos;
                _flights[i].UpdatePosition(time);
                var newPos = _flights[i].GeoPos;

                if (time > _flights[i].LandingTime)
                    _rotations[i] = 0.0;
                else
                    _rotations[i] = CalculateAngle(newPos, oldPos);
            }
        
            // picking flights currently in progress
            lock (_flightsWaitList)
            {
                var nStack = new Stack<Flight>();
                while (_flightsWaitList.Count != 0)
                {
                    var fl = _flightsWaitList.Pop();
                    if (fl.TakeOffTime > time)
                    {
                        nStack.Push(fl);
                        continue;
                    }
                    
                    fl.UpdatePosition(time);
                    _flights.Add(fl);
                
                    if (time > fl.LandingTime)
                        _rotations.Add(0.0);
                    else
                        _rotations.Add(CalculateAngle(fl.TargetAirport.GeoPos, fl.OriginAirport.GeoPos));
                }

                _flightsWaitList = nStack;
            }  
        }
        
        Runner.UpdateGUI(_gui);
    }

    // Simple pings gui to update gui
    public void LoadPositions()
        => Runner.UpdateGUI(_gui);
    
    // Method is used to calculate angle between two consecutivec positions on the plane path.
    public static double CalculateAngle(GeographicPosition newPos, GeographicPosition oldPos)
    {
        var (ox, oy) = SphericalMercator.FromLonLat(oldPos.Longitude, oldPos.Latitude);
        var (x, y) = SphericalMercator.FromLonLat(newPos.Longitude, newPos.Latitude);
        
        return double.Atan2(x-ox, y-oy);
    }

    // ------------------------------
    // Private class methods
    // ------------------------------

    void _addFlight(object? sender, Flight flight)
    {
        lock (_flightsWaitList)
            _flightsWaitList.Push(flight);
    }
    
    // ------------------------------
    // Class fields
    // ------------------------------

    private Stack<Flight> _flightsWaitList = new();
    private List<Flight> _flights = new();
    private List<double> _rotations = new();
    private Adapter _gui;
    private readonly ObjectsDB _db;
}
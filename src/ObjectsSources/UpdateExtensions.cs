using NetworkSourceSimulator;
using proj.InnerObjects;

namespace proj.ObjectsSources;

public static class UpdateExtensions
{
    public static IDUpdateArgs UpdateID(this FlightsSystemObject obj, IDUpdateArgs args)
    {
        IDUpdateArgs rv = new() { NewObjectID = obj.ID, ObjectID = args.NewObjectID };
        obj.ID = args.NewObjectID;
        return rv;
    }
    
    public static PositionUpdateArgs UpdatePosition(this Traceable obj, PositionUpdateArgs args)
    {
        PositionUpdateArgs rv = new()
        {
            Latitude = obj.GeoPos.Latitude,
            Longitude = obj.GeoPos.Longitude,
            AMSL = obj.GeoPos.AMSL,
            ObjectID = obj.ID
        };
        
        obj.GeoPos = new GeographicPosition(args.Latitude, args.Longitude, args.AMSL);
        return rv;
    }

    public static ContactInfoUpdateArgs UpdateContact(this Human obj, ContactInfoUpdateArgs args)
    {
        var rv = new ContactInfoUpdateArgs()
        {
            ObjectID = obj.ID,
            EmailAddress = obj.Email, 
            PhoneNumber = obj.Phone
        };
        
        obj.Email = args.EmailAddress;
        obj.Phone = args.PhoneNumber;

        return rv;
    }
}
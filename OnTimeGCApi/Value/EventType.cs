
namespace OnTimeGCApi
{
    public enum EventType
    {
        Appointment = 0,
        Anniversary = 1,
        AllDay = 2,
        Meeting = 3,
        Invatation = 4, // = "I"
        Cancelled = 5, // "C"
        Reschedule = 6, // = "U"
        Confirmed = 7, // = "N"
        InfoUpdate = 8, // = "E"
        Removed = 9, // = "S"
        Countered = 10, // = "T"
        Rejected = 11, // = "R"
    }
}
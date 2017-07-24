using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class AppointmentChange
    {
        public string UnID { get; set; }
        public string NewApptUnID { get; set; }
        public string NewLastMod { get; set; }
        public Dictionary<string, object> Legend { get; set; }
        public string Status { get; set; }
        public string StatusKey { get; set; }
        public string StatusText { get; set; }
        public string InfoKey { get; set; }
        public string InfoText { get; set; }
        public int SectionProcessTime { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class CalendarItem
    {
        public string UnID { get; set; }
        public int LastMod { get; set; }
        public EventType ApptType { get; set; }
        public DateTime StartDT { get; set; }
        public DateTime EndDT { get; set; }
        public string RepIns { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public string From { get; set; }
        public List<string> Categories { get; set; }
        public int BodyLength { get; set; }
        public string ApptUnID { get; set; }
        public bool Available { get; set; }
        public bool Private { get; set; }
        public bool Repeat { get; set; }
        public string LegendID { get; set; }
        public bool TimeOff { get; set; }
        public bool MyPersonal { get; set; }
        public bool NoEdit { get; set; }
        public string Chair { get; set; }
        public List<string> SendTo { get; set; }
        public List<string> CopyTo { get; set; }
        public List<string> BccTo { get; set; }
        public List<string> Rooms { get; set; }
        public List<string> Resources { get; set; }
        public int Seq { get; set; }
        public Dictionary<string, object> CustomFields { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Start: \"{0}\" | End: \"{1}\" | Type: \"{2}\" | UnId: \"{3}\" | Subject: \"{4}\" }}", this.StartDT, this.EndDT, this.ApptType, this.UnID, this.Subject);
        }
    }
}

using System;
using System.Collections.Generic;

namespace OnTimeGCApi.Calendars
{
    public class Item
    {
        private Dictionary<string, string> apptTypeMap;

        public Item()
        {
            this.apptTypeMap = new Dictionary<string,string>();
            this.apptTypeMap.Add("0", "Appointment");
            this.apptTypeMap.Add("2", "AllDay");
            this.apptTypeMap.Add("3", "Meeting");
            this.apptTypeMap.Add("I", "Invatation");
        }

        public string UnID { get; set; }
        public int LastMod { get; set; }
        // TODO: ApptType string -> int
        public string ApptType { get; set; }
        public string AppointmentType { get { return (this.apptTypeMap.ContainsKey(this.ApptType) ? this.apptTypeMap[this.ApptType] : "NotImplemented");} }
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

        public override string ToString()
        {
            return string.Format("{{ Start: \"{0}\" | End: \"{1}\" | Type: \"{2}\" ({3}) | UnId: \"{4}\" | Subject: \"{5}\" }}", this.StartDT, this.EndDT, this.ApptType, this.AppointmentType, this.UnID, this.Subject);
        }
    }
}

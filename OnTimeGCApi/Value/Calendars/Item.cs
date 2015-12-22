using System;

namespace OnTimeGCApi.Calendars
{
    public class Item
    {
        public string UnID { get; set; }
        // TODO: LastMod int -> DateTime
        public int LastMod { get; set; }
        // TODO: ApptType string -> int
        public string ApptType { get; set; }
        public string StartDT { get; set; }
        public string EndDT { get; set; }
        public string ApptUnID { get; set; }
        // TODO: LegendID string -> int
        public string LegendID { get; set; }
        public bool TimeOff { get; set; }
        public bool Available { get; set; }
        public string From { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Start: \"{0}\" | End: \"{1}\" | Type: \"{2}\" | UnId: \"{3}\" }}", this.StartDT, this.EndDT, this.ApptType, this.UnID);
        }
    }
}

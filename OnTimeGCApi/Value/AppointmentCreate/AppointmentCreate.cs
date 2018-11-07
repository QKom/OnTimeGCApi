using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class AppointmentCreate
    {
        public string NewUnID { get; set; }
        public string NewApptUnID { get; set; }
        public int NewLastMod { get; set; }
        public Dictionary<string, object> Legend { get; set; }
        public string Status { get; set; }
        public string StatusKey { get; set; }
        public string StatusText { get; set; }
        public int SectionProcessTime { get; set; }

        public override string ToString()
        {
            return $"{{ Status: \"{this.Status}\" | StatusKey: \"{this.StatusKey}\" | StatusText: \"{this.StatusText}\" | NewUnID: \"{this.NewUnID}\" }}";
        }
    }
}

namespace OnTimeGCApi
{
    public class Profile
    {
        // TODO: CallEntryType string -> ?
        public string CalEntryType { get; set; }
        public int DefaultDuration { get; set; }
        public bool ExcludeFromAll { get; set; }
        public bool ExcludeFromSent { get; set; }
        public bool EnableAlarms { get; set; }
        public bool SetAlarmAppointment { get; set; }
        // TODO: AppointmentLead int -> ?
        public int AppointmentLead { get; set; }
        public bool SetAlarmEvent { get; set; }
        // TODO: EventLead int -> ?
        public int EventLead { get; set; }
        public bool DeclineKeepInformed { get; set; }
    }
}


namespace OnTimeGCApi
{
    public class MailContactsChange
    {
        public Contact Contact { get; set; }
        public int SectionProcessTime { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public string ErrorCode { get; set; }
    }
}
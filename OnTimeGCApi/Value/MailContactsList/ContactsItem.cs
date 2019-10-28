using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class ContactsItem
    {
        public List<Contact> Contacts { get; set; }
        public int SectionProcessTime { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public string ErrorCode { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class User
    {
        public string ID { get; set; }
        public DateTime LastMod { get; set; }
        public string PreSort { get; set; }
        public string DispName { get; set; }
        public UserType Type { get; set; }
        public string Access { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MailDomain { get; set; }
        public string MailSource { get; set; }
        public string MailServer { get; set; }
        public string MailFilepath { get; set; }
        public List<string> CustomFields { get; set; }
        public List<DateTime> WorkTimes { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ShortName { get; set; }
        public string AltFullName { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Company { get; set; }
        public string OfficePhone { get; set; }
        public string CellPhone { get; set; }
        public List<string> UserCategories { get; set; }
        public int DefaultDuration { get; set; }
        public string iNotesUrl { get; set; }
        public Profile Profile { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Id: \"{0}\" | Name: \"{1}\" | Email: \"{2}\" | Type: \"{3}\" }}", this.ID, this.Name, this.Email, this.Type);
        }
    }
}

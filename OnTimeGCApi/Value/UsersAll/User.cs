using System;
using System.Collections.Generic;

namespace OnTimeGCApi.UsersAll
{
    public class User
    {
        public string ID;
        public DateTime LastMod;
        public string PreSort;
        public string DispName;
        public UserType Type;
        public string Access;
        public string Name;
        public string Email;
        public string MailDomain;
        public string MailSource;
        public string MailServer;
        public string MailFilepath;
        public List<DateTime> WorkTimes;
        public string Firstname;
        public string Lastname;
        public string ShortName;
        public string AltFullName;
        public string Title;
        public string Location;
        public string Department;
        public string Company;
        public string OfficePhone;
        public string CellPhone;
        public List<string> UserCategories;
        public int DefaultDuration;
        public string iNotesUrl;
    }
}

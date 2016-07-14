using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class Contact
    {
        public string UnID { get; set; }
        public string LastMod { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Department { get; set; }
        public string CellPhoneNumber { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string JobTitle { get; set; }
        public string Location { get; set; }
        public string Manager { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeCountry { get; set; }
        public string OfficeFaxPhoneNumber { get; set; }
        public string OfficePhoneNumber { get; set; }
        public string OfficeState { get; set; }
        public string OfficeStreetAddress { get; set; }
        public string OfficeZip { get; set; }
        public string State { get; set; }
        public string StreetAddress { get; set; }
        public string Suffix { get; set; }
        public string Title { get; set; }
        public string Zip { get; set; }
        public List<string> Categories { get; set; }
        public Dictionary<string, object> Fields { get; set; }

        public override string ToString()
        {
            return string.Format("{{ LastName: \"{0}\" | FirstName: \"{1}\" | Email: \"{2}\" | UnId: \"{3}\" }}", this.LastName, this.FirstName, this.Email, this.UnID);
        }
    }
}
using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class UsersInfo
    {
        public List<User> IDs { get; set; }
        public List<User> Emails { get; set; }
        public List<User> DNs { get; set; }
        public List<User> ShortNames { get; set; }
        public List<User> Any { get; set; }
        public List<User> Groups { get; set; }
        public int SectionProcessTime { get; set; }
    }
}
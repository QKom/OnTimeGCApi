using System;

namespace OnTimeGCApi
{
    public class GroupListItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public DateTime LastMod { get; set; }
        public int UsersCount { get; set; }
        public bool IsPublic { get; set; }
        public bool IsShared { get; set; }
        public bool IsEditor { get; set; }
        public bool Sort { get; set; }
    }
}
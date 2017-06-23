using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class GroupUserIdsItem
    {
        public string ID { get; set; }
        public bool IsUser { get; set; }
        public List<string> IDs { get; set; }
        public int SectionProcessTime { get; set; }
    }
}

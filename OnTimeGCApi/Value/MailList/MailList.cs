using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class MailList
    {
        public List<MailListItem> Items { get; set; }
        public int SectionProcessTime { get; set; }

        public override string ToString()
        {
            return $"{{ Items: \"{this.Items?.Count}\" | SectionProcessTime: \"{this.SectionProcessTime}\" }}";
        }
    }
}
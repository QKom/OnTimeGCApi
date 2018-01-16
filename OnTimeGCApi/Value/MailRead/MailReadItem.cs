using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class MailReadItem
    {
        public string UnID { get; set; }
        public int LastMod { get; set; }
        public string BodyHtml { get; set; }
        public List<MailReadAttachment> Attachments { get; set; }
        public MailReadBodyMime BodyMime { get; set; }
        public Dictionary<string, string> Fields { get; set; }
        public Dictionary<string, MailReadExtraItem> Extraitems { get; set; }
    }
}
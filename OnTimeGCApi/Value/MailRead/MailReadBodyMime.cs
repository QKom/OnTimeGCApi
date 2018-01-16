using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class MailReadBodyMime
    {
        public string Type { get; set; }
        public string BoundaryStart { get; set; }
        public string BoundaryEnd { get; set; }
        public string Contenttype { get; set; }
        public string ContentDisposition { get; set; }
        public string Data { get; set; }
        public List<MailReadBodyMime> Childs { get; set; }
    }
}
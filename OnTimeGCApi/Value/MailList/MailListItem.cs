using System;
using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class MailListItem
    {
        public string UnID { get; set; }
        public int LastMod { get; set; }
        public int Size { get; set; }
        public DateTime Date { get; set; }
        public DateTime? PostedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public List<string> SendTo { get; set; }
        public List<string> CopyTo { get; set; }
        public string BodyAbb { get; set; }
        public List<MailListAttachment> Attachments { get; set; }
        public MailType Type { get { return (this.DeliveredDate == null ? MailType.Sent : MailType.Received); } }

        public override string ToString()
        {
            return $"{{ Type: \"{this.Type}\" | Subject: \"{this.Subject}\" | Date: \"{this.Date}\" | From: \"{this.From}\" | Attachments: \"{this.Attachments?.Count}\" }}";
        }
    }
}
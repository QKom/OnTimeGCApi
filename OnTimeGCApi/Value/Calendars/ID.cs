using System.Collections.Generic;

namespace OnTimeGCApi.Calendars
{
    public class Id
    {
        public string ID { get; set; }
        public List<Item> Items { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Id: \"{0}\" | Count: \"{1}\" }}", this.ID, this.Items.Count);
        }
    }
}

using System.Collections.Generic;

namespace OnTimeGCApi
{
    public class Id
    {
        public string ID { get; set; }
        public List<CalendarItem> Items { get; set; }

        public override string ToString()
        {
            return $"{{ Id: \"{this.ID}\" | Count: \"{this.Items.Count}\" }}";
        }
    }
}
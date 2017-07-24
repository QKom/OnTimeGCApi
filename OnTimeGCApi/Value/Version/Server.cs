using System;

namespace OnTimeGCApi
{
    public class Server
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return $"{{ Name: \"{this.Name}\" | Version: \"{this.Version}\" | Patform: \"{this.Platform}\" | Time: \"{this.Time}\" }}";
        }
    }
}
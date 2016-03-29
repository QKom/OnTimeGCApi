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
            return string.Format("{{ Name: \"{0}\" | Version: \"{1}\" | Patform: \"{2}\" | Time: \"{3}\" }}", this.Name, this.Version, this.Platform, this.Time);
        }
    }
}

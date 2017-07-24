
namespace OnTimeGCApi
{
    public class VersionItem
    {
        public string Version { get; set; }
        public string BuildTime { get; set; }
        public string UserName { get; set; }
        public Server Server { get; set; }
        public DatabaseInfo ConfigDatabase { get; set; }
        public DatabaseInfo ApiDatabase { get; set; }
        public DatabaseInfo ApiLog { get; set; }
        public int SectionProcessTime { get; set; }

        public override string ToString()
        {
            return $"{{ Version: {this.Version} | ServerName: {this.Server?.Name} }}";
        }
    }
}
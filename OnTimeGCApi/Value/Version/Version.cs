
namespace OnTimeGCApi
{
    public class Version
    {
        public string APIVersion { get; set; }
        public string UserName { get; set; }
        public Server Server { get; set; }
        public DatabaseInfo ConfigDatabase { get; set; }
        public DatabaseInfo ApiDatabase { get; set; }
        public DatabaseInfo ApiLog { get; set; }
        public int SectionProcessTime { get; set; }
    }
}

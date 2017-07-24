
namespace OnTimeGCApi
{
    public class DatabaseInfo
    {
        public string Server { get; set; }
        public string Filepath { get; set; }
        public string ReplicaID { get; set; }
        public int Size { get; set; }

        public override string ToString()
        {
            return $"{{ Server: \"{this.Server}\" | Filepath: \"{this.Filepath}\" | Size: \"{this.Size}\" }}";
        }
    }
}
using System.IO;
using System.Web.Script.Serialization;

namespace OnTimeGCApi.Test
{
    public class Settings
    {
        public string ApplicationId { get; set; }
        public string ApplicationVersion { get; set; }
        public int ApiVersion { get; set; }
        public string Domain { get; set; }
        public string ApiPath { get; set; }
        public string ServletPath { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string LoginUser { get; set; }
        public string LoginPass { get; set; }
        public string GenerateTokenOnBehalfOf { get; set; }
        public string APIToken { get; set; }

        public static Settings Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            string data = File.ReadAllText(filePath);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<Settings>(data);
        }
    }
}

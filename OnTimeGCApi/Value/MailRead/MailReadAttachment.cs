
namespace OnTimeGCApi
{
    public class MailReadAttachment
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Base64 { get; set; }

        public override string ToString()
        {
            return $"{{ Name: \"{this.Name}\" | Source: \"{this.Source}\" }}";
        }
    }
}
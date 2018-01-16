
namespace OnTimeGCApi
{
    public class MailListAttachment
    {
        public string Name { get; set; }
        public int FileSize { get; set; }
        public string Source { get; set; }

        public override string ToString()
        {
            return $"{{ Name: \"{this.Name}\" | Size: \"{this.FileSize}\" | Source: \"{this.Source}\" }}";
        }
    }
}
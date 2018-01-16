
namespace OnTimeGCApi
{
    public class GetTokenItem
    {
        public string Token { get; set; }
        public string User { get; set; }
        public string ID { get; set; }
        public int CurrentTimeout { get; set; }
        public int SectionProcessTime { get; set; }

        public override string ToString()
        {
            return $"{{ User: \"{this.User}\" | Id: \"{this.ID}\" | Token: \"{this.Token}\" | CurrenTimeout: \"{this.CurrentTimeout}\" }}";
        }
    }
}
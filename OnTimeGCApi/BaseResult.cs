
namespace OnTimeGCApi
{
    public class BaseResult
    {
        public string Disclaimer { get; set; }
        public string Status { get; set; }
        public string Token { get; set; }
        public bool IsAnonymous { get; set; }
        public string ErrorCode { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Status: {0} | IsAnonymous: {1} | ErrorCode: {2} }}", this.Status, this.IsAnonymous, this.ErrorCode.ToNullString());
        }
    }
}

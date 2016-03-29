
namespace OnTimeGCApi
{
    public class BaseResult
    {
        public string APIVersion { get; set; }
        public string Disclaimer { get; set; }
        public string Status { get; set; }
        public string Token { get; set; }
        public bool IsAnonymous { get; set; }
        public string ErrorCode { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Status: {0} | ApiVersion: {1} | IsAnonymous: {2} | ErrorCode: {3} }}", this.Status, this.APIVersion, this.IsAnonymous, this.ErrorCode.ToNullString());
        }
    }
}

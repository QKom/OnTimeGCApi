
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
            return $"{{ Status: {this.Status} | IsAnonymous: {this.IsAnonymous} | ErrorCode: {this.ErrorCode} }}";
        }
    }
}
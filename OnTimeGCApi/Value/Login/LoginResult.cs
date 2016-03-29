
namespace OnTimeGCApi
{
    public class LoginResult : BaseResult
    {
        public Login Login { get; set; }
        public bool IsAuthorized { get { return (this.Status == "OK" ? true : false); } }
    }
}

using OnTimeGCApi;

namespace ExampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            OnTimeGCApiClient client = new OnTimeGCApiClient("ApiExplorer", "5", 5, "https://demo.ontimesuite.com", "/ontime/ontimegcclient.nsf/");
            OnTimeGCApi.Login.Base result = client.Login("hs", "demo");
            if (result.IsAuthorized)
            {
                OnTimeGCApi.Version.Base versionResult = client.Version();
                OnTimeGCApi.UsersAll.Base usersAllResult = client.UsersAll(null, null);
                OnTimeGCApi.Logout.Base logoutResult = client.Logout();
            }
        }
    }
}

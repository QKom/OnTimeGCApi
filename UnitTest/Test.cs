using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnTimeGCApi;
using System;

namespace UnitTest
{
    [TestClass]
    public class Test
    {
        public const string ApplicationId = "ApiExplorer";
        public const string ApplicationVersion = "5";
        public const int ApiVersion = 5;
        public const string Domain = "https://demo.ontimesuite.com";
        public const string ApiPath = "/ontime/ontimegcclient.nsf/";

        [TestMethod]
        public void LoginWithValidCredentials()
        {
            OnTimeGCApiClient client = new OnTimeGCApiClient(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            OnTimeGCApi.Login.Base result = client.Login("hs", "demo");

            Assert.AreEqual(true, result.IsAuthorized, "successful");
        }

        [TestMethod]
        public void LoginWithWrongCredentials()
        {
            OnTimeGCApiClient client = new OnTimeGCApiClient(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            try
            {
                client.Login("hs", "demo1");
            }
            catch (Exception ex)
            {
                StringAssert.Contains("Invalid credentials.", ex.Message);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void Version()
        {
            OnTimeGCApiClient client = new OnTimeGCApiClient(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            OnTimeGCApi.Login.Base result = client.Login("hs", "demo");
            if (result.IsAuthorized)
            {
                OnTimeGCApi.Version.Base versionResult = client.Version();
                Assert.AreEqual("4.4.2", versionResult.Version.APIVersion);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void UsersAll()
        {
            OnTimeGCApiClient client = new OnTimeGCApiClient(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            OnTimeGCApi.Login.Base result = client.Login("hs", "demo");
            if (result.IsAuthorized)
            {
                OnTimeGCApi.UsersAll.Base usersAllResult = client.UsersAll(null, null);
                Assert.AreEqual(10, usersAllResult.UsersAll.Users.Count);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void Logout()
        {
            OnTimeGCApiClient client = new OnTimeGCApiClient(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            OnTimeGCApi.Login.Base result = client.Login("hs", "demo");
            if (result.IsAuthorized)
            {
                OnTimeGCApi.Logout.Base logoutResult = client.Logout();
                Assert.AreEqual("Harold Spitz/OnTime", logoutResult.Logout.Name);
                return;
            }

            Assert.Fail("Login failed.");
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnTimeGCApi;
using System;
using System.Collections.Generic;

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
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            OnTimeGCApi.Login.Base result = client.Login("hs", "demo");

            Assert.AreEqual(true, result.IsAuthorized, "successful");
        }

        [TestMethod]
        public void LoginWithWrongCredentials()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
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
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
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
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
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
        public void UsersInfo()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            OnTimeGCApi.Login.Base result = client.Login("hs", "demo");
            if (result.IsAuthorized)
            {
                OnTimeGCApi.UsersInfo.Base usersInfoResult = client.UsersInfo(null, new List<string>() { "H" }, null, null, null, null, null, null);
                Assert.AreEqual(1, usersInfoResult.UsersInfo.IDs.Count);
                Assert.AreEqual("harold.spitz@ontime.com", usersInfoResult.UsersInfo.IDs[0].Email);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void Calendars()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            OnTimeGCApi.Login.Base result = client.Login("hs", "demo");
            if (result.IsAuthorized)
            {
                OnTimeGCApi.Calendars.Base calendarsResult = client.Calendars(new List<string>() { "H", "10" }, null, null, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
                Assert.AreEqual(2, calendarsResult.Calendars.IDs.Count);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void Logout()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
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

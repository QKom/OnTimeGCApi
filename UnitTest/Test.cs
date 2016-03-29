using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnTimeGCApi;
using System;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class Test
    {
        private const string ApplicationId = "ApiExplorer";
        private const string ApplicationVersion = "5";
        private const int ApiVersion = 5;
        private const string Domain = "https://demo.ontimesuite.com";
        private const string ApiPath = "/ontime/ontimegcclient.nsf/";

        [TestMethod]
        public void LoginWithValidCredentials()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login("ch", "demo");

            Assert.AreEqual(true, result.IsAuthorized, "successful");
        }

        [TestMethod]
        public void LoginWithWrongCredentials()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            try
            {
                client.Login("ch", "demo1");
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
            LoginResult result = client.Login("ch", "demo");
            if (result.IsAuthorized)
            {
                VersionResult versionResult = client.Version();
                Assert.AreEqual("Chris Holmes/OnTime", versionResult.Version.UserName);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void UsersAll()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login("ch", "demo");
            if (result.IsAuthorized)
            {
                UsersAllResult usersAllResult = client.UsersAll();
                Assert.AreNotEqual(null, usersAllResult.UsersAll.Users);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void UsersInfo()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login("ch", "demo");
            if (result.IsAuthorized)
            {
                UsersInfoResult usersInfoResult = client.UsersInfo(onTimeIds: new List<string>() { "U" });
                Assert.AreEqual(1, usersInfoResult.UsersInfo.IDs.Count);
                Assert.AreEqual("chris.holmes@ontime.com", usersInfoResult.UsersInfo.IDs[0].Email);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void Calendars()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login("ch", "demo");
            if (result.IsAuthorized)
            {
                CalendarsResult calendarsResult = client.Calendars(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), onTimeIds: new List<string>() { "U", "10" });
                Assert.AreEqual(2, calendarsResult.Calendars.IDs.Count);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void Logout()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login("ch", "demo");
            if (result.IsAuthorized)
            {
                LogoutResult logoutResult = client.Logout();
                Assert.AreEqual("Chris Holmes/OnTime", logoutResult.Logout.Name);
                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void AppointmentCreateChangeDelete()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login("ch", "demo");
            if (result.IsAuthorized)
            {
                DateTime baseValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 12, 0, 0, DateTimeKind.Utc);
                AppointmentCreateResult appointmentCreateResult = client.AppointmentCreate(EventType.Appointment, "U", baseValue, baseValue.AddMinutes(30), "TestSubject1");
                Assert.AreEqual("OK", appointmentCreateResult.AppointmentCreate.Status);

                AppointmentChangeResult appointmentChangeResult = client.AppointmentChange("U", appointmentCreateResult.AppointmentCreate.NewUnID, baseValue, baseValue.AddHours(1), subject: "TestSubject2");
                Assert.AreEqual("OK", appointmentChangeResult.AppointmentChange.Status);

                AppointmentRemoveResult appointmentRemoveResult = client.AppointmentRemove("U", appointmentCreateResult.AppointmentCreate.NewUnID);
                Assert.AreEqual("OK", appointmentRemoveResult.AppointmentRemove.Status);

                return;
            }

            Assert.Fail("Login failed.");
        }

    }
}

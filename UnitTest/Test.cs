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
        private const int ApiVersion = 7;
        private const string Domain = "https://demo.ontimesuite.com";
        private const string ApiPath = "/ontime/ontimegcclient.nsf";
        private const string ServletPath = "/servlet/ontimegc";
        
        private const string UserId = "U";
        private const string Username = "Chris Holmes/OnTime";
        private const string EmailAddress = "chris.holmes@ontime.com";
        private const string LoginUser = "ch";
        private const string LoginPass = "demo";

        [TestMethod]
        public void LoginWithValidCredentials()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login(LoginUser, LoginPass);

            Assert.AreEqual(true, result.IsAuthorized);
        }

        [TestMethod]
        public void LoginTokenSuccess()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login(LoginUser, LoginPass);
            string token = result.Token;

            client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            result = client.Login(token);

            Assert.AreEqual(true, result.IsAuthorized);
        }

        [TestMethod]
        public void LoginTokenFail()
        {
            string token = "a1GPawEEHwfFutIm0tHMWKMlVyMd5NmWi7VzlKeR3bAWJoW9VEJQzXAxJ6BIDBy4T0HdGIvFu2GrRF56xPgO3a";

            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            try
            {
                LoginResult result = client.Login(token);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(true, ex.Message.StartsWith("Login failed"));
                return;
            }

            Assert.Fail("failed");
        }

        [TestMethod]
        public void LoginWithWrongCredentials()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            try
            {
                client.Login(LoginUser, "demo1");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(true, ex.Message.Contains("Invalid credentials."));
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void Version()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login(LoginUser, LoginPass);
            if (result.IsAuthorized)
            {
                VersionResult versionResult = client.Version();
                Assert.AreEqual(Username, versionResult.Version.UserName);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void UsersAll()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login(LoginUser, LoginPass);
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
            LoginResult result = client.Login(LoginUser, LoginPass);
            if (result.IsAuthorized)
            {
                UsersInfoResult usersInfoResult = client.UsersInfo(onTimeIds: new List<string>() { UserId });
                Assert.AreEqual(1, usersInfoResult.UsersInfo.IDs.Count);
                Assert.AreEqual(EmailAddress, usersInfoResult.UsersInfo.IDs[0].Email);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void Calendars()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login(LoginUser, LoginPass);
            if (result.IsAuthorized)
            {
                CalendarsResult calendarsResult = client.Calendars(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), onTimeIds: new List<string>() { UserId, "10" });
                Assert.AreEqual(2, calendarsResult.Calendars.IDs.Count);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void Logout()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login(LoginUser, LoginPass);
            if (result.IsAuthorized)
            {
                LogoutResult logoutResult = client.Logout();
                Assert.AreEqual(Username, logoutResult.Logout.Name);
                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void GroupList()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login(LoginUser, LoginPass);
            if (result.IsAuthorized)
            {
                GroupListResult groupListResult = client.GroupList(true, true, true);
                Assert.AreEqual(true, groupListResult.GroupList.Items.Count > 0);
                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void GroupUserIds()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login(LoginUser, LoginPass);
            if (result.IsAuthorized)
            {
                GroupListResult groupListResult = client.GroupList(true, true, true);
                if (groupListResult.GroupList.Items.Count > 0)
                {
                    GroupUserIdsResult groupUserIdsResult = client.GroupUserIds(groupListResult.GroupList.Items[0].ID);

                    Assert.AreEqual(groupListResult.GroupList.Items[0].ID, groupUserIdsResult.GroupUserIDs.ID);
                    return;
                }
                Assert.Fail("Fetching GroupList");
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void AppointmentCreateChangeDelete()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath);
            LoginResult result = client.Login(LoginUser, LoginPass);
            if (result.IsAuthorized)
            {
                DateTime baseValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0, DateTimeKind.Utc);
                AppointmentCreateResult appointmentCreateResult = client.AppointmentCreate(EventType.Appointment, UserId, baseValue, baseValue.AddMinutes(30), "TestSubject1");
                Assert.AreEqual("OK", appointmentCreateResult.AppointmentCreate.Status);

                AppointmentChangeResult appointmentChangeResult = client.AppointmentChange(UserId, appointmentCreateResult.AppointmentCreate.NewUnID, baseValue, baseValue.AddHours(1), subject: "TestSubject2");
                Assert.AreEqual("OK", appointmentChangeResult.AppointmentChange.Status);

                AppointmentRemoveResult appointmentRemoveResult = client.AppointmentRemove(UserId, appointmentCreateResult.AppointmentCreate.NewUnID);
                Assert.AreEqual("OK", appointmentRemoveResult.AppointmentRemove.Status);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void MailContactsListResult()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath, ServletPath);
            LoginResult result = client.Login(LoginUser, LoginPass);
            if (result.IsAuthorized)
            {
                MailContactsListResult mailContactsListResult = client.MailContactsList(UserId);
                Assert.AreNotEqual(null, mailContactsListResult.MailContactsList.Contacts);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void MailContactCreateChangeReadDelete()
        {
            Client client = new Client(ApplicationId, ApplicationVersion, ApiVersion, Domain, ApiPath, ServletPath);
            LoginResult result = client.Login(LoginUser, LoginPass);
            if (result.IsAuthorized)
            {
                MailContactsCreateResult mailContactsCreateResult = client.MailContactCreate(UserId, "Herr Hans Test", "hans.test@foo.de", title: "Herr", additionalFields: new Dictionary<string, string>() { { "Foo", "Bar" } });
                Assert.AreEqual("OK", mailContactsCreateResult.Status);

                MailContactsChangeResult mailContactsChangeResult = client.MailContactsChange(UserId, mailContactsCreateResult.MailContactsCreate.Contact.UnID, additionalFields: new Dictionary<string, string>() { { "Foo", "FooBar" } });
                Assert.AreEqual("OK", mailContactsChangeResult.Status);

                MailContactsReadResult mailContactsReadResult = client.MailContactsRead(UserId, mailContactsCreateResult.MailContactsCreate.Contact.UnID, new List<string>() { "Foo" });
                Assert.AreEqual("hans.test@foo.de", mailContactsReadResult.MailContactsRead.Contact.Email);

                MailContactsRemoveResult mailContactsRemoveResult = client.MailContactsRemove(UserId, mailContactsCreateResult.MailContactsCreate.Contact.UnID);
                Assert.AreEqual("OK", mailContactsRemoveResult.Status);

                return;
            }

            Assert.Fail("Login failed.");
        }
    }
}

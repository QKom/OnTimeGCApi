﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace OnTimeGCApi.Test
{
    [TestClass()]
    public class Main
    {
        private static Settings Configuration;

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Configuration = Settings.Load("settings.json");
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                CalendarsResult calendarsResult = client.Calendars(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), new List<string>() { Configuration.UserId });
                foreach (CalendarItem item in calendarsResult.Calendars.IDs[0].Items)
                {
                    if (item.Subject.StartsWith("UnitTest123"))
                    {
                        AppointmentRemoveResult appointmentRemoveResult = client.AppointmentRemove(Configuration.UserId, item.UnID);
                    }
                }
            }
        }

        [TestMethod]
        public void LoginWithValidCredentials()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);

            Assert.AreEqual(true, result.IsAuthorized);
        }

        [TestMethod]
        public void LoginTokenSuccess()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            string token = result.Token;

            client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            result = client.Login(token);

            Assert.AreEqual(true, result.IsAuthorized);
        }

        [TestMethod]
        public void LoginTokenFail()
        {
            string token = "a1GPawEEHwfFutIm0tHMWKMlVyMd5NmWi7VzlKeR3bAWJoW9VEJQzXAxJ6BIDBy4T0HdGIvFu2GrRF56xPgO3a";

            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
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
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            try
            {
                client.Login(Configuration.LoginUser, "demo1");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(true, ex.Message.Contains("Invalid credentials"));
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void GenerateTokenOnBehalfOf()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                GetTokenResult getTokenResult = client.GenerateTokenOnBehalfOf(Configuration.GenerateTokenOnBehalfOf);
                Assert.AreEqual("OK", getTokenResult.Status);

                result = client.Login(getTokenResult.GetToken.Token);
                Assert.AreEqual("OK", result.Status);
                Assert.AreEqual(Configuration.GenerateTokenOnBehalfOf, result.Login.User.Email.ToLower());
                Assert.AreEqual(false, string.IsNullOrWhiteSpace(result.Token));

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void GenerateTokenOnBehalfOfAPIToken()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.APIToken);
            if (result.IsAuthorized)
            {
                GetTokenResult getTokenResult = client.GenerateTokenOnBehalfOf(Configuration.GenerateTokenOnBehalfOf);
                Assert.AreEqual("OK", getTokenResult.Status);

                result = client.Login(getTokenResult.GetToken.Token);
                Assert.AreEqual("OK", result.Status);
                Assert.AreEqual(Configuration.GenerateTokenOnBehalfOf, result.Login.User.Email.ToLower());
                Assert.AreEqual(false, string.IsNullOrWhiteSpace(result.Token));

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void Version()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                VersionResult versionResult = client.Version();
                Assert.AreEqual(Configuration.Username, versionResult.Version.UserName);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void UsersAll()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
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
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                UsersInfoResult usersInfoResult = client.UsersInfo(onTimeIds: new List<string>() { Configuration.UserId });
                Assert.AreEqual(1, usersInfoResult.UsersInfo.IDs.Count);
                Assert.AreEqual(Configuration.EmailAddress, usersInfoResult.UsersInfo.IDs[0].Email);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void Calendars()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                CalendarsResult calendarsResult = client.Calendars(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), onTimeIds: new List<string>() { Configuration.UserId });
                Assert.AreEqual(1, calendarsResult.Calendars.IDs.Count);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void Logout()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                LogoutResult logoutResult = client.Logout();
                Assert.AreEqual(Configuration.Username, logoutResult.Logout.Name);
                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void GroupList()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
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
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                GroupListResult groupListResult = client.GroupList(true, true, true);
                if (groupListResult.GroupList.Items.Count > 0)
                {
                    string groupId = groupListResult.GroupList.Items.Find(x => x.ID != null).ID;
                    GroupUserIdsResult groupUserIdsResult = client.GroupUserIds(groupId);

                    Assert.AreEqual(groupId, groupUserIdsResult.GroupUserIDs.ID);
                    return;
                }
                Assert.Fail("Fetching GroupList");
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void AppointmentCustomData()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                DateTime baseValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0, DateTimeKind.Utc);
                AppointmentCreateResult appointmentCreateResult = client.AppointmentCreate(EventType.Appointment, Configuration.UserId, baseValue, baseValue.AddMinutes(30), "UnitTest123", customFields: new Dictionary<string, object>() { { "foo", "bar" } });
                Assert.AreEqual("OK", appointmentCreateResult.AppointmentCreate.Status);
                Thread.Sleep(1000);

                bool found = false;
                CalendarsResult calendarsResult = client.Calendars(baseValue.AddDays(-1), baseValue.AddDays(1), new List<string>() { Configuration.UserId });
                foreach (CalendarItem item in calendarsResult.Calendars.IDs[0].Items)
                {
                    if (item.UnID == appointmentCreateResult.AppointmentCreate.NewUnID)
                    {
                        found = true;
                        Assert.AreEqual("UnitTest123", item.Subject);
                        Assert.AreEqual(EventType.Appointment, item.ApptType);
                        Assert.AreEqual(baseValue, item.StartDT.ToUniversalTime());
                        Assert.AreEqual("bar", item.CustomFields["foo"]);
                    }
                }
                Assert.AreEqual(true, found, "Test appointment not found");


                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void AppointmentCreateChangeDelete()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                DateTime baseValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0, DateTimeKind.Utc);
                AppointmentCreateResult appointmentCreateResult = client.AppointmentCreate(EventType.Appointment, Configuration.UserId, baseValue, baseValue.AddMinutes(30), "UnitTest123");
                Assert.AreEqual("OK", appointmentCreateResult.AppointmentCreate.Status);

                AppointmentChangeResult appointmentChangeResult = client.AppointmentChange(Configuration.UserId, appointmentCreateResult.AppointmentCreate.NewUnID, baseValue, baseValue.AddHours(1), subject: "UnitTest123");
                Assert.AreEqual("OK", appointmentChangeResult.AppointmentChange.Status);

                AppointmentRemoveResult appointmentRemoveResult = client.AppointmentRemove(Configuration.UserId, appointmentCreateResult.AppointmentCreate.NewUnID);
                Assert.AreEqual("OK", appointmentRemoveResult.AppointmentRemove.Status);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void AppointmentCreateAllDay()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                DateTime baseValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(-1);
                AppointmentCreateResult appointmentCreateResult = client.AppointmentCreate(EventType.AllDay, Configuration.UserId, baseValue, baseValue.AddDays(1), "UnitTest123");
                Assert.AreEqual("OK", appointmentCreateResult.AppointmentCreate.Status);
                Thread.Sleep(1000);

                bool found = false;
                CalendarsResult calendarsResult = client.Calendars(baseValue.AddDays(-1), baseValue.AddDays(1), new List<string>() { Configuration.UserId });
                foreach (CalendarItem item in calendarsResult.Calendars.IDs[0].Items)
                {
                    if (item.UnID == appointmentCreateResult.AppointmentCreate.NewUnID)
                    {
                        found = true;
                        Assert.AreEqual("UnitTest123", item.Subject);
                        Assert.AreEqual(EventType.AllDay, item.ApptType);
                        //Assert.AreEqual(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local), item.StartDT);
                        Assert.AreEqual(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(-1), item.StartDT.ToUniversalTime());
                    }
                }
                Assert.AreEqual(true, found, "AllDay event not found");

                AppointmentRemoveResult appointmentRemoveResult = client.AppointmentRemove(Configuration.UserId, appointmentCreateResult.AppointmentCreate.NewUnID);
                Assert.AreEqual("OK", appointmentRemoveResult.AppointmentRemove.Status);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void MailContactsListResult()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                MailContactsListResult mailContactsListResult = client.MailContactsList(Configuration.UserId);
                Assert.AreNotEqual(null, mailContactsListResult.MailContactsList.Contacts);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void MailContactCreateChangeReadDelete()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                MailContactsCreateResult mailContactsCreateResult = client.MailContactCreate(Configuration.UserId, "hans.test@foo.de", title: "Herr", firstName: "Hans", lastName: "Test", additionalFields: new Dictionary<string, string>() { { "Foo", "Bar" } });
                Assert.AreEqual("OK", mailContactsCreateResult.Status);

                MailContactsChangeResult mailContactsChangeResult = client.MailContactsChange(Configuration.UserId, mailContactsCreateResult.MailContactsCreate.Contact.UnID, additionalFields: new Dictionary<string, string>() { { "Foo", "FooBar" } });
                Assert.AreEqual("OK", mailContactsChangeResult.Status);

                MailContactsReadResult mailContactsReadResult = client.MailContactsRead(Configuration.UserId, mailContactsCreateResult.MailContactsCreate.Contact.UnID, new List<string>() { "Foo" });
                Assert.AreEqual("hans.test@foo.de", mailContactsReadResult.MailContactsRead.Contact.Email);

                MailContactsRemoveResult mailContactsRemoveResult = client.MailContactsRemove(Configuration.UserId, mailContactsCreateResult.MailContactsCreate.Contact.UnID);
                Assert.AreEqual("OK", mailContactsRemoveResult.Status);

                return;
            }

            Assert.Fail("Login failed.");
        }

        [TestMethod]
        public void CalendarsModified()
        {
            Client client = new Client(Configuration.ApplicationId, Configuration.ApplicationVersion, Configuration.ApiVersion, Configuration.Domain, Configuration.ApiPath, Configuration.ServletPath);
            LoginResult result = client.Login(Configuration.LoginUser, Configuration.LoginPass);
            if (result.IsAuthorized)
            {
                CalendarsModifiedResult calendarsModifiedResult = client.CalendarsModified();
                Assert.AreNotEqual(null, calendarsModifiedResult.CalendarsModified.ChangeKey);
                Assert.AreNotEqual(0, calendarsModifiedResult.CalendarsModified.IDs.Count);

                // change calendar
                DateTime baseValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0, DateTimeKind.Utc);
                AppointmentCreateResult appointmentCreateResult = client.AppointmentCreate(EventType.Appointment, Configuration.UserId, baseValue, baseValue.AddMinutes(30), "TestSubject1");
                Assert.AreEqual("OK", appointmentCreateResult.AppointmentCreate.Status);
                Thread.Sleep(2000);

                calendarsModifiedResult = client.CalendarsModified(calendarsModifiedResult.CalendarsModified.ChangeKey);
                Assert.AreNotEqual(null, calendarsModifiedResult.CalendarsModified.ChangeKey);
                Assert.AreNotEqual(0, calendarsModifiedResult.CalendarsModified.IDs.Count);
                Assert.AreEqual(true, calendarsModifiedResult.CalendarsModified.IDs.Contains(Configuration.UserId));

                // clean up calendar
                AppointmentRemoveResult appointmentRemoveResult = client.AppointmentRemove(Configuration.UserId, appointmentCreateResult.AppointmentCreate.NewUnID);
                Assert.AreEqual("OK", appointmentRemoveResult.AppointmentRemove.Status);

                return;
            }

            Assert.Fail("Login failed.");
        }
    }
}

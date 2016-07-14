using OnTimeGCApi;
using System;
using System.Collections.Generic;

namespace ExampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("ApiExplorer", "5", 5, "https://demo.ontimesuite.com", "/ontime/ontimegcclient.nsf", "/servlet/ontimegc");
            LoginResult result = client.Login("ch", "demo");
            string userId = "U";

            if (result.IsAuthorized)
            {
                VersionResult versionResult = client.Version();
                UsersAllResult usersAllResult = client.UsersAll();
                UsersInfoResult usersInfoResult = client.UsersInfo(onTimeIds: new List<string>() { userId });
                CalendarsResult calendarsResult = client.Calendars(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2), onTimeIds: new List<string>() { userId });

                DateTime baseValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 12, 0, 0, DateTimeKind.Utc);
                AppointmentCreateResult appointmentCreateResult = client.AppointmentCreate(EventType.Appointment, userId, baseValue, baseValue.AddMinutes(30), "TestSubject1");
                AppointmentChangeResult appointmentChangeResult = client.AppointmentChange(userId, appointmentCreateResult.AppointmentCreate.NewUnID, baseValue, baseValue.AddHours(1), subject: "TestSubject2");
                AppointmentRemoveResult appointmentRemoveResult = client.AppointmentRemove(userId, appointmentCreateResult.AppointmentCreate.NewUnID);


                MailContactsListResult mailContactsListResult = client.MailContactsList(userId, new List<string>() { "Foo" });
                MailContactsCreateResult mailContactsCreateResult = client.MailContactCreate(userId, "Herr Hans Test", "hans.test@example.com", title: "Herr", additionalFields: new Dictionary<string, string>() { { "Foo", "Bar" } });
                MailContactsChangeResult mailContactsChangeResult = client.MailContactsChange(userId, mailContactsCreateResult.MailContactsCreate.Contact.UnID, additionalFields: new Dictionary<string, string>() { { "Foo", "FooBar" } });
                MailContactsReadResult mailContactsReadResult = client.MailContactsRead(userId, mailContactsCreateResult.MailContactsCreate.Contact.UnID, new List<string>() { "Foo" });
                MailContactsRemoveResult mailContactsRemoveResult = client.MailContactsRemove(userId, mailContactsCreateResult.MailContactsCreate.Contact.UnID);

                LogoutResult logoutResult = client.Logout();
            }
        }
    }
}

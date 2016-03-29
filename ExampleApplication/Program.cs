using OnTimeGCApi;
using System;
using System.Collections.Generic;

namespace ExampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("ApiExplorer", "5", 5, "https://demo.ontimesuite.com", "/ontime/ontimegcclient.nsf/");
            LoginResult result = client.Login("ch", "demo");
            if (result.IsAuthorized)
            {
                VersionResult versionResult = client.Version();
                UsersAllResult usersAllResult = client.UsersAll();
                UsersInfoResult usersInfoResult = client.UsersInfo(onTimeIds: new List<string>() { "H" });
                CalendarsResult calendarsResult = client.Calendars(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2), onTimeIds: new List<string>() { "U", "10" });

                DateTime baseValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 12, 0, 0, DateTimeKind.Utc);
                AppointmentCreateResult appointmentCreateResult = client.AppointmentCreate(EventType.Appointment, "U", baseValue, baseValue.AddMinutes(30), "TestSubject1");
                AppointmentChangeResult appointmentChangeResult = client.AppointmentChange("U", appointmentCreateResult.AppointmentCreate.NewUnID, baseValue, baseValue.AddHours(1), subject: "TestSubject2");
                AppointmentRemoveResult appointmentRemoveResult = client.AppointmentRemove("U", appointmentCreateResult.AppointmentCreate.NewUnID);

                LogoutResult logoutResult = client.Logout();
            }
        }
    }
}

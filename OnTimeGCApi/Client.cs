﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace OnTimeGCApi
{
    public class Client
    {
        private const string DATETIME_FORMATTER = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
        private OtBase main;
        private string domain;
        private string apiPath;
        private string servletPath;
        private Uri servletEndpoint;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId">Application ID - must be present in the license key</param>
        /// <param name="applicationVersion">The application version number - useful for debugging</param>
        /// <param name="apiVersion">The version of the API you are using</param>
        /// <param name="domain"></param>
        /// <param name="apiPath"></param>
        /// <param name="servletPath"></param>
        public Client(string applicationId, string applicationVersion, int apiVersion, string domain, string apiPath, string servletPath)
        {

            this.main = new OtBase(applicationId, applicationVersion, apiVersion);
            this.domain = domain;
            this.apiPath = apiPath;
            this.servletPath = servletPath;
            this.servletEndpoint = new Uri($"{this.domain}{this.servletPath}");
        }

        /// <summary>
        /// Login and verify the token
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public LoginResult Login(string username, string password)
        {
            CookieContainer cc = new CookieContainer();
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("username", username);
            param.Add("password", password);
            param.Add("redirectto", "/names.nsf/?openicon");
            string payload = string.Join("&", param.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
            Uri uri = new Uri($"{this.domain}/names.nsf?Login");

            string response = Utilities.Post(uri, payload, ref cc);
            string magicHeader = BitConverter.ToString(Encoding.UTF8.GetBytes(response).Take(4).ToArray());
            if (magicHeader != "EF-BF-BD-50")
            {
                throw new Exception($"Invalid credentials / magicHeader doesn't match | {magicHeader} | {response}");
            }

            // token
            uri = new Uri($"{this.domain}{this.apiPath}/apihttptoken");
            payload = (new { Main = this.main }).ToJson();
            response = Utilities.Post(uri, payload, ref cc);

            BaseResult baseResult = response.ParseJson<BaseResult>();
            if (baseResult.Status != "OK")
            {
                throw new Exception($"Login failed | [{baseResult.ErrorCode}] {baseResult.Error}");
            }

            this.main.Token = baseResult.Token;

            // login
            payload = (new { Main = this.main, Login = (new { }) }).ToJson();
            response = Utilities.Post(this.servletEndpoint, payload, ref cc);

            LoginResult result = response.ParseJson<LoginResult>();
            if (result.Status != "OK")
            {
                throw new Exception($"Login failed | [{result.ErrorCode}] {result.Error}");
            }

            this.main.UpdateToken(result.Token);

            return result;
        }

        public LoginResult Login(string token)
        {
            this.main.Token = token;

            // login
            string payload = (new { Main = this.main, Login = (new { }) }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            LoginResult result = response.ParseJson<LoginResult>();
            if (result.Status != "OK")
            {
                throw new Exception($"Login failed | [{result.ErrorCode}] {result.Error}");
            }

            this.main.UpdateToken(result.Token);

            return result;
        }

        public GetTokenResult GenerateTokenOnBehalfOf(string emailAddress)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("ClientTime", DateTime.UtcNow.ToString(DATETIME_FORMATTER));

            string payload = (new { Main = this.main, GetToken = parameters }).ToJson();
            payload = payload.Replace("{\"Main\":{", $"{{\"Main\":{{\"OnBehalfOf\":\"{emailAddress}\",");
            string response = Utilities.Post(this.servletEndpoint, payload);

            GetTokenResult result = null;
            try
            {
                result = response.ParseJson<GetTokenResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        /// <summary>
        /// Tell the API that we're logging out. Not critical, more as good behavior and for logging
        /// </summary>
        /// <returns></returns>
        public LogoutResult Logout()
        {
            string payload = (new { Main = this.main, Logout = (new { }) }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            LogoutResult result = null;
            try
            {
                result = response.ParseJson<LogoutResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        /// <summary>
        /// Return some version information of API and running server
        /// </summary>
        /// <returns></returns>
        public VersionResult Version()
        {
            string payload = (new { Main = this.main, Version = (new { }) }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            VersionResult result = null;
            try
            {
                result = response.ParseJson<VersionResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        /// <summary>
        /// List of all users and their information
        /// </summary>
        /// <param name="items">List of items to return for each user. Blank fetches all</param>
        /// <param name="excludeIds">List of user IDs to exclude</param>
        /// <param name="includeNoAccess">Include information about users even though the active user doesn't have access to the calendar</param>
        /// <param name="type">Only return a specific type of users</param>
        /// <returns></returns>
        public UsersAllResult UsersAll(List<string> items = null, List<string> excludeIds = null, bool includeNoAccess = false, UserType type = UserType.All)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("IncludeNoAccess", includeNoAccess);
            parameters.Add("Type", type);

            if (items != null && items.Count != 0) { parameters.Add("Items", items); }
            if (excludeIds != null && excludeIds.Count != 0) { parameters.Add("ExcludeIDs", excludeIds); }

            string payload = (new { Main = this.main, UsersAll = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            UsersAllResult result = null;
            try
            {
                result = response.ParseJson<UsersAllResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        /// <summary>
        /// Information of specific users
        /// </summary>
        /// <param name="identities">List of identifies (OnTime ID, email, DN, shortname)</param>
        /// <param name="onTimeIds">List of OnTime ID's</param>
        /// <param name="emails">List of email addresses</param>
        /// <param name="shortNames">List of short names</param>
        /// <param name="distinguishedNames">List of distinguished names</param>
        /// <param name="groups">List of OnTime group IDs</param>
        /// <param name="excludeIds">List of OnTime IDs to exclude for the specified group(s)</param>
        /// <param name="items">List of items to return for each user. Blank fetches all</param>
        /// <returns></returns>
        public UsersInfoResult UsersInfo(List<string> identities = null, List<string> onTimeIds = null, List<string> emails = null, List<string> shortNames = null, List<string> distinguishedNames = null, List<string> groups = null, List<string> excludeIds = null, List<string> items = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (identities != null && identities.Count != 0) { parameters.Add("Any", identities); }
            if (onTimeIds != null && onTimeIds.Count != 0) { parameters.Add("IDs", onTimeIds); }
            if (emails != null && emails.Count != 0) { parameters.Add("Emails", emails); }
            if (shortNames != null && shortNames.Count != 0) { parameters.Add("ShortNames", shortNames); }
            if (distinguishedNames != null && distinguishedNames.Count != 0) { parameters.Add("DNs", distinguishedNames); }
            if (groups != null && groups.Count != 0) { parameters.Add("Groups", groups); }
            if (excludeIds != null && excludeIds.Count != 0) { parameters.Add("ExcludeIDs", excludeIds); }
            if (items != null && items.Count != 0) { parameters.Add("Items", items); }

            string payload = (new { Main = this.main, UsersInfo = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            UsersInfoResult result = null;
            try
            {
                result = response.ParseJson<UsersInfoResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        /// <summary>
        /// All calendar entries for specific users for a given time range
        /// </summary>
        /// <param name="onTimeIds">List of OnTime ID's</param>
        /// <param name="emails">List of email addresses</param>
        /// <param name="shortNames">List of short names</param>
        /// <param name="start">Start date time</param>
        /// <param name="end">End date time</param>
        /// <param name="timeOffOnly">Return TimeOff entries</param>
        /// <returns></returns>
        public CalendarsResult Calendars(DateTime start, DateTime end, List<string> onTimeIds = null, List<string> emails = null, List<string> shortNames = null, bool timeOffOnly = false)
        {
            if (start == null) { throw new ArgumentNullException("start"); }
            if (end == null) { throw new ArgumentNullException("end"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("FromDT", start.ToUniversalTime().ToString(DATETIME_FORMATTER));
            parameters.Add("ToDT", end.ToUniversalTime().ToString(DATETIME_FORMATTER));
            parameters.Add("TimeOffOnly", timeOffOnly);

            if (onTimeIds != null && onTimeIds.Count != 0) { parameters.Add("IDs", onTimeIds); }
            if (emails != null && emails.Count != 0) { parameters.Add("Emails", emails); }
            if (shortNames != null && shortNames.Count != 0) { parameters.Add("ShortNames", shortNames); }

            string payload = (new { Main = this.main, Calendars = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            // workaround for AppointmentType to be compatible with enumerations
            StringBuilder sb = new StringBuilder(response);
            sb.Replace("\"ApptType\":\"I\"", "\"ApptType\":\"4\"");
            sb.Replace("\"ApptType\":\"C\"", "\"ApptType\":\"5\"");
            sb.Replace("\"ApptType\":\"U\"", "\"ApptType\":\"6\"");
            sb.Replace("\"ApptType\":\"N\"", "\"ApptType\":\"7\"");
            sb.Replace("\"ApptType\":\"E\"", "\"ApptType\":\"8\"");
            sb.Replace("\"ApptType\":\"S\"", "\"ApptType\":\"9\"");
            sb.Replace("\"ApptType\":\"T\"", "\"ApptType\":\"10\"");
            sb.Replace("\"ApptType\":\"R\"", "\"ApptType\":\"11\"");

            response = sb.ToString();

            CalendarsResult result = null;
            try
            {
                result = response.ParseJson<CalendarsResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId">User OnTime ID to be created for</param>
        /// <param name="start">Start date time</param>
        /// <param name="end">End date time</param>
        /// <param name="subject">Subject on appointment</param>
        /// <param name="location">Location on appointment</param>
        /// <param name="categories">Categories on appointment</param>
        /// <param name="isPrivate"></param>
        /// <param name="isAvailable"></param>
        /// <param name="body">Body text in plain text. \n = newline, \t = horizontal tab, \\ = backslash</param>
        /// <param name="requiredAttendees">Requried attendees for meeting, only applicable for AppointmentType=Meeting</param>
        /// <param name="optionalAttendees">Optional attendees for meeting, only applicable for AppointmentType=Meeting</param>
        /// <param name="fyiAttendees">FYI attendees for meeting, only applicable for AppointmentType=Meeting</param>
        /// <param name="requiredRooms">Requried rooms for meeting, only applicable for AppointmentType=Meeting</param>
        /// <param name="requiredResources">Requried resources for meeting, only applicable for AppointmentType=Meeting</param>
        /// <param name="repeatDates">separated list of repeat dates for a repeating appointment, meeting or allday. If supplied the time component (of meetings and appointments) will be combined with the supplied dates from this field to form the meeting/appointment date/times.</param>
        /// <param name="customFields"></param>
        /// <returns></returns>
        public AppointmentCreateResult AppointmentCreate(EventType type, string userId, DateTime start, DateTime end, string subject, string location = null, List<string> categories = null, bool isPrivate = false, bool isAvailable = false, string body = null, List<string> requiredAttendees = null, List<string> optionalAttendees = null, List<string> fyiAttendees = null, List<string> requiredRooms = null, List<string> requiredResources = null, List<DateTime> repeatDates = null, Dictionary<string, object> customFields = null)
        {
            if (type == EventType.Invatation) { throw new ArgumentException("\"type\" cannot be an Invatation"); }
            if (userId == null || userId.Trim() == "") { throw new ArgumentNullException("userId"); }
            if (start == null) { throw new ArgumentNullException("start"); }
            if (end == null) { throw new ArgumentNullException("end"); }
            if (subject == null) { throw new ArgumentNullException("subject"); }
            if (type != EventType.Meeting && requiredAttendees != null && requiredAttendees.Count != 0) { throw new ArgumentException("\"requiredAttendees\" only applicable for Meetings"); }
            if (type != EventType.Meeting && optionalAttendees != null && optionalAttendees.Count != 0) { throw new ArgumentException("\"optionalAttendees\" only applicable for Meetings"); }
            if (type != EventType.Meeting && fyiAttendees != null && fyiAttendees.Count != 0) { throw new ArgumentException("\"fyiAttendees\" only applicable for Meetings"); }
            if (type != EventType.Meeting && requiredRooms != null && requiredRooms.Count != 0) { throw new ArgumentException("\"requiredRooms\" only applicable for Meetings"); }
            if (type != EventType.Meeting && requiredResources != null && requiredResources.Count != 0) { throw new ArgumentException("\"requiredResources\" only applicable for Meetings"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("AppointmentType", type);
            parameters.Add("UserID", userId);
            parameters.Add("StartDT", start.ToUniversalTime().ToString(DATETIME_FORMATTER));
            parameters.Add("EndDT", end.ToUniversalTime().ToString(DATETIME_FORMATTER));
            parameters.Add("Subject", subject);
            parameters.Add("Private", isPrivate);
            parameters.Add("Available", isAvailable);

            if (location != null) { parameters.Add("Location", location); }
            if (categories != null && categories.Count != 0) { parameters.Add("Categories", categories); }
            if (body != null) { parameters.Add("Body", body); }
            if (requiredAttendees != null && requiredAttendees.Count != 0) { parameters.Add("RequiredAttendees", requiredAttendees); }
            if (optionalAttendees != null && optionalAttendees.Count != 0) { parameters.Add("OptionalAttendees", optionalAttendees); }
            if (fyiAttendees != null && fyiAttendees.Count != 0) { parameters.Add("FYIAttendees", fyiAttendees); }
            if (requiredRooms != null && requiredRooms.Count != 0) { parameters.Add("RequiredRooms", requiredRooms); }
            if (requiredResources != null && requiredResources.Count != 0) { parameters.Add("RequiredResources", requiredResources); }
            if (repeatDates != null && repeatDates.Count != 0) { parameters.Add("RepeatDates", repeatDates); }
            if (customFields != null && customFields.Count != 0) { parameters.Add("CustomFields", customFields); }

            string payload = (new { Main = this.main, AppointmentCreate = parameters }).ToJson();

            // workaround for AppointmentType to be compatible with enumerations
            StringBuilder sb = new StringBuilder(payload);
            sb.Replace("\"AppointmentType\":0", "\"AppointmentType\":\"0\"");
            sb.Replace("\"AppointmentType\":1", "\"AppointmentType\":\"1\"");
            sb.Replace("\"AppointmentType\":2", "\"AppointmentType\":\"2\"");
            sb.Replace("\"AppointmentType\":3", "\"AppointmentType\":\"3\"");
            sb.Replace("\"AppointmentType\":4", "\"AppointmentType\":\"I\"");
            sb.Replace("\"AppointmentType\":5", "\"AppointmentType\":\"C\"");
            sb.Replace("\"AppointmentType\":6", "\"AppointmentType\":\"U\"");
            sb.Replace("\"AppointmentType\":7", "\"AppointmentType\":\"N\"");
            sb.Replace("\"AppointmentType\":8", "\"AppointmentType\":\"E\"");
            sb.Replace("\"AppointmentType\":9", "\"AppointmentType\":\"S\"");
            sb.Replace("\"AppointmentType\":10", "\"AppointmentType\":\"T\"");
            sb.Replace("\"AppointmentType\":11", "\"AppointmentType\":\"R\"");
            payload = sb.ToString();

            string response = Utilities.Post(this.servletEndpoint, payload);

            AppointmentCreateResult result = null;
            try
            {
                result = response.ParseJson<AppointmentCreateResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        /// <summary>
        /// Change an appointment, meeting or an allday event
        /// </summary>
        /// <param name="userId">OnTime ID to change calendar entry for</param>
        /// <param name="unId">The document UnID of the appointment you wish to change</param>
        /// <param name="start">New start date time</param>
        /// <param name="end">New end date time</param>
        /// <param name="lastModified">Last modified timestamp of the appointment - must match the actaual appointment LastMod value. If not specified the appointment is always modified.</param>
        /// <param name="newUserId">OnTime User ID to move the appointment to (if moving between people)</param>
        /// <param name="subject">Subject on appointment</param>
        /// <param name="location">Location on appointment</param>
        /// <param name="categories">Categories on appointment</param>
        /// <param name="isPrivate"></param>
        /// <param name="isAvailable"></param>
        /// <param name="body">Body text in plain text. \n = newline, \t = horizontal tab, \\ = backslash</param>
        /// <param name="requiredAttendees">Requried attendees for meeting, only applicable for AppType=Meeting</param>
        /// <param name="optionalAttendees">Optional attendees for meeting, only applicable for AppType=Meeting</param>
        /// <param name="fyiAttendees">FYI attendees for meeting, only applicable for AppType=Meeting</param>
        /// <param name="requiredRooms">Requried rooms for meeting, only applicable for AppType=Meeting</param>
        /// <param name="requiredResources">Requried resources for meeting, only applicable for AppType=Meeting</param>
        /// <param name="repeatInstance">Repeat instance you are working on for a repeating calendar entry. Obtained from the Calendar call and used together with RepWhich below to decide what to affect</param>
        /// <param name="repeatAction">For repeating calendar entries should the action affect 'this instance' (0), 'all instances' (1), 'this and previous instances' (2) or 'this and future instances' (3)</param>
        /// <param name="customFields"></param>
        /// <returns></returns>
        public AppointmentChangeResult AppointmentChange(string userId, string unId, DateTime start, DateTime end, string lastModified = null, string newUserId = null, string subject = null, string location = null, List<string> categories = null, bool isPrivate = false, bool isAvailable = false, string body = null, List<string> requiredAttendees = null, List<string> optionalAttendees = null, List<string> fyiAttendees = null, List<string> requiredRooms = null, List<string> requiredResources = null, string repeatInstance = null, int? repeatAction = null, Dictionary<string, object> customFields = null)
        {
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (unId == null) { throw new ArgumentNullException("unId"); }
            if (start == null) { throw new ArgumentNullException("start"); }
            if (end == null) { throw new ArgumentNullException("end"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserID", userId);
            parameters.Add("UnID", unId);
            parameters.Add("NewStartDT", start.ToUniversalTime().ToString(DATETIME_FORMATTER));
            parameters.Add("NewEndDT", end.ToUniversalTime().ToString(DATETIME_FORMATTER));
            parameters.Add("NewPrivate", isPrivate);
            parameters.Add("NewAvailable", isAvailable);

            if (lastModified != null) { parameters.Add("LastMod", lastModified); }
            if (newUserId != null) { parameters.Add("NewUserID", newUserId); }
            if (subject != null) { parameters.Add("NewSubject", subject); }
            if (location != null) { parameters.Add("NewLocation", location); }
            if (categories != null && categories.Count != 0) { parameters.Add("NewCategories", categories); }
            if (body != null) { parameters.Add("NewBody", body); }
            if (requiredAttendees != null && requiredAttendees.Count != 0) { parameters.Add("NewRequiredAttendees", requiredAttendees); }
            if (optionalAttendees != null && optionalAttendees.Count != 0) { parameters.Add("NewOptionalAttendees", optionalAttendees); }
            if (fyiAttendees != null && fyiAttendees.Count != 0) { parameters.Add("NewFYIAttendees", fyiAttendees); }
            if (requiredRooms != null && requiredRooms.Count != 0) { parameters.Add("NewRequiredRooms", requiredRooms); }
            if (requiredResources != null && requiredResources.Count != 0) { parameters.Add("NewRequiredResources", requiredResources); }
            if (repeatInstance != null) { parameters.Add("RepIns", repeatInstance); }
            if (repeatAction != null) { parameters.Add("RepWhich", repeatAction); }
            if (customFields != null && customFields.Count != 0) { parameters.Add("NewCustomFields", customFields); }

            string payload = (new { Main = this.main, AppointmentChange = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            AppointmentChangeResult result = null;
            try
            {
                result = response.ParseJson<AppointmentChangeResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        /// <summary>
        /// Removes an appointment
        /// </summary>
        /// <param name="userId">OnTime User ID for appointment owner</param>
        /// <param name="unId">The document UnID of the appointment you wish to remove</param>
        /// <param name="lastModified">Last modified timestamp of the appointment - must match the actaual appointment LastMod value. If not specified the appointment is always removed.</param>
        /// <param name="repeatInstance">Repeat instance you are working on for a repeating calendar entry. Obtained from the Calendar call and used together with RepWhich below to decide what to affect</param>
        /// <param name="repeatAction">For repeating calendar entries should the action affect 'this instance' (0), 'all instances' (1), 'this and previous instances' (2) or 'this and future instances' (3)</param>
        /// <returns></returns>
        public AppointmentRemoveResult AppointmentRemove(string userId, string unId, string lastModified = null, string repeatInstance = null, int? repeatAction = null)
        {
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (unId == null) { throw new ArgumentNullException("unId"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserID", userId);
            parameters.Add("UnID", unId);

            if (lastModified != null) { parameters.Add("LastMod", lastModified); }
            if (repeatInstance != null) { parameters.Add("RepIns", repeatInstance); }
            if (repeatAction != null) { parameters.Add("RepWhich", repeatAction); }

            string payload = (new { Main = this.main, AppointmentRemove = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            AppointmentRemoveResult result = null;
            try
            {
                result = response.ParseJson<AppointmentRemoveResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        /// <summary>
        /// List groups
        /// </summary>
        /// <param name="includePublic">If true result is filted to include public groups</param>
        /// <param name="includePrivate">If true result is filted to include private groups</param>
        /// <param name="includeShared">If true result is filted to include shared groups</param>
        /// <returns></returns>
        public GroupListResult GroupList(bool? includePublic, bool? includePrivate, bool? includeShared)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("OnlyLevel", "");

            if (includePublic != null) { parameters.Add("InclPublic", includePublic); }
            if (includePrivate != null) { parameters.Add("InclPrivate", includePrivate); }
            if (includeShared != null) { parameters.Add("InclShared", includeShared); }

            string payload = (new { Main = this.main, GroupList = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            GroupListResult result = null;
            try
            {
                result = response.ParseJson<GroupListResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        /// <summary>
        /// List of User IDs for given a single group ID
        /// </summary>
        /// <param name="groupId">Group ID for which to return user IDs</param>
        /// <returns></returns>
        public GroupUserIdsResult GroupUserIds(string groupId)
        {
            if (groupId == null) { throw new ArgumentNullException("groupId"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("GroupID", groupId);

            string payload = (new { Main = this.main, GroupUserIDs = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            GroupUserIdsResult result = null;
            try
            {
                result = response.ParseJson<GroupUserIdsResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        public MailContactsListResult MailContactsList(string userId, List<string> additionalFields = null)
        {
            if (this.servletPath == null) { throw new NullReferenceException("servletPath is null"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserID", userId);

            if (additionalFields != null) { parameters.Add("Fields", additionalFields); }

            string payload = (new { Main = this.main, MailContactsList = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            MailContactsListResult result = null;
            try
            {
                result = response.ParseJson<MailContactsListResult>();
                this.main.UpdateToken(result.Token);

                if (additionalFields != null)
                {
                    Dictionary<string, object> temp = response.ParseJson<Dictionary<string, object>>();
                    temp = (Dictionary<string, object>)temp["MailContactsList"];
                    foreach (Dictionary<string, object> contact in (System.Collections.ArrayList)temp["Contacts"])
                    {
                        string unId = contact["UnID"].ToString();
                        Contact c = result.MailContactsList.Contacts.Find((x) => { return (x.UnID == unId); });
                        c.Fields = new Dictionary<string, object>();

                        foreach (string item in additionalFields)
                        {
                            c.Fields.Add(item, contact[item]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        public MailContactsCreateResult MailContactCreate(string userId, string emailAddress, string title = null, string firstName = null, string lastName = null, string companyName = null, string department = null, string jobTitle = null, string officePhoneNumber = null, string cellPhoneNumber = null, List<string> categories = null, Dictionary<string, string> additionalFields = null)
        {
            if (this.servletPath == null) { throw new NullReferenceException("servletPath is null"); }
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (emailAddress == null) { throw new ArgumentNullException("emailAddress"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserID", userId);
            parameters.Add("MailAddress", emailAddress);

            if (title != null) { parameters.Add("Title", title); }
            if (firstName != null) { parameters.Add("FirstName", firstName); }
            if (lastName != null) { parameters.Add("LastName", lastName); }
            if (companyName != null) { parameters.Add("CompanyName", companyName); }
            if (department != null) { parameters.Add("Department", department); }
            if (jobTitle != null) { parameters.Add("JobTitle", jobTitle); }
            if (officePhoneNumber != null) { parameters.Add("OfficePhoneNumber", officePhoneNumber); }
            if (cellPhoneNumber != null) { parameters.Add("CellPhoneNumber", cellPhoneNumber); }
            if (categories != null && categories.Count != 0) { parameters.Add("Categories", categories); }

            if (additionalFields != null)
            {
                foreach (KeyValuePair<string, string> item in additionalFields)
                {
                    parameters.Add(item.Key, item.Value);
                }
            }

            string payload = (new { Main = this.main, MailContactsCreate = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            MailContactsCreateResult result = null;
            try
            {
                result = response.ParseJson<MailContactsCreateResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        public MailContactsReadResult MailContactsRead(string userId, string unId, List<string> additionalFields = null)
        {
            if (this.servletPath == null) { throw new NullReferenceException("servletPath is null"); }
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (unId == null) { throw new ArgumentNullException("unId"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserID", userId);
            parameters.Add("UnID", unId);

            if (additionalFields != null) { parameters.Add("Fields", additionalFields); }

            string payload = (new { Main = this.main, MailContactsRead = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            MailContactsReadResult result = null;
            try
            {
                result = response.ParseJson<MailContactsReadResult>();
                this.main.UpdateToken(result.Token);

                if (additionalFields != null && additionalFields.Count != 0)
                {
                    Dictionary<string, object> temp = response.ParseJson<Dictionary<string, object>>();
                    temp = (Dictionary<string, object>)temp["MailContactsRead"];
                    temp = (Dictionary<string, object>)temp["Contact"];
                    result.MailContactsRead.Contact.Fields = new Dictionary<string, object>();
                    foreach (string item in additionalFields)
                    {
                        if (temp.ContainsKey(item))
                        {
                            result.MailContactsRead.Contact.Fields.Add(item, temp[item]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        public MailContactsChangeResult MailContactsChange(string userId, string unId, string fullName = null, string mailAddress = null, string title = null, string companyName = null, string department = null, string jobTitle = null, string officePhoneNumber = null, string cellPhoneNumber = null, List<string> categories = null, Dictionary<string, string> additionalFields = null)
        {
            if (this.servletPath == null) { throw new NullReferenceException("servletPath is null"); }
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (unId == null) { throw new ArgumentNullException("unId"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserID", userId);
            parameters.Add("UnID", unId);

            if (fullName != null) { parameters.Add("FullName", fullName); }
            if (mailAddress != null) { parameters.Add("MailAddress", mailAddress); }
            if (title != null) { parameters.Add("Title", title); }
            if (companyName != null) { parameters.Add("CompanyName", companyName); }
            if (department != null) { parameters.Add("Department", department); }
            if (jobTitle != null) { parameters.Add("JobTitle", jobTitle); }
            if (officePhoneNumber != null) { parameters.Add("OfficePhoneNumber", officePhoneNumber); }
            if (cellPhoneNumber != null) { parameters.Add("CellPhoneNumber", cellPhoneNumber); }
            if (categories != null && categories.Count != 0) { parameters.Add("Categories", categories); }

            if (additionalFields != null)
            {
                foreach (KeyValuePair<string, string> item in additionalFields)
                {
                    parameters.Add(item.Key, item.Value);
                }
            }

            string payload = (new { Main = this.main, MailContactsChange = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            MailContactsChangeResult result = null;
            try
            {
                result = response.ParseJson<MailContactsChangeResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        public MailContactsRemoveResult MailContactsRemove(string userId, string unId)
        {
            if (this.servletPath == null) { throw new NullReferenceException("servletPath is null"); }
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (unId == null) { throw new ArgumentNullException("unId"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserID", userId);
            parameters.Add("UnID", unId);

            string payload = (new { Main = this.main, MailContactsRemove = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            MailContactsRemoveResult result = null;
            try
            {
                result = response.ParseJson<MailContactsRemoveResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        public CalendarsModifiedResult CalendarsModified(string changeKey = null)
        {
            if (this.servletPath == null) { throw new NullReferenceException("servletPath is null"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (changeKey != null) { parameters.Add("ChangeKey", changeKey); }

            string payload = (new { Main = this.main, CalendarsModified = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            CalendarsModifiedResult result = null;
            try
            {
                result = response.ParseJson<CalendarsModifiedResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        public MailListResult MailList(string userId, DateTime from, DateTime to)
        {
            if (this.servletPath == null) { throw new NullReferenceException("servletPath is null"); }
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (from == null) { throw new ArgumentNullException("from"); }
            if (to == null) { throw new ArgumentNullException("to"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserID", userId);
            parameters.Add("FromDT", from.ToUniversalTime().ToString(DATETIME_FORMATTER));
            parameters.Add("ToDT", to.ToUniversalTime().ToString(DATETIME_FORMATTER));

            string payload = (new { Main = this.main, MailList = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            MailListResult result = null;
            try
            {
                result = response.ParseJson<MailListResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        public MailReadResult MailRead(string userId, string unId, string[] fields, string[] extraItems = null, string[] attachments = null, bool bodyHtml = true, bool bodyMime = false)
        {
            if (this.servletPath == null) { throw new NullReferenceException("servletPath is null"); }
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (unId == null) { throw new ArgumentNullException("unId"); }
            if (fields == null) { throw new ArgumentNullException("fields"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserID", userId);
            parameters.Add("UnID", unId);
            parameters.Add("Fields", fields);
            parameters.Add("BodyHtml", bodyHtml);
            parameters.Add("BodyMime", bodyMime);

            if (extraItems != null && extraItems.Length != 0) { parameters.Add("ExtraItems", extraItems); }
            if (attachments != null && attachments.Length != 0) { parameters.Add("Attachments", attachments); }

            string payload = (new { Main = this.main, MailRead = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            MailReadResult result = null;
            try
            {
                result = response.ParseJson<MailReadResult>();
                this.main.UpdateToken(result.Token);

                if (fields != null && fields.Length != 0)
                {
                    Dictionary<string, object> temp = response.ParseJson<Dictionary<string, object>>();
                    temp = (Dictionary<string, object>)temp["MailRead"];
                    temp = (Dictionary<string, object>)temp["Mail"];
                    result.MailRead.Mail.Fields = new Dictionary<string, string>();
                    foreach (string item in fields)
                    {
                        if (temp.ContainsKey(item))
                        {
                            result.MailRead.Mail.Fields.Add(item, (string)temp[item]);
                        }
                    }
                }

                if (extraItems != null && extraItems.Length != 0)
                {
                    Dictionary<string, object> temp = response.ParseJson<Dictionary<string, object>>();
                    temp = (Dictionary<string, object>)temp["MailRead"];
                    temp = (Dictionary<string, object>)temp["Mail"];
                    temp = (Dictionary<string, object>)temp["ExtraItems"];

                    result.MailRead.Mail.Extraitems = new Dictionary<string, MailReadExtraItem>();
                    foreach (string item in fields)
                    {
                        if (temp.ContainsKey(item))
                        {
                            result.MailRead.Mail.Extraitems.Add(item, temp["Subject"].ToJson().ParseJson<MailReadExtraItem>());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        public MailSendResult MailSend(string[] sendTo, string subject, string body, string[] copyTo = null, string[] blindCopyTo = null, bool signature = false, MailSendAttachment[] attachments = null)
        {
            if (this.servletPath == null) { throw new NullReferenceException("servletPath is null"); }
            if (sendTo == null || sendTo.Length == 0) { throw new ArgumentNullException("sendTo"); }
            if (subject == null) { throw new ArgumentNullException("subject"); }
            if (body == null) { throw new ArgumentNullException("body"); }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("SendTo", sendTo);
            parameters.Add("Subject", subject);
            parameters.Add("Body", body);
            parameters.Add("MailSignature", signature);

            if (copyTo != null && copyTo.Length != 0) { parameters.Add("CopyTo", copyTo); }
            if (blindCopyTo != null && blindCopyTo.Length != 0) { parameters.Add("BlindCopyTo", blindCopyTo); }
            if (attachments != null && attachments.Length != 0) { parameters.Add("Attachments", attachments); }

            string payload = (new { Main = this.main, MailSend = parameters }).ToJson();
            string response = Utilities.Post(this.servletEndpoint, payload);

            MailSendResult result = null;
            try
            {
                result = response.ParseJson<MailSendResult>();
                this.main.UpdateToken(result.Token);
            }
            catch (Exception ex)
            {
                throw new Exception(response, ex);
            }

            return result;
        }

        private class OtBase
        {
            public string ApplID { get; set; }
            public string ApplVer { get; set; }
            public int APIVer { get; set; }
            public string Token { get; set; }

            public OtBase(string applicationId, string applicationVersion, int apiVersion)
            {
                this.ApplID = applicationId;
                this.ApplVer = applicationVersion;
                this.APIVer = apiVersion;
            }

            public void UpdateToken(string newToken)
            {
                if (this.APIVer > 4 && newToken != null)
                {
                    this.Token = newToken;
                }
            }
        }
    }
}

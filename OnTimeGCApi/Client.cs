/*
    Name: OnTimeGCApi
    Description: OnTime Groupcalendar API
    Version: 1.0  
    Author: Oliver Haucke  
    Author URI: http://www.qkom.de/  
    E-Mail: ohaucke@qkom.de  
    License: BSD 2-Clause  
    License URI: http://opensource.org/licenses/BSD-2-Clause
    Github: https://github.com/QKom/OnTimeGCApi
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace OnTimeGCApi
{
    public class Client
    {
        private OtBase main;
        private string domain;
        private string apiPath;
        private Uri apiEndpoint;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId">Application ID - must be present in the license key</param>
        /// <param name="applicationVersion">The application version number - useful for debugging</param>
        /// <param name="apiVersion">The version of the API you are using</param>
        /// <param name="domain"></param>
        /// <param name="apiPath"></param>
        public Client(string applicationId, string applicationVersion, int apiVersion, string domain, string apiPath)
        {
            this.main = new OtBase(applicationId, applicationVersion, apiVersion);
            this.domain = domain;
            this.apiPath = apiPath;
            this.apiEndpoint = new Uri(string.Format("{0}{1}/apihttp", this.domain, this.apiPath));
        }

        /// <summary>
        /// Login and verify the token
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public Login.Base Login(string username, string password)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("username", username);
            param.Add("password", password);
            param.Add("redirectto", "/names.nsf/$about");
            string payload = string.Join("&", param.Select(kvp => string.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value))));
            CookieContainer cc = new CookieContainer();
            Uri uri = new System.Uri(string.Format("{0}/names.nsf?Login", this.domain));

            string response = Utilities.Post(uri, payload, ref cc);
            if (!response.Contains("About the Domino Directory"))
            {
                throw new Exception("Invalid credentials.");
            }

            // token
            uri = new Uri(string.Format("{0}{1}/apihttptoken", this.domain, this.apiPath));
            payload = (new { Main = this.main }).ToJson();
            response = Utilities.Post(uri, payload, ref cc);

            BaseResult baseResult = response.ParseJson<BaseResult>();
            if (baseResult.Status != "OK")
            {
                throw new Exception(string.Format("Login failed | [{0}] {1}", baseResult.ErrorCode, baseResult.Error));
            }

            this.main.Token = baseResult.Token;

            // login
            payload = (new { Main = this.main, Login = (new { }) }).ToJson();
            response = Utilities.Post(this.apiEndpoint, payload, ref cc);

            Login.Base result = response.ParseJson<Login.Base>();
            if (result.Status != "OK")
            {
                throw new Exception(string.Format("Login failed | [{0}] {1}", result.ErrorCode, result.Error));
            }

            this.main.UpdateToken(result.Token);

            return result;
        }

        /// <summary>
        /// Tell the API that we're logging out. Not critical, more as good behavior and for logging
        /// </summary>
        /// <returns></returns>
        public Logout.Base Logout()
        {
            string payload = (new { Main = this.main, Logout = (new { }) }).ToJson();
            string response = Utilities.Post(this.apiEndpoint, payload);

            Logout.Base result = response.ParseJson<Logout.Base>();
            this.main.UpdateToken(result.Token);

            return result;
        }

        /// <summary>
        /// Return some version information of API and running server
        /// </summary>
        /// <returns></returns>
        public Version.Base Version()
        {
            string payload = (new { Main = this.main, Version = (new { }) }).ToJson();
            string response = Utilities.Post(this.apiEndpoint, payload);

            Version.Base result = response.ParseJson<Version.Base>();
            this.main.UpdateToken(result.Token);

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
        public UsersAll.Base UsersAll(List<string> items, List<string> excludeIds, bool includeNoAccess = false, UserType type = UserType.All)
        {
            if (items == null) { items = new List<string>(); }
            if (excludeIds == null) { excludeIds = new List<string>(); }

            string payload = (new { Main = this.main, UsersAll = (new { Items = items, ExcludeIDs = excludeIds, IncludeNoAccess = includeNoAccess, Type = type }) }).ToJson();
            string response = Utilities.Post(this.apiEndpoint, payload);

            UsersAll.Base result = response.ParseJson<UsersAll.Base>();
            this.main.UpdateToken(result.Token);

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
        public UsersInfo.Base UsersInfo(List<string> identities, List<string> onTimeIds, List<string> emails, List<string> shortNames, List<string> distinguishedNames, List<string> groups, List<string> excludeIds, List<string> items)
        {
            if (this.main.APIVer == 3 && identities != null) { throw new Exception("Parameter identities is not supported with api version 3"); }
            if (identities == null) { identities = new List<string>(); }
            if (onTimeIds == null) { onTimeIds = new List<string>(); }
            if (emails == null) { emails = new List<string>(); }
            if (shortNames == null) { shortNames = new List<string>(); }
            if (distinguishedNames == null) { distinguishedNames = new List<string>(); }
            if (groups == null) { groups = new List<string>(); }
            if (excludeIds == null) { excludeIds = new List<string>(); }
            if (items == null) { items = new List<string>(); }

            string payload = (new { Main = this.main, UsersInfo = (new { Any = identities, IDs = onTimeIds, Emails = emails, ShortNames = shortNames, DNs = distinguishedNames, Groups = groups, ExcludeIDs = excludeIds, Items = items }) }).ToJson();
            string response = Utilities.Post(this.apiEndpoint, payload);

            UsersInfo.Base result = response.ParseJson<UsersInfo.Base>();
            this.main.UpdateToken(result.Token);

            return result;
        }

        /// <summary>
        /// Alle calendar entries for specific users for a given time range
        /// </summary>
        /// <param name="onTimeIds">List of OnTime ID's</param>
        /// <param name="emails">List of email addresses</param>
        /// <param name="shortNames">List of short names</param>
        /// <param name="start">Start date time</param>
        /// <param name="end">End date time</param>
        /// <param name="timeOffOnly">Return TimeOff entries</param>
        /// <returns></returns>
        public Calendars.Base Calendars(List<string> onTimeIds, List<string> emails, List<string> shortNames, DateTime start, DateTime end, bool timeOffOnly = false)
        {
            if (onTimeIds == null) { onTimeIds = new List<string>(); }
            if (emails == null) { emails = new List<string>(); }
            if (shortNames == null) { shortNames = new List<string>(); }

            string payload = (new { Main = this.main, Calendars = (new { IDs = onTimeIds, Emails = emails, ShortNames = shortNames, FromDT = start.ToUniversalTime().ToString("o"), ToDT = end.ToUniversalTime().ToString("o"), TimeOffOnly = timeOffOnly }) }).ToJson();
            string response = Utilities.Post(this.apiEndpoint, payload);

            Calendars.Base result = response.ParseJson<Calendars.Base>();
            this.main.UpdateToken(result.Token);

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
                if (this.APIVer > 4)
                {
                    this.Token = newToken;
                }
            }
        }
    }
}

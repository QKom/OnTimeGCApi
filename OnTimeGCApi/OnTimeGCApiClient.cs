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
    public class OnTimeGCApiClient
    {
        private OtBase main;
        private string domain;
        private string apiPath;
        private Uri apiEndpoint;

        public OnTimeGCApiClient(string applicationId, string applicationVersion, int apiVersion, string domain, string apiPath)
        {
            this.main = new OtBase(applicationId, applicationVersion, apiVersion);
            this.domain = domain;
            this.apiPath = apiPath;
            this.apiEndpoint = new Uri(string.Format("{0}{1}/apihttp", this.domain, this.apiPath));
        }

        public Login.Base Login(string username, string password)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("username", username);
            param.Add("password", password);
            param.Add("redirectto", "/names.nsf/$about");
            string payload = string.Join("&", param.Select(kvp => string.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value))));
            CookieContainer cc = new CookieContainer();
            Uri uri = new System.Uri(string.Format("{0}/names.nsf?Login", this.domain));

            string response = Utilities.Post(uri, payload, ref cc, Encoding.UTF8);
            if (!response.Contains("About the Domino Directory"))
            {
                throw new Exception("Invalid credentials.");
            }

            // token
            uri = new Uri(string.Format("{0}{1}/apihttptoken", this.domain, this.apiPath));
            payload = (new { Main = this.main }).ToJson();
            response = Utilities.Post(uri, payload, ref cc, Encoding.UTF8);

            BaseResult baseResult = response.ParseJson<BaseResult>();
            if (baseResult.Status != "OK")
            {
                throw new Exception(string.Format("Login failed | [{0}] {1}", baseResult.ErrorCode, baseResult.Error));
            }
            this.main.Token = baseResult.Token;

            // login
            payload = (new { Main = this.main, Login = (new { }) }).ToJson();
            response = Utilities.Post(this.apiEndpoint, payload, ref cc, Encoding.UTF8);

            Login.Base result = response.ParseJson<Login.Base>();
            if (result.Status != "OK")
            {
                throw new Exception(string.Format("Login failed | [{0}] {1}", result.ErrorCode, result.Error));
            }
            this.main.UpdateToken(result.Token);

            return result;
        }

        public Logout.Base Logout()
        {
            string payload = (new { Main = this.main, Logout = (new { }) }).ToJson();
            string response = Utilities.Post(this.apiEndpoint, payload, Encoding.UTF8);

            Logout.Base result = response.ParseJson<Logout.Base>();
            this.main.UpdateToken(result.Token);

            return result;
        }

        public UsersAll.Base UsersAll(List<string> items, List<string> excludeIds, bool includeNoAccess = false, UsersAll.UserType type = OnTimeGCApi.UsersAll.UserType.All)
        {
            if (items == null)
            {
                items = new List<string>();
            }
            if (excludeIds == null)
            {
                excludeIds = new List<string>();
            }

            string payload = (new { Main = this.main, UsersAll = (new { Items = items, ExcludeIDs = excludeIds, IncludeNoAccess = includeNoAccess, Type = type }) }).ToJson();
            string response = Utilities.Post(this.apiEndpoint, payload, Encoding.UTF8);

            UsersAll.Base result = response.ParseJson<UsersAll.Base>();
            this.main.UpdateToken(result.Token);

            return result;
        }

        public Version.Base Version()
        {
            string payload = (new { Main = this.main, Version = (new { }) }).ToJson();
            string response = Utilities.Post(this.apiEndpoint, payload, Encoding.UTF8);

            Version.Base result = response.ParseJson<Version.Base>();
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

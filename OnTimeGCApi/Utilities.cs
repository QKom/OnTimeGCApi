using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;

namespace OnTimeGCApi
{
    public static class Utilities
    {
        public static string ToJson<T>(this T source)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(source);
        }

        public static T ParseJson<T>(this string source)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Deserialize<T>(source);
        }

        public static string ToNullString<T>(this T source)
        {
            return (source == null ? "NULL" : source.ToString());
        }

        private static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }

        public static string Post(Uri endPoint, string postData, string referer = "")
        {
            CookieContainer temp = new CookieContainer();
            return Post(endPoint, postData, ref temp, referer);
        }

        public static string Post(Uri endPoint, string postData, ref CookieContainer cc, string referer = "")
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
            request.CookieContainer = cc;

            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:20.0) Gecko/20100101 Firefox/20.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Language: de-de,de;q=0.8,en-us;q=0.5,en;q=0.3");
            request.Headers.Add("Accept-Encoding: gzip, deflate");
            request.ContentLength = data.Length;
            if (referer != "")
            {
                request.Referer = referer;
            }

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = (response.ContentEncoding.ToLower().Contains("gzip") ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress) : response.GetResponseStream()))

            using (StreamReader buffer = new StreamReader(responseStream, Encoding.UTF8))
            {
                return buffer.ReadToEnd();
            }
        }
    }
}

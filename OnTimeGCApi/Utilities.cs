﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;

namespace OnTimeGCApi
{
    internal static class Utilities
    {
        internal static string ToJson<T>(this T source)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue - 1;

            return serializer.Serialize(source);
        }

        internal static T ParseJson<T>(this string source)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue - 1;

            return serializer.Deserialize<T>(source);
        }

        internal static string ToNullString<T>(this T source)
        {
            return (source == null ? "NULL" : source.ToString());
        }

        private static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }

        internal static string Post(Uri endPoint, string postData, string referer = "")
        {
            CookieContainer temp = new CookieContainer();
            return Post(endPoint, postData, ref temp, referer);
        }

        internal static string Post(Uri endPoint, string postData, ref CookieContainer cc, string referer = "")
        {
            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
            request.CookieContainer = cc;

            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:20.0) Gecko/20100101 Firefox/20.0";
            if (postData.StartsWith("{"))
            {
                request.ContentType = "application/json; charset=utf-8";
            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            request.Headers.Add("Accept-Encoding: gzip, deflate");
            request.ContentLength = data.Length;
            request.Timeout = 120000;
            if (referer != "")
            {
                request.Referer = referer;
            }

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = (response.ContentEncoding.ToLower().Contains("gzip") ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress) : response.GetResponseStream()))

                using (StreamReader buffer = new StreamReader(responseStream, Encoding.UTF8))
                {
                    return buffer.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    throw ex;
                }

                using (HttpWebResponse response = (HttpWebResponse)ex.Response)
                using (Stream responseStream = (response.ContentEncoding.ToLower().Contains("gzip") ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress) : response.GetResponseStream()))

                using (StreamReader buffer = new StreamReader(responseStream, Encoding.UTF8))
                {
                    return buffer.ReadToEnd();
                }
            }
        }
    }
}

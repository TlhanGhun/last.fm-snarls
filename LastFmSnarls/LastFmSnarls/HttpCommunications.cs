using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;


namespace LastFmSnarls
{
    class HttpCommunications
    {
        public static Response SendPostRequest(string url, object data, bool allowAutoRedirect)
        {
            try
            {
                string formData = string.Empty;
                HttpCommunications.GetProperties(data).ToList().ForEach(x =>
                {
                    string key = x.Key;
                    if (x.Key == "newone") { 
                        // this is a workaround as new is a command in C#...
                        key = "new"; 
                    }
                    formData += string.Format("{0}={1}&", key, x.Value);
                });
                formData = formData.TrimEnd('&');

                url = ProcessUrl(url);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.AllowAutoRedirect = allowAutoRedirect;
                request.Accept = "*/*";
                request.UserAgent = "last.fm snarls (http://www.tlhan-ghun.de/)";
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] encodedData = new UTF8Encoding().GetBytes(formData);
                request.ContentLength = encodedData.Length;

                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(encodedData, 0, encodedData.Length);
                }

                return new Response(GetResponse(request));
            }
            catch (System.Exception e)
            {
                return null;
            }
        }


        public static Response SendGetRequest(string url, object data, bool allowAutoRedirect)
        {
            string paramData = string.Empty;
            HttpCommunications.GetProperties(data).ToList().ForEach(x =>
            {
                paramData += string.Format("{0}={1}&", x.Key, x.Value);
            });
            paramData = paramData.TrimEnd('&');

            url = ProcessUrl(url);

            HttpWebRequest request = paramData.Length > 0 ? (HttpWebRequest)WebRequest.Create(string.Format("{0}?{1}", url, paramData)) : (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = allowAutoRedirect;


            return new Response(GetResponse(request));
        }

        #region Private

        private static string ProcessUrl(string url)
        {
            string questionMarkSymbol = "?";
            if (url.Contains(questionMarkSymbol))
            {
                url = url.Replace(questionMarkSymbol, System.Web.HttpUtility.UrlEncode(questionMarkSymbol));
            }
            return url;
        }

        private static HttpWebResponse GetResponse(HttpWebRequest request)
        {

            HttpWebResponse response;
            try
            {
                HttpWebResponse responseTemp = (HttpWebResponse)request.GetResponse();
                response = responseTemp;
            }
            catch (System.Exception e)
            {
                // some proxys have problems with Continue-100 headers
                request.ProtocolVersion = HttpVersion.Version10;
                request.ServicePoint.Expect100Continue = false;
                System.Net.ServicePointManager.Expect100Continue = false;
                HttpWebResponse responseTemp = (HttpWebResponse)request.GetResponse();
                response = responseTemp;
                System.Console.WriteLine(e.Message);
            }

            return response;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetProperties(object o)
        {
            foreach (var p in o.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                yield return new KeyValuePair<string, string>(p.Name.TrimStart('_'), System.Web.HttpUtility.UrlEncode(p.GetValue(o, null).ToString()));
            }
        }

        #endregion
    }
}

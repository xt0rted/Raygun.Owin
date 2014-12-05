using System.Collections.Generic;
using System.IO;
using System.Linq;

using Raygun.Owin;

namespace Raygun.Messages
{
    using OwinEnvironment = IDictionary<string, object>;

    public class RaygunRequestMessage
    {
        public RaygunRequestMessage(OwinEnvironment environment)
        {
            var request = new OwinRequest(environment);

            Url = request.Uri.ToString();
            HostName = request.Host.Value;
            HttpMethod = request.Method;
            IPAddress = request.RemoteIpAddress;
            QueryString = request.Query.ToDictionary(_ => _.Key, _ => string.Join(", ", _.Value));
            Headers = request.Headers.ToDictionary(_ => _.Key, _ => string.Join(", ", _.Value));

            // ToDo: load form data?

            if (request.ContentType != "text/html" && request.ContentType != "application/x-www-form-urlencoded" && request.Method != "GET")
            {
                int length = 4096;
                string temp = new StreamReader(request.Body).ReadToEnd();
                if (temp.Length < length)
                {
                    length = temp.Length;
                }

                RawData = temp.Substring(0, length);
            }
        }

        public string HostName { get; set; }
        public string Url { get; set; }
        public string HttpMethod { get; set; }
        public string IPAddress { get; set; }
        public IDictionary<string, string> QueryString { get; set; }
        //public IDictionary Data { get; set; }
        //public IDictionary Form { get; set; }
        public string RawData { get; set; }
        public IDictionary<string, string> Headers { get; set; }
    }
}
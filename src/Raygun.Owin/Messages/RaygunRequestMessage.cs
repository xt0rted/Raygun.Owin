namespace Raygun.Messages
{
    using System.Collections.Generic;

    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public class RaygunRequestMessage
    {
        public string HostName { get; set; }
        public string Url { get; set; }
        public string HttpMethod { get; set; }
        public string IPAddress { get; set; }
        public IDictionary<string, string> QueryString { get; set; }
        public IDictionary<string, string> Form { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string RawData { get; set; }
    }
}
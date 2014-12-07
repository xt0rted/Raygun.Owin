namespace Raygun.Builders
{
    using System.IO;
    using System.Linq;

    using Raygun.Messages;
    using Raygun.Owin;

    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public static class RaygunRequestMessageBuilder
    {
        public static RaygunRequestMessage Build(OwinEnvironment environment)
        {
            var request = new OwinRequest(environment);

            // ToDo: limit querystring, form, and header values to 256 characters
            var message = new RaygunRequestMessage();

            message.HostName = request.Host.Value;
            message.Url = request.Uri.ToString();
            message.HttpMethod = request.Method;
            message.IPAddress = request.RemoteIpAddress; // ToDo: bring this up to par with the official client

            // ToDo: filter querystring values
            message.QueryString = request.Query.ToDictionary(_ => _.Key, _ => string.Join(", ", _.Value));

            // ToDo: filter form values (add 'password' by default?)
            message.Form = request.ReadForm().ToDictionary(_ => _.Key, _ => string.Join(", ", _.Value));

            // ToDo: filter headers
            message.Headers = request.Headers.ToDictionary(_ => _.Key, _ => string.Join(", ", _.Value));
            message.Headers.Remove("Cookie");

            if (request.ContentType != "text/html" && request.ContentType != "application/x-www-form-urlencoded" && request.Method != "GET")
            {
                int length = 4096;
                string temp = new StreamReader(request.Body).ReadToEnd();
                if (temp.Length < length)
                {
                    length = temp.Length;
                }

                message.RawData = temp.Substring(0, length);
            }

            return message;
        }
    }
}
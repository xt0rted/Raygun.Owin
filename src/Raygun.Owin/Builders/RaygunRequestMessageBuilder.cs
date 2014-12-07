namespace Raygun.Builders
{
    using System;
    using System.Collections.Generic;
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

            message.Cookies = GetCookies(request.Cookies);

            if (request.ContentType != "text/html" && request.ContentType != "application/x-www-form-urlencoded" && request.Method != "GET")
            {
                var text = request.BodyAsString();

                message.RawData = text.Substring(0, Math.Min(4096, text.Length));
            }

            message.Data = GetData(request).Where(_ => _.Value != null).ToDictionary(_ => _.Key, _ => _.Value);

            return message;
        }

        private static IList<RaygunRequestMessage.Cookie> GetCookies(RequestCookieCollection cookies)
        {
            // ToDo: filter cookies
            return cookies
                .Select(c => new RaygunRequestMessage.Cookie(c.Key, c.Value))
                .ToList();
        }

        private static IEnumerable<KeyValuePair<string, object>> GetData(OwinRequest request)
        {
            #region OWIN v1.0.0 - 3.2.3. Other Data

            yield return new KeyValuePair<string, object>(OwinConstants.OwinVersion, request.Get<string>(OwinConstants.OwinVersion));

            #endregion

            #region OWIN v1.0.0 - 3.2.1. Request Data

            yield return new KeyValuePair<string, object>(OwinConstants.RequestProtocol, request.Get<string>(OwinConstants.RequestProtocol));

            #endregion

            #region OWIN Key Guidelines and Common Keys - 6. Common Keys

            foreach (var pair in request.Get<IDictionary<string, object>>(OwinConstants.CommonKeys.Capabilities))
            {
                yield return pair;
            }

            #endregion

            yield return new KeyValuePair<string, object>("host.AppName", request.Get<string>("host.AppName"));
            yield return new KeyValuePair<string, object>("host.AppMode", request.Get<string>("host.AppMode"));

            yield return new KeyValuePair<string, object>("integratedpipeline.CurrentStage", request.Get<string>("integratedpipeline.CurrentStage"));
        }
    }
}
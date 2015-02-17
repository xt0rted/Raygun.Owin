using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
        private const string ContentLengthHeaderName = "Content-Length";
        internal static int? GetContentLength(this IOwinRequest request)
        {
            Contract.Assert(request != null);

            IHeaderDictionary headers = request.Headers;

            if (headers == null)
            {
                return null;
            }

            string[] values;

            if (!headers.TryGetValue(ContentLengthHeaderName, out values))
            {
                return null;
            }

            if (values == null || values.Length != 1)
            {
                return null;
            }

            string value = values[0];

            if (value == null)
            {
                return null;
            }

            int parsed;

            if (!Int32.TryParse(value, out parsed))
            {
                return null;
            }

            if (parsed < 0)
            {
                return null;
            }

            return parsed;
        }

        internal static async Task<Stream> CreateBufferedRequestBodyAsync(this IOwinRequest owinRequest)
        {
            // We need to replace the request body with a buffered stream so that other components can read the stream.
            // For this stream to be useful, it must NOT be diposed along with the request. Streams created by
            // StreamContent do get disposed along with the request, so use MemoryStream to buffer separately.
            MemoryStream buffer;
            int? contentLength = owinRequest.GetContentLength();

            if (!contentLength.HasValue)
            {
                buffer = new MemoryStream();
            }
            else
            {
                buffer = new MemoryStream(contentLength.Value);
            }

            //cancellationToken.ThrowIfCancellationRequested();

            using (var copier = new StreamContent(owinRequest.Body))
            {
                await copier.CopyToAsync(buffer);
            }
            //await owinRequest.Body.CopyToAsync(buffer);

            // Provide the non-disposing, buffered stream to later OWIN components (set to the stream's beginning).
            buffer.Position = 0;
            owinRequest.Body = buffer;

            // For MemoryStream, Length is guaranteed to be an int.
            return new MemoryStream(buffer.GetBuffer(), 0, (int)buffer.Length);
        }

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

            var capabilities = request.Get<IDictionary<string, object>>(OwinConstants.CommonKeys.Capabilities);
            if (capabilities != null)
            {
                foreach (var capability in capabilities)
                {
                    yield return capability;
                }
            }

            #endregion

            yield return new KeyValuePair<string, object>("host.AppName", request.Get<string>("host.AppName"));
            yield return new KeyValuePair<string, object>("host.AppMode", request.Get<string>("host.AppMode"));

            yield return new KeyValuePair<string, object>("integratedpipeline.CurrentStage", request.Get<string>("integratedpipeline.CurrentStage"));
        }
    }
}
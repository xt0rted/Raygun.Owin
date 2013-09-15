using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Raygun.Owin
{
    using OwinEnvironment = IDictionary<string, object>;

    // This class is based on code from Microsoft.Owin.OwinRequest
    internal class OwinRequest
    {
        public OwinRequest(OwinEnvironment environment)
        {
            Environment = environment;
        }

        private OwinEnvironment Environment { get; set; }

        public Stream Body
        {
            get
            {
                return Get<Stream>(OwinConstants.RequestBody);
            }
        }

        public string ContentType
        {
            get
            {
                return GetHeader(Headers, Constants.Headers.ContentType);
            }
        }

        public IDictionary<string, string[]> Headers
        {
            get
            {
                return Get<IDictionary<string, string[]>>(OwinConstants.RequestHeaders);
            }
        }

        public string Host
        {
            get
            {
                return GetHost(this);
            }
        }

        public string LocalIpAddress
        {
            get
            {
                return Get<string>(OwinConstants.CommonKeys.LocalIpAddress);
            }
        }

        public string LocalPort
        {
            get
            {
                return Get<string>(OwinConstants.CommonKeys.LocalPort);
            }
        }

        public string Method
        {
            get
            {
                return Get<string>(OwinConstants.RequestMethod);
            }
        }

        public string Path
        {
            get
            {
                return Get<string>(OwinConstants.RequestPath);
            }
        }

        public string PathBase
        {
            get
            {
                return Get<string>(OwinConstants.RequestPathBase);
            }
        }

        public IDictionary<string, string[]> Query
        {
            get
            {
                return GetQuery(Environment);
            }
        }

        public string QueryString
        {
            get
            {
                return Get<string>(OwinConstants.RequestQueryString);
            }
        }

        public string RemoteIpAddress
        {
            get
            {
                return Get<string>(OwinConstants.CommonKeys.RemoteIpAddress);
            }
        }

        public string Scheme
        {
            get
            {
                return Get<string>(OwinConstants.RequestScheme);
            }
        }

        public Uri Uri
        {
            get
            {
                var url = Scheme + Uri.SchemeDelimiter + Host + PathBase + Path;
                if (QueryString != string.Empty)
                {
                    url += "?" + QueryString.Replace("#", "%23");
                }

                return new Uri(url);
            }
        }

        public T Get<T>(string key)
        {
            return Environment.Get<T>(key);
        }

        private static string GetHeader(IDictionary<string, string[]> headers, string key)
        {
            string[] headersUnmodified = GetHeaderUnmodified(headers, key);
            if (headersUnmodified == null)
            {
                return null;
            }

            return string.Join(",", headersUnmodified);
        }

        private static string[] GetHeaderUnmodified(IDictionary<string, string[]> headers, string key)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            string[] results;

            if (!headers.TryGetValue(key, out results))
            {
                return null;
            }

            return results;
        }

        private static string GetHost(OwinRequest request)
        {
            var headers = request.Headers;

            string host = GetHeader(headers, Constants.Headers.Host);
            if (!string.IsNullOrWhiteSpace(host))
            {
                return host;
            }

            var localIpAddress = request.LocalIpAddress ?? "localhost";
            var localPort = request.LocalPort;

            if (string.IsNullOrWhiteSpace(localPort))
            {
                return localIpAddress;
            }

            return localIpAddress + ":" + localPort;
        }

        private static IDictionary<string, string[]> GetQuery(OwinEnvironment environment)
        {
            // If we're running alongside Microsoft.Owin then we'll first check to see if this has already been parsed.
            // We can add more checks as other libraries become available.
            var query = environment.Get<IDictionary<string, string[]>>("Microsoft.Owin.Query#dictionary");
            if (query != null)
            {
                return query;
            }

            string text = environment.Get<string>(OwinConstants.RequestQueryString);
            var accumulator = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            ParseDelimited(text, new[] { '&', ';' }, (hostName, value, state) =>
            {
                var dictionary = (IDictionary<string, List<string>>)state;

                List<string> existing;
                if (!dictionary.TryGetValue(hostName, out existing))
                {
                    dictionary.Add(hostName, new List<string>(1) { value });
                }
                else
                {
                    existing.Add(value);
                }
            }, accumulator);

            return accumulator.ToDictionary(_ => _.Key,
                                            _ => _.Value.ToArray());
        }

        private static void ParseDelimited(string text, char[] delimiters, Action<string, string, object> callback, object state)
        {
            int textLength = text.Length;
            int equalIndex = text.IndexOf('=');
            if (equalIndex == -1)
            {
                equalIndex = textLength;
            }

            int scanIndex = 0;
            while (scanIndex < textLength)
            {
                int delimiterIndex = text.IndexOfAny(delimiters, scanIndex);
                if (delimiterIndex == -1)
                {
                    delimiterIndex = textLength;
                }

                if (equalIndex < delimiterIndex)
                {
                    while (scanIndex != equalIndex && char.IsWhiteSpace(text[scanIndex]))
                    {
                        ++scanIndex;
                    }

                    string name = text.Substring(scanIndex, equalIndex - scanIndex);
                    string value = text.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);

                    callback(
                        Uri.UnescapeDataString(name.Replace('+', ' ')),
                        Uri.UnescapeDataString(value.Replace('+', ' ')),
                        state);

                    equalIndex = text.IndexOf('=', delimiterIndex);
                    if (equalIndex == -1)
                    {
                        equalIndex = textLength;
                    }
                }

                scanIndex = delimiterIndex + 1;
            }
        }
    }
}
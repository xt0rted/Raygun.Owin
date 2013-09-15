using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Raygun.Owin.Tests
{
    [TestFixture]
    public class OwinRequestTests
    {
        [TestCase(null, null, "test.me", ExpectedResult = "test.me")]
        [TestCase(null, "1326", "test.me", ExpectedResult = "test.me")]
        [TestCase("1.2.3.4", null, "test.me", ExpectedResult = "test.me")]
        [TestCase("1.2.3.4", null, null, ExpectedResult = "1.2.3.4")]
        [TestCase("1.2.3.4", "1326", null, ExpectedResult = "1.2.3.4:1326")]
        public string Verify_Host(string host, string port, string hostHeader)
        {
            var headers = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            if (hostHeader != null)
            {
                headers.Add("Host", new[] { hostHeader });
            }

            var env = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            env.Add("server.LocalIpAddress", host);
            env.Add("server.LocalPort", port);
            env.Add("owin.RequestHeaders", headers);

            var request = new OwinRequest(env);

            return request.Host;
        }

        [TestCase("", "https://test.me/api/users")]
        [TestCase("id=13&filter=f1&filter=f2", "https://test.me/api/users?id=13&filter=f1&filter=f2")]
        public void Verify_Uri(string queryString, string result)
        {
            var headers = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            headers.Add("Host", new[] { "test.me" });

            var env = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            env.Add("owin.RequestHeaders", headers);
            env.Add("owin.RequestScheme", "https");
            env.Add("owin.RequestPathBase", string.Empty);
            env.Add("owin.RequestPath", "/api/users");
            env.Add("owin.RequestQueryString", queryString);

            var request = new OwinRequest(env);

            Assert.AreEqual(new Uri(result), request.Uri);
        }
    }
}
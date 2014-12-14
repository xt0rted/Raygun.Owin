namespace Raygun
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using Raygun.Messages;

    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public class RaygunSettings
    {
        public RaygunSettings()
        {
            ApiEndpoint = ConfigurationManager.AppSettings["raygun:apiEndpoint"] ?? Constants.RaygunApiEndpoint;
            ApiKey = ConfigurationManager.AppSettings["raygun:apiKey"];
            ApplicationVersion = LoadApplicationVersion(ConfigurationManager.AppSettings["raygun:applicationVersion"]);
            Tags = LoadTags(ConfigurationManager.AppSettings["raygun:tags"]);
        }

        public string ApiEndpoint { get; set; }
        public string ApiKey { get; set; }
        public string ApplicationVersion { get; set; }
        public Func<OwinEnvironment, RaygunIdentifierMessage> LoadUserDetails { get; set; }
        public Action<RaygunMessage> MessageInspector { get; set; }
        public IList<string> Tags { get; set; }
        public bool ThrowOnError { get; set; }

        private static string LoadApplicationVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return null;
            }

            return version.Trim();
        }

        private static IList<string> LoadTags(string tags)
        {
            tags = tags ?? string.Empty;

            return tags.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(t => t.Trim())
                       .Where(t => t != string.Empty)
                       .ToList();
        }
    }
}
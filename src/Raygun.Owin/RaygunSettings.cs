namespace Raygun
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using Raygun.Messages;

    public class RaygunSettings
    {
        public RaygunSettings()
        {
            ApiEndpoint = ConfigurationManager.AppSettings["raygun:apiEndpoint"] ?? Constants.RaygunApiEndpoint;
            ApiKey = ConfigurationManager.AppSettings["raygun:apiKey"];
            Tags = LoadTags(ConfigurationManager.AppSettings["raygun:tags"]);
        }

        public string ApiEndpoint { get; set; }
        public string ApiKey { get; set; }
        public Action<RaygunMessage> MessageInspector { get; set; }
        public IList<string> Tags { get; set; }
        public bool ThrowOnError { get; set; }

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
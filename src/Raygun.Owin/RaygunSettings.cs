namespace Raygun
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    using Raygun.Messages;

    public class RaygunSettings
    {
        public RaygunSettings()
        {
            ApiEndpoint = ConfigurationManager.AppSettings["raygun:apiEndpoint"] ?? Constants.RaygunApiEndpoint;
            ApiKey = ConfigurationManager.AppSettings["raygun:apiKey"];
            Tags = new List<string>();
        }

        public string ApiEndpoint { get; set; }
        public string ApiKey { get; set; }
        public Action<RaygunMessage> MessageInspector { get; set; }
        public IList<string> Tags { get; set; }
        public bool ThrowOnError { get; set; }
    }
}
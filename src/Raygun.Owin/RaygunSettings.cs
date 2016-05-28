namespace Raygun
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Raygun.Messages;

    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public partial class RaygunSettings
    {
        public static RaygunSettings Settings { get; } = LoadFromAppSettings();

        public string ApiEndpoint { get; set; }

        public string ApiKey { get; set; }

        public string ApplicationVersion { get; set; }

        public Func<OwinEnvironment, RaygunIdentifierMessage> LoadUserDetails { get; set; }

        public Action<RaygunMessage> MessageInspector { get; set; }

        public IList<string> Tags { get; set; }

        public bool ThrowOnError { get; set; }

        public bool ExcludeErrorsFromLocal { get; set; }
    }
}

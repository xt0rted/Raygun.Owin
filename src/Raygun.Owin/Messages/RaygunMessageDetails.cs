namespace Raygun.Messages
{
    using System.Collections.Generic;

    public class RaygunMessageDetails
    {
        public RaygunMessageDetails()
        {
            Tags = new List<string>();
        }

        public string MachineName { get; set; }

        public string Version { get; set; }

        public RaygunErrorMessage Error { get; set; }

        public RaygunEnvironmentMessage Environment { get; set; }

        public RaygunClientMessage Client { get; set; }

        public IList<string> Tags { get; set; }

        public IDictionary<string, object> UserCustomData { get; set; }

        public RaygunIdentifierMessage User { get; set; }

        public RaygunRequestMessage Request { get; set; }

        public RaygunResponseMessage Response { get; set; }
    }
}
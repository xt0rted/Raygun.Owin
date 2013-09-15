using System.Collections;
using System.Collections.Generic;

namespace Raygun.Messages
{
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
        public IDictionary UserCustomData { get; set; }
        public RaygunRequestMessage Request { get; set; }
    }
}
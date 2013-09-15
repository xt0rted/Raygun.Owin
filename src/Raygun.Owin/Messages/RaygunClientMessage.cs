using System.Reflection;

namespace Raygun.Messages
{
    public class RaygunClientMessage
    {
        public RaygunClientMessage()
        {
            Name = "Raygun.Owin";
            Version = Assembly.GetAssembly(typeof (RaygunClient))
                              .GetName()
                              .Version.ToString();
            ClientUrl = "https://github.com/xt0rted/Raygun.Owin";
        }

        public string Name { get; set; }
        public string Version { get; set; }
        public string ClientUrl { get; set; }
    }
}
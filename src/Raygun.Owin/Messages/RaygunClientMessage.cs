namespace Raygun.Messages
{
    public class RaygunClientMessage
    {
        public RaygunClientMessage()
        {
            Name = RaygunClient.ClientName;
            Version = RaygunClient.ClientVersion;
            ClientUrl = "https://github.com/xt0rted/Raygun.Owin";
        }

        public string Name { get; set; }

        public string Version { get; set; }

        public string ClientUrl { get; set; }

        public override string ToString()
        {
            return string.Format("[RaygunClientMessage: Name={0}, Version={1}, ClientUrl={2}], Name, Version, ClientUrl");
        }
    }
}
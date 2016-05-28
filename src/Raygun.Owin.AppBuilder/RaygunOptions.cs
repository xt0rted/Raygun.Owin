namespace Raygun.Owin
{
    public class RaygunOptions
    {
        public RaygunOptions()
        {
            Settings = RaygunSettings.Settings;
        }

        public bool LogUnhandledExceptions { get; set; }

        public bool LogUnhandledRequests { get; set; }

        public RaygunSettings Settings { get; set; }
    }
}
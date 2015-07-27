namespace Raygun.Owin.Samples.SelfHosted
{
    using System.Reflection;
    using System.Security.Claims;

    using Microsoft.Owin;

    using Nancy;

    using Raygun.Messages;

    public partial class Startup
    {
        private static string ApplicationVersion()
        {
            var application = Assembly.GetExecutingAssembly();

            return application.GetName().Version.ToString();
        }

        protected override RaygunSettings SetupSettings()
        {
            var settings = base.SetupSettings();

            settings.ApplicationVersion = ApplicationVersion();
            settings.LoadUserDetails = environment =>
            {
                if (environment == null) return null;

                var request = new OwinRequest(environment);

                return GetRaygunIdentifierForNancyContext(request) ??
                       GetRaygunIdentifierForClaimsPrincipal(request);
            };

#if DEBUG
            settings.Tags.Add("debug");
#endif

            return settings;
        }

        private RaygunIdentifierMessage GetRaygunIdentifierForNancyContext(OwinRequest request)
        {
            var context = request.Get<NancyContext>("nancy.NancyContext");
            if (context == null || context.CurrentUser == null)
            {
                return null;
            }

            var user = context.CurrentUser as UserIdentity;
            if (user == null)
            {
                return null;
            }

            var userId = user.UserId.ToString("D");
            return new RaygunIdentifierMessage(user.UserName)
            {
                Email = string.Format("{0}@raygunio.test", user.UserName),
                FirstName = "robbie",
                FullName = "robbie the robot",
                IsAnonymous = false,
                UUID = userId
            };
        }

        private RaygunIdentifierMessage GetRaygunIdentifierForClaimsPrincipal(OwinRequest request)
        {
            var result = new RaygunIdentifierMessage(null)
            {
                IsAnonymous = true
            };

            var principal = request.User as ClaimsPrincipal;
            if (principal != null && principal.Identity != null)
            {
                result.Identifier = principal.Identity.Name;

                if (principal.Identity.IsAuthenticated)
                {
                    result.IsAnonymous = false;
                }
            }

            return result;
        }
    }
}
[assembly: Microsoft.Owin.OwinStartup(typeof(Raygun.Owin.Samples.NancyFX.Startup))]

namespace Raygun.Owin.Samples.NancyFX
{
    using System;
    using System.Reflection;
    using System.Security.Claims;

    using global::Owin;

    using Microsoft.Owin;

    using Nancy;
    using Nancy.Extensions;
    using Nancy.Owin;

    using Raygun.Messages;

    public class Startup : RaygunStartup
    {
        private readonly Lazy<string> _applicationVersion = new Lazy<string>(() =>
        {
            var application = Assembly.GetExecutingAssembly();

            return application.GetName().Version.ToString();
        });

        protected override string ApplicationVersion()
        {
            return _applicationVersion.Value;
        }

        protected override RaygunSettings SetupSettings()
        {
            var settings = base.SetupSettings();

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

        public override void SetupConfiguration(IAppBuilder app, RaygunSettings settings)
        {
            app.UseRaygun(options =>
            {
                options.LogUnhandledExceptions = true;
                options.LogUnhandledRequests = true;
                options.Settings = settings;
            });

            app.Use((context, next) =>
            {
                if (context.Request.Path.StartsWithSegments(new PathString("/exception")))
                {
                    throw new NotImplementedException("Route: /exception");
                }

                return next();
            });

            app.UseNancy(options =>
            {
                options.PerformPassThrough = context =>
                {
                    if (context.Response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        Exception exception;
                        if (context.TryGetException(out exception))
                        {
                            if (exception is RequestExecutionException)
                            {
                                exception = exception.InnerException;
                            }

                            new RaygunClient(settings).SendInBackground(context.GetOwinEnvironment(), exception);
                        }
                    }

                    return false;
                };
            });
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
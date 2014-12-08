[assembly: Microsoft.Owin.OwinStartup(typeof(Raygun.Owin.Samples.NancyFX.Startup))]

namespace Raygun.Owin.Samples.NancyFX
{
    using System;

    using global::Owin;

    using Microsoft.Owin;

    using Nancy;
    using Nancy.Extensions;
    using Nancy.Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var settings = new RaygunSettings();

#if DEBUG
            settings.Tags.Add("debug");
#endif

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
    }
}
[assembly: Microsoft.Owin.OwinStartup(typeof(Raygun.Owin.Samples.NancyFX.Startup))]

namespace Raygun.Owin.Samples.NancyFX
{
    using System;

    using global::Owin;

    using Microsoft.Owin;
    using Microsoft.Owin.Extensions;

    public partial class Startup : RaygunStartup
    {
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

            app.UseNancy();
            app.UseStageMarker(PipelineStage.MapHandler);
        }
    }
}
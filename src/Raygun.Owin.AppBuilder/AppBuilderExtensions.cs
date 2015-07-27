namespace Raygun.Owin
{
    using System;
    using System.Collections.Generic;

    using global::Owin;

    using Raygun.LibOwin;

    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRaygun(this IAppBuilder builder, Action<RaygunOptions> configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            var options = new RaygunOptions();

            configuration(options);

            return UseRaygun(builder, options);
        }

        public static IAppBuilder UseRaygun(this IAppBuilder builder, RaygunOptions options = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            var raygunOptions = options ?? new RaygunOptions();

            if (raygunOptions.LogUnhandledExceptions)
            {
                builder.Use(typeof (RaygunUnhandledExceptionMiddleware), raygunOptions.Settings, options.PreventWrappingRequestBody);
            }

            if (raygunOptions.LogUnhandledRequests)
            {
                builder.Use(typeof (RaygunUnhandledRequestMiddleware), raygunOptions.Settings, options.PreventWrappingRequestBody);
            }

            SetRaygunCapabilities(builder.Properties);

            return builder;
        }

        private static void SetRaygunCapabilities(IDictionary<string, object> properties)
        {
            object obj;
            if (properties.TryGetValue(OwinConstants.CommonKeys.Capabilities, out obj))
            {
                var capabilities = (IDictionary<string, object>) obj;
                capabilities["raygun.Version"] = RaygunClient.ClientVersion;
            }
        }
    }
}
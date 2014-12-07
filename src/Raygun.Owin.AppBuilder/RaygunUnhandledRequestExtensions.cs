namespace Raygun.Owin
{
    using System;

    using global::Owin;

    public static class RaygunUnhandledRequestExtensions
    {
        public static IAppBuilder UseRaygunUnhandledRequestLogger(this IAppBuilder builder)
        {
            return UseRaygunUnhandledRequestLogger(builder, new RaygunSettings());
        }

        public static IAppBuilder UseRaygunUnhandledRequestLogger(this IAppBuilder builder, RaygunSettings settings)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            return builder.Use(typeof(RaygunUnhandledRequestMiddleware), settings);
        }
    }
}
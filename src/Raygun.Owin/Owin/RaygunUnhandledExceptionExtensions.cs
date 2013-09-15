using System;

using Owin;

namespace Raygun.Owin
{
    public static class RaygunUnhandledExceptionExtensions
    {
        public static IAppBuilder UseRaygunUnhandledExceptionLogger(this IAppBuilder builder)
        {
            return UseRaygunUnhandledExceptionLogger(builder, new RaygunSettings());
        }

        public static IAppBuilder UseRaygunUnhandledExceptionLogger(this IAppBuilder builder, RaygunSettings settings)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            return builder.Use(typeof(RaygunUnhandledExceptionMiddleware), settings);
        }
    }
}
namespace Raygun
{
    using System;

    using Raygun.Owin;

    internal static class OwinContextHelpers
    {
        public static IOwinContext WithResponseStatus(this IOwinContext context, int code, string phrase)
        {
            context.Response.StatusCode = code;
            context.Response.ReasonPhrase = phrase;

            return context;
        }

        public static IOwinContext WithUri(this IOwinContext context, Uri uri)
        {
            context.Request.Scheme = uri.Scheme;
            context.Request.Host = new HostString(uri.Host);
            context.Request.Path = new PathString(uri.AbsolutePath);
            context.Request.QueryString = new QueryString(uri.Query);

            return context;
        }
    }
}
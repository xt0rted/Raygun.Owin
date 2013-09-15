namespace Raygun
{
    internal static class OwinConstants
    {
        /// <summary>
        /// owin.RequestBody
        /// </summary>
        internal const string RequestBody = "owin.RequestBody";

        /// <summary>
        /// owin.RequestHeaders
        /// </summary>
        internal const string RequestHeaders = "owin.RequestHeaders";

        /// <summary>
        /// owin.RequestMethod
        /// </summary>
        internal const string RequestMethod = "owin.RequestMethod";

        /// <summary>
        /// owin.RequestPath
        /// </summary>
        internal const string RequestPath = "owin.RequestPath";

        /// <summary>
        /// owin.RequestPathBase
        /// </summary>
        internal const string RequestPathBase = "owin.RequestPathBase";

        /// <summary>
        /// owin.RequestQueryString
        /// </summary>
        internal const string RequestQueryString = "owin.RequestQueryString";

        /// <summary>
        /// owin.RequestScheme
        /// </summary>
        internal const string RequestScheme = "owin.RequestScheme";

        internal static class CommonKeys
        {
            /// <summary>
            /// server.LocalIpAddress
            /// </summary>
            internal const string LocalIpAddress = "server.LocalIpAddress";

            /// <summary>
            /// server.LocalPort
            /// </summary>
            internal const string LocalPort = "server.LocalPort";

            /// <summary>
            /// server.RemoteIpAddress
            /// </summary>
            internal const string RemoteIpAddress = "server.RemoteIpAddress";
        }

        internal static class RaygunKeys
        {
            /// <summary>
            /// webapi.exception
            /// </summary>
            internal const string WebApiExceptionKey = "webapi.exception";
        }
    }
}
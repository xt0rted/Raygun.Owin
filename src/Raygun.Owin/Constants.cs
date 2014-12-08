namespace Raygun
{
    internal static class Constants
    {
        internal static class Headers
        {
            /// <summary>
            /// X-ApiKey
            /// </summary>
            internal const string ApiKey = "X-ApiKey";
        }

        internal static class RaygunKeys
        {
            /// <summary>
            /// webapi.exception
            /// </summary>
            internal const string WebApiExceptionKey = "webapi.exception";
        }

        internal const string RaygunApiEndpoint = "https://api.raygun.io/entries";
    }
}
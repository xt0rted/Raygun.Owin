namespace Raygun
{
    using System;
    using System.Threading.Tasks;

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

        internal static readonly Task CompletedTask = CreateCompletedTask();

        private static Task CreateCompletedTask()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        public static Task FromError(Exception exception)
        {
            return FromError<AsyncVoid>(exception);
        }

        public static Task<TResult> FromError<TResult>(Exception exception)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);
            return tcs.Task;
        }

        private struct AsyncVoid
        {
        }
    }
}
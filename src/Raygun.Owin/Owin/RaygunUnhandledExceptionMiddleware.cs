namespace Raygun.Owin
{
    using System;
    using System.Threading.Tasks;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public class RaygunUnhandledExceptionMiddleware
    {
        private readonly AppFunc _next;
        private readonly RaygunClient _client;

        public RaygunUnhandledExceptionMiddleware(AppFunc next, RaygunSettings settings)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            _next = next;

            if (!string.IsNullOrEmpty(settings.ApiKey))
            {
                _client = new RaygunClient(settings);
            }
        }

        public Task Invoke(OwinEnvironment environment)
        {
            if (_client == null)
            {
                return _next(environment);
            }

            return _next(environment).ContinueWith(appTask =>
            {
                if (appTask.IsFaulted && appTask.Exception != null)
                {
                    foreach (var innerException in appTask.Exception.InnerExceptions)
                    {
                        HandleException(environment, innerException);
                    }

                    throw appTask.Exception;
                }

                var exception = environment.Get<Exception>(Constants.RaygunKeys.WebApiExceptionKey);
                if (exception != null)
                {
                    HandleException(environment, exception);

                    throw exception;
                }

                return appTask;
            });
        }

        private void HandleException(OwinEnvironment environment, Exception exception)
        {
            _client.SendInBackground(environment, exception);
        }
    }
}
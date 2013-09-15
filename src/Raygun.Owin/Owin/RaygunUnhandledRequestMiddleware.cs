using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raygun.Owin
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using OwinEnvironment = IDictionary<string, object>;

    public class RaygunUnhandledRequestMiddleware
    {
        private readonly AppFunc _next;
        private readonly RaygunClient _client;

        public RaygunUnhandledRequestMiddleware(AppFunc next, RaygunSettings settings)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
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
                return _next.Invoke(environment);
            }

            var exception = new UnhandledRequestException();

            return HandleExceptionWrapper(environment, exception);
        }

        private Task HandleException(OwinEnvironment environment, Exception exception)
        {
            _client.SendInBackground(environment, exception);

            return Constants.CompletedTask;
        }

        private Task HandleExceptionWrapper(OwinEnvironment environment, Exception exception)
        {
            try
            {
                return HandleException(environment, exception);
            }
            catch
            {
                return Constants.FromError(exception);
            }
        }
    }
}
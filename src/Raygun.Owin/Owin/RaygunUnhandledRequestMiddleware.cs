namespace Raygun.Owin
{
    using System;
    using System.Threading.Tasks;

    using Raygun.LibOwin;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

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

        public async Task Invoke(OwinEnvironment environment)
        {
            await _next(environment);

            if (_client == null)
            {
                return;
            }

            var responseCode = environment.Get<int>(OwinConstants.ResponseStatusCode);
            if (responseCode == 404)
            {
                var requestUrl = environment.Get<string>(OwinConstants.RequestPath);

                _client.SendInBackground(environment, new UnhandledRequestException(requestUrl));
            }
        }
    }
}
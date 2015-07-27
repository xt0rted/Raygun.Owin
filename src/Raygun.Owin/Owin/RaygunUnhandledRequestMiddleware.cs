namespace Raygun.Owin
{
    using System.Threading.Tasks;

    using Raygun.LibOwin;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public class RaygunUnhandledRequestMiddleware : BaseRaygunMiddleware
    {
        public RaygunUnhandledRequestMiddleware(AppFunc next, RaygunSettings settings, bool preventWrappingRequestBody = false)
            : base(next, settings, preventWrappingRequestBody)
        {
        }

        public async Task Invoke(OwinEnvironment environment)
        {
            WrapRequestBody(environment);

            await Next(environment);

            if (Client != null)
            {
                var request = new OwinRequest(environment);
                var response = new OwinResponse(environment);

                if (response.StatusCode == 404)
                {
                    var requestUrl = request.Path;

                    Client.SendInBackground(environment, new UnhandledRequestException(requestUrl.Value));
                }
            }
        }
    }
}
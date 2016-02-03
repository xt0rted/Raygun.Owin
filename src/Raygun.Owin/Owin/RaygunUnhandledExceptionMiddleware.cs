namespace Raygun.Owin
{
    using System;
    using System.Threading.Tasks;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public class RaygunUnhandledExceptionMiddleware : BaseRaygunMiddleware
    {
        public RaygunUnhandledExceptionMiddleware(AppFunc next, RaygunSettings settings, bool preventWrappingRequestBody = false)
            : base(next, settings, preventWrappingRequestBody)
        {
        }

        public async Task Invoke(OwinEnvironment environment)
        {
            WrapRequestBody(environment);

            try
            {
                await Next(environment);
            }
            catch (AggregateException ex)
            {
                if (Client != null)
                {
                    foreach (var innerException in ex.InnerExceptions)
                    {
                        HandleException(environment, innerException);
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                if (Client != null)
                {
                    HandleException(environment, ex);
                }

                throw;
            }
        }

        private void HandleException(OwinEnvironment environment, Exception exception)
        {
            Client.SendInBackground(environment, exception);
        }
    }
}
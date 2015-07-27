namespace Raygun.Owin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Raygun.LibOwin;

    public abstract class BaseRaygunMiddleware
    {
        protected readonly Func<IDictionary<string, object>, Task> Next;
        protected readonly RaygunClient Client;

        private readonly bool _preventWrappingRequestBody;

        protected BaseRaygunMiddleware(Func<IDictionary<string, object>, Task> next, RaygunSettings settings, bool preventWrappingRequestBody = false)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            Next = next;
            _preventWrappingRequestBody = preventWrappingRequestBody;

            if (!string.IsNullOrEmpty(settings.ApiKey))
            {
                Client = new RaygunClient(settings);
            }
        }

        protected void WrapRequestBody(IDictionary<string, object> environment)
        {
            if (Client != null && !_preventWrappingRequestBody)
            {
                if (environment.Get<Stream>(OwinConstants.RequestBody).GetType() != typeof (SeekableStreamWrapper))
                {
                    environment[OwinConstants.RequestBody] = new SeekableStreamWrapper(environment.Get<Stream>(OwinConstants.RequestBody));
                }
            }
        }
    }
}
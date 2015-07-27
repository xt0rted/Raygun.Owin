namespace Raygun.Owin.Samples.SelfHosted
{
    using System;
    using System.Runtime.ExceptionServices;

    using Nancy;
    using Nancy.ErrorHandling;

    // http://dhickey.ie/2014/02/bubbling-exceptions-in-nancy-up-the-owin-pipeline/
    public class RethrowStatusCodeHandler : IStatusCodeHandler
    {
        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            if (!context.Items.ContainsKey(NancyEngine.ERROR_EXCEPTION))
            {
                return false;
            }

            var exception = context.Items[NancyEngine.ERROR_EXCEPTION] as Exception;

            return statusCode == HttpStatusCode.InternalServerError && exception != null;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var innerException = ((Exception) context.Items[NancyEngine.ERROR_EXCEPTION]).InnerException;

            ExceptionDispatchInfo.Capture(innerException)
                                 .Throw();
        }
    }
}
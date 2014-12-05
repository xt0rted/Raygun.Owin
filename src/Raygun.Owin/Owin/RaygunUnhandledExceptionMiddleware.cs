using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raygun.Owin
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using OwinEnvironment = IDictionary<string, object>;

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

            try
            {
                return _next.Invoke(environment)
                           .ContinueWith(appTask =>
                           {
                               if (appTask.IsFaulted)
                               {
                                   var errorLoggingErrors = appTask.Exception.InnerExceptions
                                                                   .Select(innerException =>
                                                                   {
                                                                       try
                                                                       {
                                                                           HandleException(environment, innerException);
                                                                       }
                                                                       catch
                                                                       {
                                                                           return true;
                                                                       }

                                                                       return false;
                                                                   })
                                                                   .Any();

                                   if (errorLoggingErrors)
                                   {
                                       return Constants.FromError(appTask.Exception);
                                   }

                                   return Constants.CompletedTask;
                               }

                               var exception = environment.Get<Exception>(Constants.RaygunKeys.WebApiExceptionKey);
                               if (exception != null)
                               {
                                   return HandleExceptionWrapper(environment, exception);
                               }

                               return Constants.CompletedTask;
                           });
            }
            catch (Exception ex)
            {
                try
                {
                    HandleException(environment, ex);
                    return Constants.CompletedTask;
                }
                catch
                {
                }

                throw;
            }
        }

        private void HandleException(OwinEnvironment environment, Exception exception)
        {
            _client.SendInBackground(environment, exception);
        }

        private Task HandleExceptionWrapper(OwinEnvironment environment, Exception exception)
        {
            try
            {
                HandleException(environment, exception);
                return Constants.CompletedTask;
            }
            catch
            {
                return Constants.FromError(exception);
            }
        }
    }
}
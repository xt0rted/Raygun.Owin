namespace Raygun
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    using Raygun.Messages;

    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public class RaygunClient : RaygunClientBase
    {
        private static readonly Lazy<string> ClientNameLoader = new Lazy<string>(() => typeof (RaygunClient).Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title);
        private static readonly Lazy<string> ClientVersionLoader = new Lazy<string>(() => typeof (RaygunClient).Assembly.GetName().Version.ToString());
        private static readonly List<Type> WrapperExceptions = new List<Type>();

        private readonly RaygunSettings _settings;

        static RaygunClient()
        {
            AddWrapperExceptions(typeof (TargetInvocationException));

            try
            {
                // Since we don't reference the System.Web assembly we need to try to lazily load this reference
                AddWrapperExceptions(Assembly.ReflectionOnlyLoad("System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").GetType("System.Web.HttpUnhandledException"));
            }
            catch
            {
            }

            AddIgnoredExceptionFilters(e =>
            {
                if (e.GetType().FullName == "System.Web.HttpException")
                {
                    var errorCode = (int) ReflectionHelpers.GetPropertyValue(e, "ErrorCode");
                    if (errorCode == unchecked ((int) 0x80070057))
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        public RaygunClient()
            : this(new RaygunSettings())
        {
        }

        public RaygunClient(RaygunSettings settings)
        {
            _settings = settings;
        }

        internal static string ClientName
        {
            get { return ClientNameLoader.Value; }
        }

        internal static string ClientVersion
        {
            get { return ClientVersionLoader.Value; }
        }

        public static void AddWrapperExceptions(params Type[] wrapperExceptions)
        {
            foreach (var type in wrapperExceptions)
            {
                if (!WrapperExceptions.Contains(type))
                {
                    WrapperExceptions.Add(type);
                }
            }
        }

        public static void RemoveWrapperExceptions(params Type[] wrapperExceptions)
        {
            foreach (var type in wrapperExceptions)
            {
                WrapperExceptions.Remove(type);
            }
        }

        public static void AddIgnoredExceptionFilters(params Func<Exception, bool>[] exceptionFilters)
        {
            foreach (var filter in exceptionFilters)
            {
                if (!IgnoredExceptionFilters.Contains(filter))
                {
                    IgnoredExceptionFilters.Add(filter);
                }
            }
        }

        public static void RemoveIgnoredExceptionFilters(params Func<Exception, bool>[] exceptionFilters)
        {
            foreach (var filter in exceptionFilters)
            {
                IgnoredExceptionFilters.Remove(filter);
            }
        }

        private Exception StripWrapperExceptions(Exception exception)
        {
            if (exception != null && WrapperExceptions.Any(wrapperException => exception.GetType() == wrapperException && exception.InnerException != null))
            {
                return StripWrapperExceptions(exception.InnerException);
            }

            return exception;
        }

        public RaygunMessage BuildMessage(OwinEnvironment environment, Exception exception, IList<string> tags = null, IDictionary<string, object> userCustomData = null)
        {
            exception = StripWrapperExceptions(exception);

            RaygunIdentifierMessage userInfo = null;
            if (_settings.LoadUserDetails != null)
            {
                userInfo = _settings.LoadUserDetails(environment);
            }

            var mergedTags = _settings.Tags
                                      .Union(tags ?? Enumerable.Empty<string>())
                                      .ToList();

            var message = RaygunMessageBuilder.New
                                              .SetHttpDetails(environment)
                                              .SetEnvironmentDetails()
                                              .SetMachineName(Environment.MachineName)
                                              .SetExceptionDetails(exception)
                                              .SetClientDetails()
                                              .SetVersion(_settings.ApplicationVersion)
                                              .SetTags(mergedTags)
                                              .SetUserCustomData(userCustomData)
                                              .SetUser(userInfo)
                                              .Build();
            return message;
        }

        public void Send(OwinEnvironment environment, Exception exception, IList<string> tags = null, IDictionary<string, object> userCustomData = null)
        {
            if (CanSend(exception))
            {
                var message = BuildMessage(environment, exception, tags, userCustomData);

                if (_settings.MessageInspector != null)
                {
                    _settings.MessageInspector(message);
                }

                Send(message);
                FlagAsSent(exception);
            }
        }

        public void SendInBackground(OwinEnvironment environment, Exception exception, IList<string> tags = null, IDictionary<string, object> userCustomData = null)
        {
            if (CanSend(exception))
            {
                var message = BuildMessage(environment, exception, tags, userCustomData);

                if (_settings.MessageInspector != null)
                {
                    _settings.MessageInspector(message);
                }

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Send(message);
                    FlagAsSent(exception);
                });
            }
        }

        protected void Send(RaygunMessage raygunMessage)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add(Constants.Headers.ApiKey, _settings.ApiKey);
                client.Encoding = Encoding.UTF8;

                try
                {
                    var message = SimpleJson.SerializeObject(raygunMessage);

                    client.UploadString(_settings.ApiEndpoint, message);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Error Logging Exception to Raygun.io {0}", ex.Message));

                    if (_settings.ThrowOnError)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
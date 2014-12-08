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

        private readonly RaygunSettings _settings;

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

        public RaygunMessage BuildMessage(OwinEnvironment environment, Exception exception, IList<string> tags = null)
        {
            var mergedTags = _settings.Tags
                                      .Union(tags ?? Enumerable.Empty<string>())
                                      .ToList();

            var message = RaygunMessageBuilder.New
                                              .SetHttpDetails(environment)
                                              .SetEnvironmentDetails()
                                              .SetMachineName(Environment.MachineName)
                                              .SetExceptionDetails(exception)
                                              .SetClientDetails()
                                              .SetVersion()
                                              .SetTags(mergedTags)
                                              .Build();
            return message;
        }

        public void SendInBackground(OwinEnvironment environment, Exception exception, IList<string> tags = null)
        {
            var message = BuildMessage(environment, exception, tags);

            if (_settings.MessageInspector != null)
            {
                _settings.MessageInspector(message);
            }

            Send(message);
            FlagAsSent(exception);
        }

        protected void Send(RaygunMessage raygunMessage)
        {
            ThreadPool.QueueUserWorkItem(_ =>
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
            });
        }
    }
}
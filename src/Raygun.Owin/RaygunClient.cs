using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

using Raygun.Messages;

namespace Raygun
{
    using OwinEnvironment = IDictionary<string, object>;

    public class RaygunClient
    {
        private readonly RaygunSettings _settings;

        public RaygunClient()
            : this(new RaygunSettings())
        {
        }

        public RaygunClient(RaygunSettings settings)
        {
            _settings = settings;
        }

        public RaygunMessage BuildMessage(OwinEnvironment environment, Exception exception)
        {
            var message = RaygunMessageBuilder.New
                                              .SetHttpDetails(environment)
                                              .SetEnvironmentDetails()
                                              .SetMachineName(Environment.MachineName)
                                              .SetExceptionDetails(exception)
                                              .SetClientDetails()
                                              .SetVersion()
                                              .Build();
            return message;
        }

        public void SendInBackground(OwinEnvironment environment, Exception exception)
        {
            var message = BuildMessage(environment, exception);

            if (_settings.Tags != null)
            {
                foreach (var tag in _settings.Tags)
                {
                    message.Details.Tags.Add(tag);
                }
            }

            if (_settings.MessageInspector != null)
            {
                _settings.MessageInspector(message);
            }

            Send(message);
        }

        public void Send(RaygunMessage raygunMessage)
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
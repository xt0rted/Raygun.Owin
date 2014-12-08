namespace Raygun
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Reflection;

    using Raygun.Builders;
    using Raygun.Messages;
    using Raygun.Owin;

    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public class RaygunMessageBuilder : IRaygunMessageBuilder
    {
        private readonly RaygunMessage _raygunMessage;

        private RaygunMessageBuilder()
        {
            _raygunMessage = new RaygunMessage();
        }

        public static IRaygunMessageBuilder New
        {
            get { return new RaygunMessageBuilder(); }
        }

        public RaygunMessage Build()
        {
            return _raygunMessage;
        }

        public IRaygunMessageBuilder SetClientDetails()
        {
            _raygunMessage.Details.Client = new RaygunClientMessage();

            return this;
        }

        public IRaygunMessageBuilder SetEnvironmentDetails()
        {
            _raygunMessage.Details.Environment = RaygunEnvironmentMessageBuilder.Build();

            return this;
        }

        public IRaygunMessageBuilder SetExceptionDetails(Exception exception)
        {
            if (exception != null)
            {
                _raygunMessage.Details.Error = RaygunErrorMessageBuilder.Build(exception);
            }

            if (exception != null && exception.GetType().FullName == "System.Web.HttpException")
            {
                var code = (int) ReflectionHelpers.RunInstanceMethod(exception, "GetHttpCode");
                string description = null;

                if (Enum.IsDefined(typeof (HttpStatusCode), code))
                {
                    description = ((HttpStatusCode) code).ToString();
                }

                _raygunMessage.Details.Response = new RaygunResponseMessage { StatusCode = code, StatusDescription = description };
            }

            var webError = exception as WebException;
            if (webError != null)
            {
                if (webError.Status == WebExceptionStatus.ProtocolError && webError.Response is HttpWebResponse)
                {
                    var response = (HttpWebResponse) webError.Response;
                    _raygunMessage.Details.Response = new RaygunResponseMessage { StatusCode = (int) response.StatusCode, StatusDescription = response.StatusDescription };
                }
                else if (webError.Status == WebExceptionStatus.ProtocolError && webError.Response is FtpWebResponse)
                {
                    var response = (FtpWebResponse) webError.Response;
                    _raygunMessage.Details.Response = new RaygunResponseMessage { StatusCode = (int) response.StatusCode, StatusDescription = response.StatusDescription };
                }
                else
                {
                    _raygunMessage.Details.Response = new RaygunResponseMessage { StatusDescription = webError.Status.ToString() };
                }
            }

            return this;
        }

        public IRaygunMessageBuilder SetHttpDetails(OwinEnvironment environment)
        {
            if (environment != null)
            {
                _raygunMessage.Details.Request = RaygunRequestMessageBuilder.Build(environment);

                if (_raygunMessage.Details.Response == null)
                {
                    _raygunMessage.Details.Response = new RaygunResponseMessage
                    {
                        StatusCode = environment.Get<int>(OwinConstants.ResponseStatusCode),
                        StatusDescription = environment.Get<string>(OwinConstants.ResponseReasonPhrase)
                    };
                }
            }

            return this;
        }

        public IRaygunMessageBuilder SetMachineName(string machineName)
        {
            _raygunMessage.Details.MachineName = machineName;

            return this;
        }

        public IRaygunMessageBuilder SetUserCustomData(IDictionary<string, object> userCustomData)
        {
            _raygunMessage.Details.UserCustomData = userCustomData;

            return this;
        }

        public IRaygunMessageBuilder SetVersion()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly != null)
            {
                _raygunMessage.Details.Version = entryAssembly.GetName()
                                                              .Version
                                                              .ToString();
            }
            else
            {
                _raygunMessage.Details.Version = "Not supplied";
            }

            return this;
        }
    }
}
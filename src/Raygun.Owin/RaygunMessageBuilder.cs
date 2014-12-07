namespace Raygun
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Raygun.Builders;
    using Raygun.Messages;

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
                _raygunMessage.Details.Error = new RaygunErrorMessage(exception);
            }

            return this;
        }

        public IRaygunMessageBuilder SetHttpDetails(OwinEnvironment environment)
        {
            if (environment != null)
            {
                _raygunMessage.Details.Request = RaygunRequestMessageBuilder.Build(environment);
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
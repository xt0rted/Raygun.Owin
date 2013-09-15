using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Raygun.Messages;

namespace Raygun
{
    using OwinEnvironment = IDictionary<string, object>;

    public class RaygunMessageBuilder
    {
        private readonly RaygunMessage _raygunMessage;

        private RaygunMessageBuilder()
        {
            _raygunMessage = new RaygunMessage();
        }

        public static RaygunMessageBuilder New
        {
            get
            {
                return new RaygunMessageBuilder();
            }
        }

        public RaygunMessage Build()
        {
            return _raygunMessage;
        }

        public RaygunMessageBuilder SetClientDetails()
        {
            _raygunMessage.Details.Client = new RaygunClientMessage();

            return this;
        }

        public RaygunMessageBuilder SetEnvironmentDetails()
        {
            _raygunMessage.Details.Environment = new RaygunEnvironmentMessage();

            return this;
        }

        public RaygunMessageBuilder SetExceptionDetails(Exception exception)
        {
            if (exception != null)
            {
                _raygunMessage.Details.Error = new RaygunErrorMessage(exception);
            }

            return this;
        }

        public RaygunMessageBuilder SetHttpDetails(OwinEnvironment environment)
        {
            if (environment != null)
            {
                _raygunMessage.Details.Request = new RaygunRequestMessage(environment);
            }

            return this;
        }

        public RaygunMessageBuilder SetMachineName(string machineName)
        {
            _raygunMessage.Details.MachineName = machineName;

            return this;
        }

        public RaygunMessageBuilder SetUserCustomData(IDictionary userCustomData)
        {
            _raygunMessage.Details.UserCustomData = userCustomData;

            return this;
        }

        public RaygunMessageBuilder SetVersion()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly != null)
            {
                _raygunMessage.Details.Version = entryAssembly.GetName()
                                                              .Version.ToString();
            }
            else
            {
                _raygunMessage.Details.Version = "Not supplied";
            }

            return this;
        }
    }
}
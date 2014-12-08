namespace Raygun
{
    using System;
    using System.Collections.Generic;

    using Raygun.Messages;

    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    public interface IRaygunMessageBuilder
    {
        RaygunMessage Build();

        IRaygunMessageBuilder SetClientDetails();
        IRaygunMessageBuilder SetEnvironmentDetails();
        IRaygunMessageBuilder SetExceptionDetails(Exception exception);
        IRaygunMessageBuilder SetHttpDetails(OwinEnvironment environment);
        IRaygunMessageBuilder SetMachineName(string machineName);
        IRaygunMessageBuilder SetTags(IList<string> tags);
        IRaygunMessageBuilder SetUserCustomData(IDictionary<string, object> userCustomData);
        IRaygunMessageBuilder SetVersion();
    }
}
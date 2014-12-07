namespace Raygun
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Raygun.Messages;

    public interface IRaygunMessageBuilder
    {
        RaygunMessage Build();

        IRaygunMessageBuilder SetClientDetails();
        IRaygunMessageBuilder SetEnvironmentDetails();
        IRaygunMessageBuilder SetExceptionDetails(Exception exception);
        IRaygunMessageBuilder SetHttpDetails(IDictionary<string, object> environment);
        IRaygunMessageBuilder SetMachineName(string machineName);
        IRaygunMessageBuilder SetUserCustomData(IDictionary userCustomData);
        IRaygunMessageBuilder SetVersion();
    }
}
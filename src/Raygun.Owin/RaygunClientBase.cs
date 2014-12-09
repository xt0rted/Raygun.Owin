namespace Raygun
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public abstract class RaygunClientBase
    {
        protected internal const string SentKey = "AlreadySentByRaygun";
        protected static readonly List<Func<Exception, bool>> IgnoredExceptionFilters = new List<Func<Exception, bool>>();

        protected bool CanSend(Exception exception)
        {
            var canSend = exception == null ||
                          exception.Data == null ||
                          !exception.Data.Contains(SentKey) ||
                          false.Equals(exception.Data[SentKey]);

            return canSend && IgnoredExceptionFilters.All(filter =>
            {
                // if for some reason a filter throws an exception then we want to send anyhow
                try
                {
                    return filter(exception);
                }
                catch
                {
                    return true;
                }
            });
        }

        protected void FlagAsSent(Exception exception)
        {
            if (exception != null && exception.Data != null)
            {
                try
                {
                    var genericTypes = exception.Data.GetType().GetGenericArguments();
                    if (genericTypes.Length == 0 || genericTypes[0].IsAssignableFrom(typeof (string)))
                    {
                        exception.Data[SentKey] = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("Failed to flag exception as sent: {0}", ex.Message));
                }
            }
        }
    }
}
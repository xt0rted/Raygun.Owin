namespace Raygun
{
    using System;
    using System.Diagnostics;

    public abstract class RaygunClientBase
    {
        protected internal const string SentKey = "AlreadySentByRaygun";

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
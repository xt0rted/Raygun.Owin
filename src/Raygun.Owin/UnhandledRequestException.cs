namespace Raygun
{
    using System;

    public class UnhandledRequestException : Exception
    {
        public UnhandledRequestException(string url)
            : base("The request was unhandled.")
        {
            RequestUrl = url;
        }

        public UnhandledRequestException(string url, string message)
            : base(message)
        {
            RequestUrl = url;
        }

        public UnhandledRequestException(string url, string message, Exception innerException)
            : base(message, innerException)
        {
            RequestUrl = url;
        }

        public virtual string RequestUrl { get; private set; }
    }
}
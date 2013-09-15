using System;

namespace Raygun
{
    public class UnhandledRequestException : Exception
    {
        public UnhandledRequestException()
            : base("The request was unhandled.")
        {
        }

        public UnhandledRequestException(string message)
            : base(message)
        {
        }

        public UnhandledRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
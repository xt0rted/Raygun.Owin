namespace Raygun.Messages
{
    using System.Collections.Generic;

    public class RaygunErrorMessage
    {
        public RaygunErrorMessage InnerError { get; set; }
        public IDictionary<object, object> Data { get; set; }
        public string ClassName { get; set; }
        public string Message { get; set; }
        public IEnumerable<RaygunErrorStackTraceLineMessage> StackTrace { get; set; }
    }
}
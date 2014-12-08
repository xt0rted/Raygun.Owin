namespace Raygun.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Raygun.Messages;

    public class RaygunErrorMessageBuilder : RaygunErrorMessageBuilderBase
    {
        public static RaygunErrorMessage Build(Exception exception)
        {
            var message = new RaygunErrorMessage();

            var exceptionType = exception.GetType();

            message.Message = exception.Message;
            message.ClassName = exceptionType.FullName;

            message.StackTrace = BuildStackTrace(exception);

            if (exception.Data != null)
            {
                var data = new Dictionary<object, object>();
                foreach (var key in exception.Data.Keys)
                {
                    if (!RaygunClientBase.SentKey.Equals(key))
                    {
                        data[key] = exception.Data[key];
                    }
                }

                message.Data = data;
            }

            if (exception.InnerException != null)
            {
                message.InnerError = Build(exception.InnerException);
            }

            return message;
        }

        private static RaygunErrorStackTraceLineMessage[] BuildStackTrace(Exception exception)
        {
            var lines = new List<RaygunErrorStackTraceLineMessage>();

        private static IEnumerable<RaygunErrorStackTraceLineMessage> BuildStackTrace(Exception exception)
        {
            var stackTrace = new StackTrace(exception, true);
            var frames = stackTrace.GetFrames();

            if (frames == null || frames.Length == 0)
            {
                yield return new RaygunErrorStackTraceLineMessage { FileName = "none", LineNumber = 0 };
                yield break;
            }

            foreach (var frame in frames)
            {
                var method = frame.GetMethod();

                if (method != null)
                {
                    var lineNumber = frame.GetFileLineNumber();

                    if (lineNumber == 0)
                    {
                        lineNumber = frame.GetILOffset();
                    }

                    var methodName = GenerateMethodName(method);

                    var file = frame.GetFileName();

                    var className = method.ReflectedType != null ? method.ReflectedType.FullName : "(unknown)";

                    yield return new RaygunErrorStackTraceLineMessage
                    {
                        FileName = file,
                        LineNumber = lineNumber,
                        MethodName = methodName,
                        ClassName = className
                    };
                }
            }
        }
    }
}
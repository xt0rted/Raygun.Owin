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

            message.Data = exception.Data;

            if (exception.InnerException != null)
            {
                message.InnerError = Build(exception.InnerException);
            }

            return message;
        }

        private static RaygunErrorStackTraceLineMessage[] BuildStackTrace(Exception exception)
        {
            var lines = new List<RaygunErrorStackTraceLineMessage>();

            var stackTrace = new StackTrace(exception, true);
            var frames = stackTrace.GetFrames();

            if (frames == null || frames.Length == 0)
            {
                var line = new RaygunErrorStackTraceLineMessage { FileName = "none", LineNumber = 1 };
                lines.Add(line);
                return lines.ToArray();
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

                    var line = new RaygunErrorStackTraceLineMessage
                    {
                        FileName = file,
                        LineNumber = lineNumber,
                        MethodName = methodName,
                        ClassName = className
                    };

                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }
    }
}
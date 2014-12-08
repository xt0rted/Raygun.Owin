namespace Raygun.Builders
{
    using System.Reflection;
    using System.Text;

    public abstract class RaygunErrorMessageBuilderBase
    {
        protected static string GenerateMethodName(MethodBase method)
        {
            var result = new StringBuilder();

            result.Append(method.Name);

            var first = true;
            if (method is MethodInfo && method.IsGenericMethod)
            {
                var genericArguments = method.GetGenericArguments();

                result.Append("[");

                for (var i = 0; i < genericArguments.Length; i++)
                {
                    if (!first)
                    {
                        result.Append(",");
                    }
                    else
                    {
                        first = false;
                    }

                    result.Append(genericArguments[i].Name);
                }

                result.Append("]");
            }

            result.Append("(");

            var parameters = method.GetParameters();
            first = true;
            for (var i = 0; i < parameters.Length; i++)
            {
                if (!first)
                {
                    result.Append(", ");
                }
                else
                {
                    first = false;
                }

                var type = "<UnknownType>";

                if (parameters[i].ParameterType != null)
                {
                    type = parameters[i].ParameterType.Name;
                }

                result.Append(type);
                result.Append(" ");
                result.Append(parameters[i].Name);
            }

            result.Append(")");

            return result.ToString();
        }
    }
}
namespace Raygun
{
    using System;
    using System.Reflection;

    internal static class ReflectionHelpers
    {
        // http://stackoverflow.com/q/1196991/39605
        public static object GetPropertyValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static object RunInstanceMethod(object source, string method)
        {
            return RunInstanceMethod(source, method, new object[] { });
        }

        public static object RunInstanceMethod(object source, string method, object[] objParams)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var type = source.GetType();
            var m = type.GetMethod(method, flags);
            if (m == null)
            {
                throw new ArgumentException(string.Format("There is no method '{0}' for type '{1}'.", method, type));
            }

            var objRet = m.Invoke(source, objParams);
            return objRet;
        }
    }
}
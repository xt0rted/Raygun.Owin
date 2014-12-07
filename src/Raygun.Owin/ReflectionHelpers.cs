namespace Raygun
{
    using System;
    using System.Reflection;

    internal static class ReflectionHelpers
    {
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
namespace Raygun
{
    using OwinEnvironment = System.Collections.Generic.IDictionary<string, object>;

    internal static class Helpers
    {
        internal static T Get<T>(this OwinEnvironment env, string key)
        {
            object value;
            if (env.TryGetValue(key, out value))
            {
                return (T)value;
            }

            return default(T);
        }
    }
}
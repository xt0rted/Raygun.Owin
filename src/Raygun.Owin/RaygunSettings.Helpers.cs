namespace Raygun
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public partial class RaygunSettings
    {
        private static bool GetBoolean(string settingName, bool defaultValue)
        {
            bool temp;
            if (!bool.TryParse(ConfigurationManager.AppSettings[settingName], out temp))
            {
                temp = defaultValue;
            }

            return temp;
        }

        private static string GetString(string settingName, string defaultValue = null)
        {
            var temp = ConfigurationManager.AppSettings[settingName];
            if (string.IsNullOrWhiteSpace(temp))
            {
                temp = defaultValue;
            }

            return temp;
        }

        internal static RaygunSettings LoadFromAppSettings()
        {
            var settings = new RaygunSettings
            {
                ApiEndpoint = GetString("raygun:apiEndpoint", Constants.RaygunApiEndpoint),
                ApiKey = GetString("raygun:apiKey"),
                ApplicationVersion = LoadApplicationVersion(GetString("raygun:applicationVersion")),
                Tags = LoadTags(GetString("raygun:tags")),
                ThrowOnError = GetBoolean("raygun:throwOnError", true),
                ExcludeErrorsFromLocal = GetBoolean("raygun:excludeErrorsFromLocal", false)
            };

            return settings;
        }

        private static string LoadApplicationVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return null;
            }

            return version.Trim();
        }

        private static IList<string> LoadTags(string tags)
        {
            tags = tags ?? string.Empty;

            return tags.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(t => t.Trim())
                       .Where(t => t != string.Empty)
                       .ToList();
        }
    }
}

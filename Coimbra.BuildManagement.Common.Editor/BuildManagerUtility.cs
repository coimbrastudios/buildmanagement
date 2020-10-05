using System;
using System.IO;

namespace Coimbra.BuildManagement.Common
{
    internal static class BuildManagerUtility
    {
        internal const string PackageName = "com.coimbrastudios.buildmanagement";
        internal const string UserPreferencesPath = "Preferences/CS Build Management";
        internal const string ProjectSettingsPath = "Project/CS Build Management";

        internal static void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        internal static bool TryMatchSearch(string searchContext, string target)
        {
            if (searchContext == null)
            {
                return true;
            }

            searchContext = searchContext.Trim();

            if (string.IsNullOrEmpty(searchContext))
            {
                return true;
            }

            string[] split = searchContext.Split(' ');

            foreach (string value in split)
            {
                if (!string.IsNullOrEmpty(value) && target.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

using System;
using System.IO;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace Coimbra.BuildManagement.Editor.Common
{
    internal static class BuildManagerUtility
    {
        internal const string PackageName = "com.coimbrastudios.buildmanagement";
        internal const string UserPreferencesPath = "Preferences/CS Build Management";
        internal const string ProjectSettingsPath = "Project/CS Build Management";

        internal static void DrawBoolStateField(string searchContext, GUIContent label, UserSetting<BoolState> userSetting, bool defaultValue, Action onChanged)
        {
            if (!TryMatchSearch(searchContext, label.text))
            {
                return;
            }

            using (EditorGUI.ChangeCheckScope changeCheckScope = new())
            {
                BoolState value = EditorGUILayout.Toggle(label, userSetting.value.GetOrDefault(defaultValue)) ? BoolState.True : BoolState.False;

                if (changeCheckScope.changed)
                {
                    userSetting.value = value;
                    onChanged?.Invoke();
                }

                SettingsGUILayout.DoResetContextMenuForLastRect(userSetting);
            }
        }

        internal static void DrawEnumField<T>(string searchContext, GUIContent label, UserSetting<T> userSetting, Action onChanged)
            where T : Enum
        {
            if (!TryMatchSearch(searchContext, label.text))
            {
                return;
            }

            using (EditorGUI.ChangeCheckScope changeCheckScope = new())
            {
                T value = (T)EditorGUILayout.EnumPopup(label, userSetting);

                if (changeCheckScope.changed)
                {
                    userSetting.value = value;
                    onChanged?.Invoke();
                }

                SettingsGUILayout.DoResetContextMenuForLastRect(userSetting);
            }
        }

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

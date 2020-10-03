using Coimbra.BuildManagement.Common;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace Coimbra.BuildManagement.Global
{
    [NoReorder]
    internal static class GlobalSettingsProvider
    {
        private enum ScriptingBackend
        {
            [UsedImplicitly] Mono,
            [UsedImplicitly] IL2CPP,
        }

        [NoReorder]
        internal static class General
        {
            [UserSetting] private static readonly GlobalSetting<bool> UseProjectFolderAsBuildNameSetting = new GlobalSetting<bool>("General.UseProjectFolderAsBuildName", false);
            [UserSetting] private static readonly GlobalSetting<bool> ShowSplashScreenSetting = new GlobalSetting<bool>("General.ShowSplashScreen", true);
            [UserSetting] private static readonly GlobalSetting<bool> ShowUnityLogoSetting = new GlobalSetting<bool>("General.ShowUnityLogo", false);
            [UserSetting] private static readonly GlobalSetting<bool> UseUtcNowAsIosBuildNumberSetting = new GlobalSetting<bool>("General.UseUtcNowAsIosBuildNumber", true);
            [UserSetting] private static readonly GlobalSetting<bool> UseUtcNowAsMacBuildNumberSetting = new GlobalSetting<bool>("General.UseUtcNowAsMacBuildNumber", true);

            private static readonly GUIContent ShowUserPreferencesLabel = new GUIContent("Show User Preferences");
            private static readonly GUIContent UseProjectFolderAsBuildNameLabel = new GUIContent("Use Project Folder As Build Name*",
                                                                                                 "If not checked, the build name metadata will be the same as the product name.");
            private static readonly GUIContent ShowSplashScreenLabel = new GUIContent("Show Splash Screen*",
                                                                                      "This setting will only take effect if you are a Plus/Pro subscriber. Useful if working on a project in which not all developers have an active subscription.");
            private static readonly GUIContent ShowUnityLogoLabel = new GUIContent("Show Unity Logo*",
                                                                                   "This setting will only take effect if you are a Plus/Pro subscriber. Useful when working on a project in which not all developers have an active subscription.");
            private static readonly GUIContent UseUtcNowAsIosBuildNumberLabel = new GUIContent("Use Utc Now as iOS Build Number*",
                                                                                               "If checked, the iOS Build Number will be the build time in UTC (Coordinated Universal Time). The build number will be in the format 'yyyy.MMdd.HHmm' thus ensuring an ever-increasing and ever-changing Build Number.");
            private static readonly GUIContent UseUtcNowAsMacBuildNumberLabel = new GUIContent("Use Utc Now as Mac Build Number*",
                                                                                               "If checked, the Mac Build Number will be the build time in UTC (Coordinated Universal Time). The build number will be in the format 'yyyy.MMdd.HHmm' thus ensuring an ever-increasing and ever-changing Build Number.");

            internal static bool UseProjectFolderAsBuildName => UseProjectFolderAsBuildNameSetting;
            internal static bool ShowSplashScreen => ShowSplashScreenSetting;
            internal static bool ShowUnityLogo => ShowUnityLogoSetting;
            internal static bool UseUtcNowAsIosBuildNumber => UseUtcNowAsIosBuildNumberSetting;
            internal static bool UseUtcNowAsMacBuildNumber => UseUtcNowAsMacBuildNumberSetting;

            [UsedImplicitly] [UserSettingBlock(" ")]
            private static void DrawGeneralBlock(string searchContext)
            {
                if (BuildManagerUtility.TryMatchSearch(searchContext, ShowUserPreferencesLabel.text))
                {
                    if (GUILayout.Button(ShowUserPreferencesLabel))
                    {
                        SettingsService.OpenUserPreferences(BuildManagerUtility.UserPreferencesPath);
                    }

                    EditorGUILayout.Separator();
                }

                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    UseProjectFolderAsBuildNameSetting.value = SettingsGUILayout.SettingsToggle(UseProjectFolderAsBuildNameLabel, UseProjectFolderAsBuildNameSetting, searchContext);
                    ShowSplashScreenSetting.value = SettingsGUILayout.SettingsToggle(ShowSplashScreenLabel, ShowSplashScreenSetting, searchContext);

                    using (new EditorGUI.IndentLevelScope())
                    {
                        using (new EditorGUI.DisabledScope(!ShowSplashScreenSetting))
                        {
                            ShowUnityLogoSetting.value = SettingsGUILayout.SettingsToggle(ShowUnityLogoLabel, ShowUnityLogoSetting, searchContext);
                        }
                    }

                    UseUtcNowAsIosBuildNumberSetting.value = SettingsGUILayout.SettingsToggle(UseUtcNowAsIosBuildNumberLabel, UseUtcNowAsIosBuildNumberSetting, searchContext);
                    UseUtcNowAsMacBuildNumberSetting.value = SettingsGUILayout.SettingsToggle(UseUtcNowAsMacBuildNumberLabel, UseUtcNowAsMacBuildNumberSetting, searchContext);

                    if (changeCheckScope.changed)
                    {
                        Settings.Save();
                    }
                }
            }
        }

        [NoReorder]
        internal static class CloudBuild
        {
            [UserSetting] private static readonly GlobalSetting<bool> AppendBuildNumberToVersionSetting = new GlobalSetting<bool>("CloudBuild.AppendBuildNumberToVersion", false);
            [UserSetting] private static readonly GlobalSetting<string> BuildNumberExpressionSetting = new GlobalSetting<string>("CloudBuild.BuildNumberExpression", BuildNumberPlaceholder);
            [UserSetting] private static readonly GlobalSetting<bool> UseBuildStartTimeAsUtcNowSetting = new GlobalSetting<bool>("CloudBuild.UseBuildStartTimeAsUtcNow", true);
            [UserSetting] private static readonly GlobalSetting<bool> SumBuildNumberToBundleVersionCodeSetting = new GlobalSetting<bool>("CloudBuild.SumBuildNumberToBundleVersionCode", true);

            private const string BuildNumberPlaceholder = "#";
            private static readonly GUIContent AppendBuildNumberToVersionLabel = new GUIContent("Append Build Number to Version*",
                                                                                                "If checked, it will append the build number to the end of the version. This will ensure an ever-increasing and ever-changing Version.");
            private static readonly GUIContent BuildNumberExpressionLabel = new GUIContent("Build Number Expression*",
                                                                                           $"Use {BuildNumberPlaceholder} symbol as the build number placeholder. If the expression is not valid, the result will be the Build Number.");
            private static readonly GUIContent UseBuildStartTimeAsUtcNowLabel = new GUIContent("Use Build Start Time as Utc Now*",
                                                                                               "If checked, it will override the UTC (Coordinated Universal Time) information with the actual Build Start Time information from the Unity Cloud Build.");
            private static readonly GUIContent SumBuildNumberToBundleVersionCodeLabel = new GUIContent("Sum Build Number to Bundle Version Code*",
                                                                                                       "If checked, the Android Bundle Version Code will be a sum of the current Bundle Version Code with the Unity Cloud Build build number. This will ensure an ever-increasing and ever-changing Bundle Version Code.");

            internal static bool AppendBuildNumberToVersion => AppendBuildNumberToVersionSetting;
            internal static bool UseBuildStartTimeAsUtcNow => UseBuildStartTimeAsUtcNowSetting;
            internal static bool SumBuildNumberToBundleVersionCode => SumBuildNumberToBundleVersionCodeSetting;

            public static string GetBuildNumberExpression(string buildNumber)
            {
                return BuildNumberExpressionSetting.value.Replace(BuildNumberPlaceholder, buildNumber);
            }

            [UsedImplicitly] [UserSettingBlock("Cloud Build")]
            private static void DrawCloudBuildsBlock(string searchContext)
            {
                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    AppendBuildNumberToVersionSetting.value = SettingsGUILayout.SettingsToggle(AppendBuildNumberToVersionLabel, AppendBuildNumberToVersionSetting, searchContext);

                    using (new EditorGUI.IndentLevelScope())
                    {
                        using (new EditorGUI.DisabledScope(!AppendBuildNumberToVersionSetting))
                        {
                            if (BuildManagerUtility.TryMatchSearch(searchContext, BuildNumberExpressionLabel.text))
                            {
                                BuildNumberExpressionSetting.value = EditorGUILayout.TextField(BuildNumberExpressionLabel, BuildNumberExpressionSetting);
                                SettingsGUILayout.DoResetContextMenuForLastRect(BuildNumberExpressionSetting);

                                Rect position = EditorGUILayout.GetControlRect(true);
                                position.xMin += EditorGUIUtility.labelWidth;

                                if (GUI.Button(position, "Validate"))
                                {
                                    string buildNumber = Random.Range(1, 100).ToString();
                                    string original = BuildNumberExpressionSetting;
                                    string replaced = original.Replace(BuildNumberPlaceholder, buildNumber);

                                    if (ExpressionEvaluator.Evaluate(replaced, out int result))
                                    {
                                        Debug.Log($"Expression ({original}) is valid! Validated with Build Number {buildNumber}: ({replaced})={result}");
                                    }
                                    else
                                    {
                                        Debug.LogError($"({original}) is not a valid expression!");
                                    }
                                }
                            }
                        }
                    }

                    UseBuildStartTimeAsUtcNowSetting.value = SettingsGUILayout.SettingsToggle(UseBuildStartTimeAsUtcNowLabel, UseBuildStartTimeAsUtcNowSetting, searchContext);
                    SumBuildNumberToBundleVersionCodeSetting.value = SettingsGUILayout.SettingsToggle(SumBuildNumberToBundleVersionCodeLabel, SumBuildNumberToBundleVersionCodeSetting, searchContext);

                    if (changeCheckScope.changed)
                    {
                        Settings.Save();
                    }
                }
            }
        }

        [NoReorder]
        internal static class LocalBuild
        {
            [UserSetting] private static readonly GlobalSetting<bool> AllowFallbackToMonoSetting = new GlobalSetting<bool>("LocalBuild.AllowFallbackToMono", true);
            [UserSetting] private static readonly GlobalSetting<ScriptingBackend> PreferredLinuxScriptingBackendSetting = new GlobalSetting<ScriptingBackend>("LocalBuild.PreferredLinuxScriptingBackend", ScriptingBackend.IL2CPP);
            [UserSetting] private static readonly GlobalSetting<ScriptingBackend> PreferredMacScriptingBackendSetting = new GlobalSetting<ScriptingBackend>("LocalBuild.PreferredMacScriptingBackend", ScriptingBackend.IL2CPP);
            [UserSetting] private static readonly GlobalSetting<ScriptingBackend> PreferredWindowsScriptingBackendSetting = new GlobalSetting<ScriptingBackend>("LocalBuild.PreferredWindowsScriptingBackend", ScriptingBackend.IL2CPP);

            private static readonly GUIContent AllowFallbackToMonoLabel = new GUIContent("Allow Fallback To Mono*",
                                                                                         "If IL2PP is the preferred scripting backend and it is not supported for the current selected platform in your machine it will automatically fallback to Mono.");
            private static readonly GUIContent PreferredLinuxScriptingBackendLabel = new GUIContent("Preferred Linux Scripting Backend");
            private static readonly GUIContent PreferredMacScriptingBackendLabel = new GUIContent("Preferred Mac Scripting Backend");
            private static readonly GUIContent PreferredWindowsScriptingBackendLabel = new GUIContent("Preferred Windows Scripting Backend");

            internal static bool AllowFallbackToMono => AllowFallbackToMonoSetting;
            internal static ScriptingImplementation PreferredLinuxScriptingBackend => (ScriptingImplementation)PreferredLinuxScriptingBackendSetting.value;
            internal static ScriptingImplementation PreferredMacScriptingBackend => (ScriptingImplementation)PreferredMacScriptingBackendSetting.value;
            internal static ScriptingImplementation PreferredWindowsScriptingBackend => (ScriptingImplementation)PreferredWindowsScriptingBackendSetting.value;

            [UsedImplicitly] [UserSettingBlock("Local Build")]
            private static void DrawLocalBuildsBlock(string searchContext)
            {
                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    AllowFallbackToMonoSetting.value = SettingsGUILayout.SettingsToggle(AllowFallbackToMonoLabel, AllowFallbackToMonoSetting, searchContext);

                    using (new EditorGUI.IndentLevelScope())
                    {
                        using (new EditorGUI.DisabledScope(!AllowFallbackToMonoSetting))
                        {
                            if (BuildManagerUtility.TryMatchSearch(searchContext, PreferredLinuxScriptingBackendLabel.text))
                            {
                                PreferredLinuxScriptingBackendSetting.value = (ScriptingBackend)EditorGUILayout.EnumPopup(PreferredLinuxScriptingBackendLabel, PreferredLinuxScriptingBackendSetting);
                                SettingsGUILayout.DoResetContextMenuForLastRect(PreferredLinuxScriptingBackendSetting);
                            }

                            if (BuildManagerUtility.TryMatchSearch(searchContext, PreferredMacScriptingBackendLabel.text))
                            {
                                PreferredMacScriptingBackendSetting.value = (ScriptingBackend)EditorGUILayout.EnumPopup(PreferredMacScriptingBackendLabel, PreferredMacScriptingBackendSetting);
                                SettingsGUILayout.DoResetContextMenuForLastRect(PreferredMacScriptingBackendSetting);
                            }

                            if (BuildManagerUtility.TryMatchSearch(searchContext, PreferredWindowsScriptingBackendLabel.text))
                            {
                                PreferredWindowsScriptingBackendSetting.value = (ScriptingBackend)EditorGUILayout.EnumPopup(PreferredWindowsScriptingBackendLabel, PreferredWindowsScriptingBackendSetting);
                                SettingsGUILayout.DoResetContextMenuForLastRect(PreferredWindowsScriptingBackendSetting);
                            }
                        }
                    }

                    if (changeCheckScope.changed)
                    {
                        Settings.Save();
                    }
                }
            }
        }

        private static Settings _settings;

        [NotNull] internal static Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings(BuildManagerUtility.PackageName);
                }

                return _settings;
            }
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new UserSettingsProvider(BuildManagerUtility.ProjectSettingsPath, Settings, new[]
            {
                typeof(GlobalSettingsProvider).Assembly,
            }, SettingsScope.Project);
        }
    }
}

using Coimbra.BuildManagement.Common;
using JetBrains.Annotations;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace Coimbra.BuildManagement.Local
{
    [NoReorder]
    internal static class LocalSettingsProvider
    {
        [UserSetting] private static readonly LocalSetting<OpenBuiltPlayerOptions> OpenBuiltPlayerOptionsSetting = new LocalSetting<OpenBuiltPlayerOptions>(BuildManagerUtility.PackageName + ".localBuilds.openBuiltPlayerOptions", OpenBuiltPlayerOptions.OpenStandardizedOutput);
        [UserSetting] private static readonly LocalSetting<bool> CreateStandardizedBuildOutputSetting = new LocalSetting<bool>(BuildManagerUtility.PackageName + ".localBuilds.createStandardizedBuildOutput", true);
        [UserSetting] private static readonly LocalSetting<bool> GroupByBuildNameSetting = new LocalSetting<bool>(BuildManagerUtility.PackageName + ".localBuilds.groupByBuildName", false);
        [UserSetting] private static readonly LocalSetting<bool> GroupByBuildTargetSetting = new LocalSetting<bool>(BuildManagerUtility.PackageName + ".localBuilds.groupByBuildTarget", true);
        [UserSetting] private static readonly LocalSetting<string> StandardizedBuildOutputPathSetting = new LocalSetting<string>(BuildManagerUtility.PackageName + ".localBuilds.standardizedBuildOutputPath", DefaultStandardizedBuildOutputPath);

        private const string DefaultStandardizedBuildOutputPath = "Builds";
        private static readonly GUIContent OpenBuiltPlayerOptionsLabel = new GUIContent("Open Built Player Options*",
                                                                                        "By default Unity shows the built player folder after the building process. With this option you can choose to see both the original output folder and the standardized output, only one of them or even none of them at all.");
        private static readonly GUIContent CreateStandardizedBuildOutputLabel = new GUIContent("Create Standardized Build Output*",
                                                                                               "If checked it will copy the built player into the selected folder with a standardized name.");
        private static readonly GUIContent GroupByBuildNameLabel = new GUIContent("Group by Build Name*",
                                                                                  "If checked, the copied output will be put inside a folder with the same name as the build name. This way you can choose to have one build folder for all your projects without becoming starting mess.");
        private static readonly GUIContent GroupByBuildTargetLabel = new GUIContent("Group by Build Target*",
                                                                                    "If checked, the copied output will be put inside a folder with the name of the build target. If Group by Build Name is also checked, the build target folder folder will be placed inside the build name folder.");
        private static readonly GUIContent StandardizedBuildOutputPathLabel = new GUIContent("Standardized Build Output Path*",
                                                                                             "You can click in the path label to open the folder. The default value is a folder named 'Builds' relative to your project folder.");
        private static readonly GUIContent ChangeButtonLabel = new GUIContent("Change Path");
        private static readonly GUIContent ShowProjectSettingsLabel = new GUIContent("Show Project Settings");
        private static Settings _settings;

        internal static bool CreateStandardizedBuildOutput => CreateStandardizedBuildOutputSetting;
        internal static bool GroupByBuildName => GroupByBuildNameSetting;
        internal static bool GroupByBuildTarget => GroupByBuildTargetSetting;
        internal static OpenBuiltPlayerOptions OpenBuiltPlayerOptions => OpenBuiltPlayerOptionsSetting;
        internal static string StandardizedBuildOutputPath
        {
            get
            {
                if (StandardizedBuildOutputPathSetting != DefaultStandardizedBuildOutputPath)
                {
                    return StandardizedBuildOutputPathSetting;
                }

                string path = Path.GetDirectoryName(Application.dataPath);
                Assert.IsFalse(string.IsNullOrWhiteSpace(path));

                return Path.Combine(path, DefaultStandardizedBuildOutputPath);
            }
        }
        [NotNull]
        internal static Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings(new[]
                    {
                        new UserSettingsFolderRepository(BuildManagerUtility.PackageName, "Settings"),
                    });
                }

                return _settings;
            }
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new UserSettingsProvider(BuildManagerUtility.UserPreferencesPath, Settings, new[]
            {
                typeof(LocalSettingsProvider).Assembly,
            });
        }

        [UsedImplicitly] [UserSettingBlock(" ")]
        private static void DrawHeaderBlock(string searchContext)
        {
            if (BuildManagerUtility.TryMatchSearch(searchContext, ShowProjectSettingsLabel.text))
            {
                if (GUILayout.Button(ShowProjectSettingsLabel))
                {
                    SettingsService.OpenProjectSettings(BuildManagerUtility.ProjectSettingsPath);
                }

                EditorGUILayout.Separator();
            }

            using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                if (BuildManagerUtility.TryMatchSearch(searchContext, OpenBuiltPlayerOptionsLabel.text))
                {
                    OpenBuiltPlayerOptionsSetting.value = (OpenBuiltPlayerOptions)EditorGUILayout.EnumFlagsField(OpenBuiltPlayerOptionsLabel, OpenBuiltPlayerOptionsSetting);
                    SettingsGUILayout.DoResetContextMenuForLastRect(OpenBuiltPlayerOptionsSetting);
                }

                CreateStandardizedBuildOutputSetting.value = SettingsGUILayout.SettingsToggle(CreateStandardizedBuildOutputLabel, CreateStandardizedBuildOutputSetting, searchContext);

                if (changeCheckScope.changed)
                {
                    Settings.Save();
                }
            }

            using (new EditorGUI.DisabledScope(!CreateStandardizedBuildOutputSetting))
            {
                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        GroupByBuildNameSetting.value = SettingsGUILayout.SettingsToggle(GroupByBuildNameLabel, GroupByBuildNameSetting, searchContext);
                        GroupByBuildTargetSetting.value = SettingsGUILayout.SettingsToggle(GroupByBuildTargetLabel, GroupByBuildTargetSetting, searchContext);

                        if (changeCheckScope.changed)
                        {
                            Settings.Save();
                        }

                        if (!BuildManagerUtility.TryMatchSearch(searchContext, StandardizedBuildOutputPathLabel.text)
                         && !BuildManagerUtility.TryMatchSearch(searchContext, ChangeButtonLabel.text))
                        {
                            return;
                        }

                        Rect position = EditorGUILayout.GetControlRect();
                        position = EditorGUI.PrefixLabel(position, StandardizedBuildOutputPathLabel);

                        if (GUI.Button(position, ChangeButtonLabel, EditorStyles.miniButton))
                        {
                            string value = EditorUtility.OpenFolderPanel("Builds folder location", Path.GetDirectoryName(StandardizedBuildOutputPath), Path.GetFileName(StandardizedBuildOutputPath));

                            if (string.IsNullOrEmpty(value) == false)
                            {
                                StandardizedBuildOutputPathSetting.SetValue(value, true);
                            }
                        }

                        SettingsGUILayout.DoResetContextMenuForLastRect(StandardizedBuildOutputPathSetting);
                        position = EditorGUILayout.GetControlRect();

                        if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && position.Contains(Event.current.mousePosition))
                        {
                            string path = Directory.Exists(StandardizedBuildOutputPath) ? StandardizedBuildOutputPath : Path.GetDirectoryName(Application.dataPath);
                            Assert.IsFalse(string.IsNullOrWhiteSpace(path));
                            Process.Start(path);
                        }

                        string outputLabel = StandardizedBuildOutputPathSetting == DefaultStandardizedBuildOutputPath
                                                 ? $"./{DefaultStandardizedBuildOutputPath}"
                                                 : StandardizedBuildOutputPath;

                        EditorGUI.LabelField(position, outputLabel, EditorStyles.miniLabel);
                        SettingsGUILayout.DoResetContextMenuForLastRect(StandardizedBuildOutputPathSetting);
                    }
                }
            }
        }
    }
}

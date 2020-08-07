using Coimbra.BuildManagement.Editor.Global;
using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Coimbra.BuildManagement.Editor
{
    /// <summary>
    /// Use this class to <see cref="PreprocessBuild"/> configured in the Project Settings menu and to <see cref="Finish"/> the project after the build process.
    /// </summary>
    public static class BuildManager
    {
        private const string UtcNowFormat = "yyyy.MMdd.HHmm";

        private static readonly string CreatedStreamingAssetsKey = $"{nameof(Coimbra)}.{nameof(BuildManagement)}.{nameof(BuildMetadata)}.{nameof(CreatedStreamingAssetsKey)}";

        internal static string BuildName
        {
            get
            {
                if (GlobalSettingsProvider.General.UseProjectFolderAsBuildName)
                {
                    return Path.GetFileName(Path.GetDirectoryName(Application.dataPath));
                }

                return PlayerSettings.productName;
            }
        }

        /// <summary>
        /// Apply settings configured in the Project Settings menu.
        /// </summary>
        public static void ApplySettings()
        {
            DateTime rawUtcNow = DateTime.UtcNow;
            PlayerSettings.SplashScreen.show = GlobalSettingsProvider.General.ShowSplashScreen;
            PlayerSettings.SplashScreen.showUnityLogo = GlobalSettingsProvider.General.ShowUnityLogo;
            UnityCloudBuildManifest manifest = UnityCloudBuildManifest.GetInstance();

            if (manifest != null)
            {
                if (GlobalSettingsProvider.CloudBuild.AppendBuildNumberToVersion)
                {
                    string expression = GlobalSettingsProvider.CloudBuild.GetBuildNumberExpression(manifest.BuildNumber);
                    bool isValid = ExpressionEvaluator.Evaluate(expression, out int result);
                    string valueToAppend = isValid ? result.ToString() : manifest.BuildNumber;
                    PlayerSettings.bundleVersion = $"{PlayerSettings.bundleVersion}{valueToAppend}";
                }

                if (GlobalSettingsProvider.CloudBuild.UseBuildStartTimeAsUtcNow)
                {
                    rawUtcNow = DateTime.Parse(manifest.BuildStartTime);
                }

                // ReSharper disable once InvocationIsSkipped
                ApplyAndroidBundleVersionCode(manifest);
            }

            string utcNow = rawUtcNow.ToString(UtcNowFormat);

            // ReSharper disable once InvocationIsSkipped
            ApplyIosBuildNumber(utcNow);

            // ReSharper disable once InvocationIsSkipped
            ApplyMacOsBuildNumber(utcNow);

            string directory = Application.streamingAssetsPath;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                SessionState.SetBool(CreatedStreamingAssetsKey, true);
            }

            BuildMetadata buildMetadata = BuildMetadata.SetInstance(BuildName, $"{PlayerSettings.bundleVersion}-{utcNow}");

            const ImportAssetOptions options = ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport;

            AssetDatabase.ImportAsset(buildMetadata.AssetFilePath, options);
        }

        /// <summary>
        /// Clean data created temporarily during the build process and generate the standardized output.
        /// </summary>
        public static void Finish(BuildReport buildReport, bool generateStandardizedOutput, bool showStandardizedOutput = false)
        {
            BuildMetadata buildMetadata = BuildMetadata.GetInstance();
            StandardizedBuildCreator standardizedBuildCreator = null;

            if (buildReport != null && generateStandardizedOutput)
            {
                standardizedBuildCreator = new StandardizedBuildCreator(buildReport.summary, buildMetadata);
                standardizedBuildCreator.Schedule();
            }

            if (buildMetadata != null)
            {
                try
                {
                    File.Delete(buildMetadata.AbsoluteFilePath);
                    File.Delete($"{buildMetadata.AbsoluteFilePath}.meta");

                    if (SessionState.GetBool(CreatedStreamingAssetsKey, false))
                    {
                        string streamingAssetsPath = Application.streamingAssetsPath;

                        try
                        {
                            Directory.Delete(streamingAssetsPath);
                            File.Delete($"{streamingAssetsPath}.meta");
                        }
                        catch { }

                        SessionState.EraseBool(CreatedStreamingAssetsKey);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }

            standardizedBuildCreator?.Complete(showStandardizedOutput);
        }

        [Conditional("UNITY_STANDALONE")]
        internal static void ApplyStandaloneScriptingBackend(BuildTarget buildTarget)
        {
            if (!GlobalSettingsProvider.LocalBuild.AllowFallbackToMono)
            {
                return;
            }

            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                {
#if UNITY_EDITOR_WIN
                    SetWindowsScriptingBackend();
#else
                    SetFallbackScriptingBackend();
#endif
                    break;
                }

                case BuildTarget.StandaloneOSX:
                {
#if UNITY_EDITOR_OSX
                    SetMacScriptingBackend();
#else
                    SetFallbackScriptingBackend();
#endif
                    break;
                }

                case BuildTarget.StandaloneLinux64:
                {
#if UNITY_EDITOR_LINUX
                    SetLinuxScriptingBackend();
#else
                    SetFallbackScriptingBackend();
#endif
                    break;
                }
            }
        }

        [Conditional("UNITY_ANDROID")]
        private static void ApplyAndroidBundleVersionCode(UnityCloudBuildManifest manifest)
        {
            if (!GlobalSettingsProvider.CloudBuild.SumBuildNumberToBundleVersionCode)
            {
                return;
            }

            ExpressionEvaluator.Evaluate(manifest.BuildNumber, out int result);
            PlayerSettings.Android.bundleVersionCode += result;
        }

        [Conditional("UNITY_IOS")]
        private static void ApplyIosBuildNumber(string utcNow)
        {
            if (GlobalSettingsProvider.General.UseUtcNowAsIosBuildNumber)
            {
                PlayerSettings.iOS.buildNumber = utcNow;
            }
        }

        [Conditional("UNITY_STANDALONE_OSX")]
        private static void ApplyMacOsBuildNumber(string utcNow)
        {
            if (GlobalSettingsProvider.General.UseUtcNowAsMacBuildNumber)
            {
                PlayerSettings.macOS.buildNumber = utcNow;
            }
        }

        private static void SetFallbackScriptingBackend()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        }

        [UsedImplicitly]
        private static void SetMacScriptingBackend()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, GlobalSettingsProvider.LocalBuild.PreferredMacScriptingBackend);
        }

        [UsedImplicitly]
        private static void SetLinuxScriptingBackend()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, GlobalSettingsProvider.LocalBuild.PreferredLinuxScriptingBackend);
        }

        [UsedImplicitly]
        private static void SetWindowsScriptingBackend()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, GlobalSettingsProvider.LocalBuild.PreferredWindowsScriptingBackend);
        }
    }
}
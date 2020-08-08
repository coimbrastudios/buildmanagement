using Coimbra.BuildManagement.Editor.Local;
using JetBrains.Annotations;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace Coimbra.BuildManagement.Editor
{
    /// <summary>
    /// Use <see cref="BuildPlayer"/> method in <see cref="BuildPlayerWindow.RegisterBuildPlayerHandler"/> to integrate with the standard <see cref="BuildPipeline"/>
    /// </summary>
    public static class BuildPlayerHandler
    {
        /// <summary>
        /// Apply the <see cref="BuildManager"/> configurations and build the player.
        /// </summary>
        [PublicAPI]
        public static void BuildPlayer(BuildPlayerOptions buildPlayerOptions)
        {
            if (string.IsNullOrEmpty(buildPlayerOptions.locationPathName))
            {
                return;
            }

            if (buildPlayerOptions.target == BuildTarget.WebGL && Path.GetFileName(buildPlayerOptions.locationPathName) != "WebGL")
            {
                buildPlayerOptions.locationPathName = $"{buildPlayerOptions.locationPathName}/WebGL";
            }

            bool ignoreStandardizedOutput = !LocalSettingsProvider.CreateStandardizedBuildOutput
#if UNITY_STANDALONE_OSX
                                         || UnityEditor.OSXStandalone.UserBuildSettings.createXcodeProject
#elif UNITY_STANDALONE_WIN
                                         || UnityEditor.WindowsStandalone.UserBuildSettings.createSolution
#endif
                                         || EditorUserBuildSettings.exportAsGoogleAndroidProject;

            switch (buildPlayerOptions.target)
            {
                // Supported platforms for the standardized build output.
                case BuildTarget.Android:
                case BuildTarget.iOS:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.tvOS:
                case BuildTarget.WebGL:
                    break;

                default:
                {
                    ignoreStandardizedOutput = true;

                    Debug.Log($"Generating a standardized build output for {buildPlayerOptions.target} is not currently supported.");

                    break;
                }
            }

            OpenBuiltPlayerOptions openBuiltPlayerOptions = LocalSettingsProvider.OpenBuiltPlayerOptions;
            bool autoRunPlayer = (buildPlayerOptions.options & BuildOptions.AutoRunPlayer) != 0;
            bool openOriginalOutput = (openBuiltPlayerOptions & OpenBuiltPlayerOptions.OpenOriginalOutput) != 0;
            bool openStandardizedOutput = (openBuiltPlayerOptions & OpenBuiltPlayerOptions.OpenStandardizedOutput) != 0;

            openOriginalOutput = openOriginalOutput || (ignoreStandardizedOutput && openStandardizedOutput);

            if (autoRunPlayer || !openOriginalOutput)
            {
                buildPlayerOptions.options &= ~BuildOptions.ShowBuiltPlayer;
            }

            BuildManager.ApplyStandaloneScriptingBackend(buildPlayerOptions.target);
            BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

            if (buildReport == null || !ValidateBuildResult(buildReport.summary) || ignoreStandardizedOutput)
            {
                return;
            }

            StandardizedBuildCreator standardizedBuildCreator = new StandardizedBuildCreator(buildReport.summary, !autoRunPlayer && openStandardizedOutput);
            standardizedBuildCreator.Execute();
        }

        private static bool ValidateBuildResult(BuildSummary buildSummary)
        {
            string message = $"Build completed with a result of '{buildSummary.result:g}'";

            switch (buildSummary.result)
            {
                case BuildResult.Unknown:
                case BuildResult.Failed:
                {
                    Debug.LogError(message);

                    return false;
                }

                case BuildResult.Succeeded:
                {
                    Debug.Log(message);

                    return true;
                }

                case BuildResult.Cancelled:
                {
                    Debug.Log(message);

                    return false;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(buildSummary.result), $"{buildSummary.result}");
            }
        }
    }
}

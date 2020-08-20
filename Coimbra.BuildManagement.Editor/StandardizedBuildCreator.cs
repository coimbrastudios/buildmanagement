using Coimbra.BuildManagement.Common;
using Coimbra.BuildManagement.Local;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine.Assertions;

namespace Coimbra.BuildManagement
{
    internal sealed class StandardizedBuildCreator
    {
        private const string IgnoredFolderSuffix = "_BackUpThisFolder_ButDontShipItWithYourGame";

        private readonly bool _showFolder;
        private readonly string _buildName;
        private readonly string _buildVersion;
        private readonly string _originalOutputPath;
        private readonly string _productName;
        private readonly string _standardOutputFolderPath;
        private readonly BuildTarget _buildTarget;

        internal StandardizedBuildCreator(BuildSummary buildSummary, bool showFolder)
        {
            _showFolder = showFolder;
            _buildTarget = buildSummary.platform;
            _originalOutputPath = buildSummary.outputPath;
            _productName = PlayerSettings.productName;
            _buildVersion = $"v{BuildManager.LastFullVersion}";
            _standardOutputFolderPath = $"{LocalSettingsProvider.StandardizedBuildOutputPath}";
            _buildName = BuildManager.LastBuildName;

            if (LocalSettingsProvider.GroupByBuildName)
            {
                _standardOutputFolderPath = $"{_standardOutputFolderPath}/{_buildName}";
            }

            if (LocalSettingsProvider.GroupByBuildTarget)
            {
                _standardOutputFolderPath = $"{_standardOutputFolderPath}/{_buildTarget}";
            }

            _standardOutputFolderPath = $"{_standardOutputFolderPath}/{_buildName} ({_buildTarget}) {_buildVersion}";
        }

        internal void Execute()
        {
            switch (_buildTarget)
            {
                case BuildTarget.Android:
                {
                    string newOutputPath = $"{_standardOutputFolderPath}/{_buildName} {_buildVersion}";
                    CopyAndroidFilesAsync(_originalOutputPath, newOutputPath, _productName);

                    break;
                }

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux64:
                {
                    CopyAllFilesAsync(_originalOutputPath, _standardOutputFolderPath, _buildName);

                    break;
                }

                case BuildTarget.StandaloneOSX:
                {
                    string newOutputPath = $"{_standardOutputFolderPath}/{_buildName}.app";
                    CopyAllFilesAsync(_originalOutputPath, newOutputPath);

                    break;
                }

                case BuildTarget.iOS:
                case BuildTarget.tvOS:
                {
                    CopyAllFilesAsync(_originalOutputPath, _standardOutputFolderPath);

                    break;
                }

                case BuildTarget.WebGL:
                {
                    string newOutputPath = $"{_standardOutputFolderPath}/WebGL";
                    CopyAllFilesAsync(_originalOutputPath, newOutputPath);

                    break;
                }
            }

            if (_showFolder)
            {
                Process.Start(_standardOutputFolderPath);
            }
        }

        private static void CopyAllFilesAsync(string originalOutputPath, string standardOutputPath)
        {
            if (Directory.Exists(standardOutputPath))
            {
                Directory.Delete(standardOutputPath, true);
            }

            void action(string sourceFile)
            {
                string destinationFile = $"{standardOutputPath}/{sourceFile.Remove(0, originalOutputPath.Length)}";
                BuildManagerUtility.EnsureDirectoryExists(Path.GetDirectoryName(destinationFile));
                File.Copy(sourceFile, destinationFile);
            }

            string[] files = Directory.GetFiles(originalOutputPath, "*", SearchOption.AllDirectories);

            Parallel.ForEach(files, action);
        }

        private static void CopyAllFilesAsync(string originalOutputPath, string standardOutputFolderPath, string buildName)
        {
            if (Directory.Exists(standardOutputFolderPath))
            {
                Directory.Delete(standardOutputFolderPath, true);
            }

            string originalOutputFolderPath = Path.GetDirectoryName(originalOutputPath);
            string originalOutputFileName = Path.GetFileNameWithoutExtension(originalOutputPath);

            Assert.IsFalse(string.IsNullOrWhiteSpace(originalOutputFolderPath));

            string ignoredFolder = Path.Combine(originalOutputFolderPath, $"{originalOutputFileName}{IgnoredFolderSuffix}");

            void action(string sourceFile)
            {
                if (sourceFile.StartsWith(ignoredFolder))
                {
                    return;
                }

                string relativePath = sourceFile.Remove(0, originalOutputFolderPath.Length + 1);
                string standardOutputFileName;

                if (relativePath.StartsWith(originalOutputFileName))
                {
                    standardOutputFileName = $"{buildName}{relativePath.Remove(0, originalOutputFileName.Length)}";
                }
                else
                {
                    standardOutputFileName = relativePath;
                }

                string destinationFile = $"{standardOutputFolderPath}/{standardOutputFileName}";

                BuildManagerUtility.EnsureDirectoryExists(Path.GetDirectoryName(destinationFile));
                File.Copy(sourceFile, destinationFile);
            }

            string[] files = Directory.GetFiles(originalOutputFolderPath, "*", SearchOption.AllDirectories);

            Parallel.ForEach(files, action);
        }

        private static void CopyAndroidFilesAsync(string originalOutputPath, string standardOutputPath, string productName)
        {
            string extension = Path.GetExtension(originalOutputPath);
            string folder = Path.GetDirectoryName(standardOutputPath);

            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            Assert.IsFalse(string.IsNullOrWhiteSpace(folder));
            Directory.CreateDirectory(folder);

            if (string.IsNullOrWhiteSpace(extension))
            {
                const string arm64 = ".arm64-v8a.apk";
                const string armeabi = ".armeabi-v7a.apk";

                string[] sourceFiles =
                {
                    $"{originalOutputPath}/{productName}{arm64}",
                    $"{originalOutputPath}/{productName}{armeabi}",
                };

                string[] destinationFiles =
                {
                    $"{standardOutputPath}{arm64}",
                    $"{standardOutputPath}{armeabi}",
                };

                void action(int i)
                {
                    File.Copy(sourceFiles[i], destinationFiles[i]);
                }

                Parallel.For(0, 2, action);
            }
            else
            {
                File.Copy(originalOutputPath, $"{standardOutputPath}{extension}");
            }
        }
    }
}

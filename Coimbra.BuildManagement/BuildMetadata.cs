using JetBrains.Annotations;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.CrashReportHandler;
using UnityEngine.Scripting;

namespace Coimbra.BuildManagement
{
    /// <summary>
    ///     Holds custom metadata injected during the build.
    /// </summary>
    [Preserve]
    [Serializable]
    public sealed class BuildMetadata
    {
        [SerializeField] private string _buildName;
        [SerializeField] private string _fullVersion;

        internal const string FileName = "Coimbra.BuildManagement.BuildMetadata.json";
        internal readonly string AbsoluteFilePath = Path.Combine(Application.streamingAssetsPath, FileName);
        internal readonly string AssetFilePath = $"Assets/StreamingAssets/{FileName}";

        private BuildMetadata() { }

        /// <summary>
        ///     The build name as chosen in the project settings.
        /// </summary>
        [NotNull]
        public string BuildName => _buildName ?? string.Empty;

        /// <summary>
        ///     The unique version of the build in the format "{bundleVersion}-{buildTime}".
        /// </summary>
        [NotNull]
        public string FullVersion => _fullVersion ?? string.Empty;

        /// <summary>
        ///     Use this to access the custom build metadata.
        /// </summary>
        /// <returns>null if the custom build metadata could not be found.</returns>
        [CanBeNull]
        public static BuildMetadata GetInstance()
        {
            string filePath = $"{Application.streamingAssetsPath}/{FileName}";

            if (!File.Exists(filePath))
            {
                return null;
            }

            BuildMetadata instance = new BuildMetadata();
            JsonUtility.FromJsonOverwrite(File.ReadAllText(filePath), instance);

            return instance;
        }

        internal static BuildMetadata SetInstance(string buildName, string fullVersion)
        {
            BuildMetadata instance = new BuildMetadata
            {
                _buildName = buildName,
                _fullVersion = fullVersion,
            };

            File.WriteAllText(instance.AbsoluteFilePath, JsonUtility.ToJson(instance));

            return instance;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InjectMetadata()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                return;
            }
#endif
            BuildMetadata instance = GetInstance();

            if (instance == null)
            {
                return;
            }

            CrashReportHandler.SetUserMetadata(nameof(BuildName), instance.BuildName);
            CrashReportHandler.SetUserMetadata(nameof(FullVersion), instance.FullVersion);
        }
    }
}

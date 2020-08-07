using System;
using System.IO;
using UnityEngine;
using UnityEngine.CrashReportHandler;
using UnityEngine.Scripting;

namespace Coimbra.BuildManagement
{
    /// <summary>
    /// Holds custom metadata injected during the build.
    /// </summary>
    [Preserve] [Serializable]
    public sealed class BuildMetadata
    {
        internal static readonly string FileName = $"{nameof(Coimbra)}.{nameof(BuildManagement)}.{nameof(BuildMetadata)}.json";

        internal readonly string AbsoluteFilePath = Path.Combine(Application.streamingAssetsPath, FileName);
        internal readonly string AssetFilePath = $"Assets/StreamingAssets/{FileName}";

#pragma warning disable 0649
        [SerializeField] private string m_BuildName;
        [SerializeField] private string m_FullVersion;
#pragma warning restore 0649

        private BuildMetadata() { }

        /// <summary>
        /// The build name as chosen in the project settings.
        /// </summary>
        public string BuildName => m_BuildName;
        /// <summary>
        /// The unique version of the build in the format "{bundleVersion}-{buildTime}".
        /// </summary>
        public string FullVersion => m_FullVersion;

#if !UNITY_EDITOR
#if UNITY_2019_1_OR_NEWER
        [Preserve, RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#else
        [Preserve, RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
#endif
        private static void InjectMetadata()
        {
            BuildMetadata instance = GetInstance();

            if (instance == null)
            {
                return;
            }

            CrashReportHandler.SetUserMetadata(nameof(BuildName), instance.BuildName);
            CrashReportHandler.SetUserMetadata(nameof(FullVersion), instance.FullVersion);
        }

        /// <summary>
        /// Use this to access the custom build metadata.
        /// </summary>
        /// <returns>null if the custom build metadata could not be found.</returns>
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
                m_BuildName = buildName,
                m_FullVersion = fullVersion,
            };

            File.WriteAllText(instance.AbsoluteFilePath, JsonUtility.ToJson(instance));

            return instance;
        }
    }
}

using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra.BuildManagement
{
    /// <summary>
    /// Use this to quick access the <a href="https://docs.unity3d.com/Manual/UnityCloudBuildManifest.html">UnityCloudBuildManifest</a> json.
    /// </summary>
    [Preserve] [Serializable]
    public sealed class UnityCloudBuildManifest
    {
        [SerializeField] private string buildNumber = default;
        [SerializeField] private string buildStartTime = default;
        [SerializeField] private string bundleId = default;
        [SerializeField] private string cloudBuildTargetName = default;
        [SerializeField] private string projectId = default;
        [SerializeField] private string scmBranch = default;
        [SerializeField] private string scmCommitId = default;
        [SerializeField] private string unityVersion = default;
        [SerializeField] private string xcodeVersion = default;

        private UnityCloudBuildManifest() { }

        public string BuildNumber => buildNumber;
        public string BuildStartTime => buildStartTime;

        [PublicAPI] public string BundleId => bundleId;
        [PublicAPI] public string CloudBuildTargetName => cloudBuildTargetName;
        [PublicAPI] public string ProjectId => projectId;
        [PublicAPI] public string ScmBranch => scmBranch;
        [PublicAPI] public string ScmCommitId => scmCommitId;
        [PublicAPI] public string UnityVersion => unityVersion;
        [PublicAPI] public string XcodeVersion => xcodeVersion;

        /// <summary>
        /// Use this to access the UnityCloudBuildManifest.
        /// </summary>
        /// <returns>null if the UnityCloudBuildManifest is not found.</returns>
        [CanBeNull]
        public static UnityCloudBuildManifest GetInstance()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("UnityCloudBuildManifest.json");

            if (textAsset == null)
            {
                return null;
            }

            UnityCloudBuildManifest instance = new UnityCloudBuildManifest();

            JsonUtility.FromJsonOverwrite(textAsset.text, instance);

            return instance;
        }
    }
}

#pragma warning disable 0649

using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra.BuildManagement
{
    /// <summary>
    ///     Use this to quick access the <a href="https://docs.unity3d.com/Manual/UnityCloudBuildManifest.html">UnityCloudBuildManifest</a> json.
    /// </summary>
    [Preserve]
    [Serializable]
    public sealed class UnityCloudBuildManifest
    {
        [SerializeField] private string buildNumber;
        [SerializeField] private string buildStartTime;
        [SerializeField] private string bundleId;
        [SerializeField] private string cloudBuildTargetName;
        [SerializeField] private string projectId;
        [SerializeField] private string scmBranch;
        [SerializeField] private string scmCommitId;
        [SerializeField] private string unityVersion;
        [SerializeField] private string xcodeVersion;

        private UnityCloudBuildManifest() { }

        public string BuildNumber => buildNumber;

        public string BuildStartTime => buildStartTime;

        public string BundleId => bundleId;

        public string CloudBuildTargetName => cloudBuildTargetName;

        public string ProjectId => projectId;

        public string ScmBranch => scmBranch;

        public string ScmCommitId => scmCommitId;

        public string UnityVersion => unityVersion;

        public string XcodeVersion => xcodeVersion;

        /// <summary>
        ///     Use this to access the UnityCloudBuildManifest.
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

#pragma warning restore 0649

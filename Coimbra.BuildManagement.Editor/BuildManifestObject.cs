#if !UNITY_CLOUD_BUILD
using JetBrains.Annotations;
using System.Collections.Generic;

namespace UnityEngine.CloudBuild
{
    internal sealed class BuildManifestObject : ScriptableObject
    {
        [UsedImplicitly]
        public void ClearValues() { }

        [UsedImplicitly]
        public T GetValue<T>(string key)
        {
            return default;
        }

        [UsedImplicitly]
        public void SetValue(string key, object value) { }

        [UsedImplicitly]
        public void SetValues(Dictionary<string, object> sourceDict) { }

        [UsedImplicitly]
        public Dictionary<string, object> ToDictionary()
        {
            return default;
        }

        [UsedImplicitly]
        public string ToJson()
        {
            return default;
        }

        [UsedImplicitly]
        public override string ToString()
        {
            return "";
        }

        [UsedImplicitly]
        public bool TryGetValue<T>(string key, out T result)
        {
            result = default;

            return false;
        }
    }
}
#endif

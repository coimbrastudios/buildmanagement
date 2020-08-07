#if !UNITY_CLOUD_BUILD
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UnityEngine.CloudBuild
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    internal sealed class BuildManifestObject : ScriptableObject
    {
        /// <summary>
        /// Try to get a manifest value - returns true if key was found and could be cast to type T, otherwise returns false.
        /// </summary>
        public bool TryGetValue<T>(string key, out T result)
        {
            result = default;
            return false;
        }

        /// <summary>
        /// Retrieve a manifest value or throw an exception if the given key isn't found.
        /// </summary>
        public T GetValue<T>(string key)
        {
            return default;
        }

        /// <summary>
        /// Set the value for a given key.
        /// </summary>
        public void SetValue(string key, object value) { }

        /// <summary>
        /// Copy values from a dictionary. ToString() will be called on dictionary values before being stored.
        /// </summary>
        public void SetValues(Dictionary<string, object> sourceDict) { }

        /// <summary>
        /// Remove all key/value pairs.
        /// </summary>
        public void ClearValues() { }

        /// <summary>
        /// Return a dictionary that represents the current BuildManifestObject.
        /// </summary>
        public Dictionary<string, object> ToDictionary()
        {
            return default;
        }

        /// <summary>
        /// Return a JSON formatted string that represents the current BuildManifestObject
        /// </summary>
        public string ToJson()
        {
            return default;
        }

        /// <summary>
        /// Return an INI formatted string that represents the current BuildManifestObject
        /// </summary>
        public override string ToString()
        {
            return "";
        }
    }
}
#endif

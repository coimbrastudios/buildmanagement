using System;
using System.IO;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace Coimbra.BuildManagement.Editor.Local
{
    [Serializable]
    internal sealed class UserSettingsFolderRepository : ISettingsRepository
    {
        private const bool PrettyPrintJson = true;
        private const string SettingsDirectory = "UserSettings/Packages";

        [SerializeField] private string _name = default;
        [SerializeField] private string _path = default;
        [SerializeField] private SettingsDictionary _dictionary = new SettingsDictionary();

        private bool _initialized;
        private string _cachedJson;

        internal UserSettingsFolderRepository(string package, string name)
        {
            _name = name;
            _path = GetSettingsPath(package, name);
            _initialized = false;

            AssemblyReloadEvents.beforeAssemblyReload += Save;
            EditorApplication.quitting += Save;
        }

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            if (File.Exists(path))
            {
                _dictionary = null;
                _cachedJson = File.ReadAllText(path);
                EditorJsonUtility.FromJsonOverwrite(_cachedJson, this);

                if (_dictionary == null)
                {
                    _dictionary = new SettingsDictionary();
                }
            }
        }

        public SettingsScope scope => SettingsScope.User;
        public string name => _name;
        public string path => _path;

        public static string GetSettingsPath(string packageName, string name = "Settings")
        {
            return $"{SettingsDirectory}/{packageName}/{name}.json";
        }

        public void Save()
        {
            Initialize();

            if (!File.Exists(path))
            {
                string directory = Path.GetDirectoryName(path);
                Assert.IsFalse(string.IsNullOrWhiteSpace(directory));
                Directory.CreateDirectory(directory);
            }

            string newSettingsJson = EditorJsonUtility.ToJson(this, PrettyPrintJson);
            bool areJsonsEqual = newSettingsJson == _cachedJson;

            if (!AssetDatabase.IsOpenForEdit(path) && areJsonsEqual == false)
            {
                if (!AssetDatabase.MakeEditable(path))
                {
                    Debug.LogWarning($"Could not save package settings to {path}");

                    return;
                }
            }

            try
            {
                if (!areJsonsEqual)
                {
                    File.WriteAllText(path, newSettingsJson);
                    _cachedJson = newSettingsJson;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Debug.LogWarning($"Could not save package settings to {path}");
            }
        }

        public void Set<T>(string key, T value)
        {
            Initialize();
            _dictionary.Set(key, value);
        }

        public T Get<T>(string key, T fallback = default(T))
        {
            Initialize();

            return _dictionary.Get(key, fallback);
        }

        public bool ContainsKey<T>(string key)
        {
            Initialize();

            return _dictionary.ContainsKey<T>(key);
        }

        public void Remove<T>(string key)
        {
            Initialize();
            _dictionary.Remove<T>(key);
        }
    }
}

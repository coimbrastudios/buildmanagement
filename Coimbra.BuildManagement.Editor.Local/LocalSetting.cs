using UnityEditor;
using UnityEditor.SettingsManagement;

namespace Coimbra.BuildManagement.Editor.Local
{
    internal sealed class LocalSetting<T> : UserSetting<T>
    {
        internal LocalSetting(string key, T value)
            : base(LocalSettingsProvider.Settings, key, value, SettingsScope.User) { }
    }
}

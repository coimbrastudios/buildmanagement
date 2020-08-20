using UnityEditor.SettingsManagement;

namespace Coimbra.BuildManagement.Global
{
    internal sealed class GlobalSetting<T> : UserSetting<T>
    {
        internal GlobalSetting(string key, T value)
            : base(GlobalSettingsProvider.Settings, key, value) { }
    }
}

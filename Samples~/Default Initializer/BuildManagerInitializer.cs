using Coimbra.BuildManagement.Editor;
using UnityEditor;

namespace Coimbra.BuildManagement.Samples.Editor.DefaultInitializer
{
    public static class BuildManagerInitializer
    {
#if !UNITY_CLOUD_BUILD
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler.BuildPlayer);
        }
#endif
    }
}

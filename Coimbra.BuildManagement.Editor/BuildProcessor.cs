using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Coimbra.BuildManagement
{
    internal sealed class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            BuildManager.CleanUp();
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            BuildManager.ApplySettings();
        }
    }
}

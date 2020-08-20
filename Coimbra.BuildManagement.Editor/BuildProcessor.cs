using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Coimbra.BuildManagement
{
    internal sealed class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 0;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            BuildManager.ApplySettings();
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            BuildManager.CleanUp();
        }
    }
}

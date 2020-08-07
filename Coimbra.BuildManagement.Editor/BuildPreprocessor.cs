using System.Diagnostics;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Coimbra.BuildManagement.Editor
{
    internal sealed class BuildPreprocessor : IPreprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 0;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            BuildManager.ApplySettings();
        }
    }
}

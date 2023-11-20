using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class EditorPostBuild : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
#if UNITY_WEBGL
        string outputZip = Path.Combine(report.summary.outputPath, "WebGL.zip");
        if (File.Exists(outputZip)) File.Delete(outputZip);
#endif
    }
}

using System.Diagnostics;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class EditorPostBuild : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
#if UNITY_WEBGL
        string outputPath = report.summary.outputPath;
        string pathToBackup = Path.Combine(Directory.GetParent(outputPath).FullName, "Backup");
        CopyFilesRecursively(pathToBackup, outputPath);

        string outputZip = Path.Combine(outputPath, "WebGL.zip");
        if (File.Exists(outputZip)) File.Delete(outputZip);

        Process proc = new();
        ProcessStartInfo startInfo = new()
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            FileName = "cmd",
            Arguments = $"7z a -tzip -ssw -mx5 -r0 \"{outputZip}\" \"{Path.Combine(outputPath, "*")}\" -x!*.zip -x!*.7z -x!*.rar"
        };
        proc.StartInfo = startInfo;
        proc.Start();
#endif
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class EditorPreBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string curVersion = PlayerSettings.bundleVersion.Split(' ')[0];
#if UNITY_WEBGL
        PlayerSettings.bundleVersion = curVersion + " WebGL";
#elif UNITY_ANDROID
        PlayerSettings.bundleVersion = curVersion + " Android";
#elif UNITY_IOS
        PlayerSettings.bundleVersion = curVersion + " IOS";
#elif UNITY_STANDALONE_WIN
        PlayerSettings.bundleVersion = curVersion + " Win";
#elif UNITY_STANDALONE_OSX
        PlayerSettings.bundleVersion = curVersion + " OSX";
#elif UNITY_STANDALONE_LINUX
        PlayerSettings.bundleVersion = curVersion + " Linux";
#endif
    }
}

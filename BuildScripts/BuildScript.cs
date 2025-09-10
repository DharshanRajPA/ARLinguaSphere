using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

namespace BuildScripts
{
    public class BuildScript
    {
        [MenuItem("Build/Build Android")]
        public static void BuildAndroid()
        {
            Debug.Log("Starting Android build...");
            
            // Set build settings
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            
            // Configure player settings
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            
            // Set bundle identifier
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.arlinguasphere.app");
            
            // Set version
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.Android.bundleVersionCode = 1;
            
            // Configure AR settings
            PlayerSettings.Android.ARCoreEnabled = true;
            PlayerSettings.Android.ARCoreRequired = true;
            
            // Set permissions
            PlayerSettings.Android.usesCamera = true;
            PlayerSettings.Android.usesMicrophone = true;
            PlayerSettings.Android.usesLocation = false;
            
            // Build scenes
            string[] scenes = GetBuildScenes();
            
            // Set build path
            string buildPath = "build/Android/ARLinguaSphere.apk";
            Directory.CreateDirectory(Path.GetDirectoryName(buildPath));
            
            // Build
            BuildReport report = BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.Android, BuildOptions.None);
            
            // Check build result
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Android build succeeded: {report.summary.totalSize} bytes");
            }
            else
            {
                Debug.LogError($"Android build failed: {report.summary.result}");
                EditorApplication.Exit(1);
            }
        }
        
        [MenuItem("Build/Build Android AAB")]
        public static void BuildAndroidAAB()
        {
            Debug.Log("Starting Android AAB build...");
            
            // Set build settings for AAB
            EditorUserBuildSettings.buildAppBundle = true;
            
            // Configure player settings (same as APK)
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            
            // Set bundle identifier
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.arlinguasphere.app");
            
            // Set version
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.Android.bundleVersionCode = 1;
            
            // Configure AR settings
            PlayerSettings.Android.ARCoreEnabled = true;
            PlayerSettings.Android.ARCoreRequired = true;
            
            // Set permissions
            PlayerSettings.Android.usesCamera = true;
            PlayerSettings.Android.usesMicrophone = true;
            PlayerSettings.Android.usesLocation = false;
            
            // Build scenes
            string[] scenes = GetBuildScenes();
            
            // Set build path
            string buildPath = "build/Android/ARLinguaSphere.aab";
            Directory.CreateDirectory(Path.GetDirectoryName(buildPath));
            
            // Build
            BuildReport report = BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.Android, BuildOptions.None);
            
            // Check build result
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Android AAB build succeeded: {report.summary.totalSize} bytes");
            }
            else
            {
                Debug.LogError($"Android AAB build failed: {report.summary.result}");
                EditorApplication.Exit(1);
            }
        }
        
        private static string[] GetBuildScenes()
        {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }
            return scenes;
        }
        
        [MenuItem("Build/Clean Build")]
        public static void CleanBuild()
        {
            Debug.Log("Cleaning build directory...");
            
            string buildDir = "build";
            if (Directory.Exists(buildDir))
            {
                Directory.Delete(buildDir, true);
            }
            
            Debug.Log("Build directory cleaned.");
        }
    }
}

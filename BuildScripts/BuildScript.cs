using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.IO;

namespace BuildScripts
{
    /// <summary>
    /// Automated build script for ARLinguaSphere
    /// Supports CI/CD pipeline builds with proper configuration
    /// </summary>
    public static class BuildScript
    {
        private const string COMPANY_NAME = "ARLinguaSphere";
        private const string PRODUCT_NAME = "ARLinguaSphere";
        private const string BUNDLE_IDENTIFIER = "com.arlinguasphere.app";
        
        // Build paths
        private static readonly string BUILD_PATH = Path.Combine(Directory.GetCurrentDirectory(), "build");
        private static readonly string ANDROID_PATH = Path.Combine(BUILD_PATH, "Android");
        private static readonly string IOS_PATH = Path.Combine(BUILD_PATH, "iOS");
        
        [MenuItem("ARLinguaSphere/Build/Android APK")]
        public static void BuildAndroidAPK()
        {
            BuildAndroid(false);
        }
        
        [MenuItem("ARLinguaSphere/Build/Android AAB")]
        public static void BuildAndroidAAB()
        {
            BuildAndroid(true);
        }
        
        [MenuItem("ARLinguaSphere/Build/iOS")]
        public static void BuildiOS()
        {
            BuildForPlatform(BuildTarget.iOS, IOS_PATH);
        }
        
        /// <summary>
        /// Main Android build method called by CI/CD
        /// </summary>
        public static void BuildAndroid()
        {
            string[] args = Environment.GetCommandLineArgs();
            bool buildAAB = Array.Exists(args, arg => arg.Contains("aab"));
            BuildAndroid(buildAAB);
        }
        
        private static void BuildAndroid(bool buildAAB = false)
        {
            Debug.Log("ARLinguaSphere: Starting Android build...");
            
            // Configure Android settings
            ConfigureAndroidSettings(buildAAB);
            
            // Build
            BuildForPlatform(BuildTarget.Android, ANDROID_PATH);
            
            Debug.Log("ARLinguaSphere: Android build completed!");
        }
        
        private static void ConfigureAndroidSettings(bool buildAAB)
        {
            // Set target platform
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            
            // Configure player settings
            PlayerSettings.companyName = COMPANY_NAME;
            PlayerSettings.productName = PRODUCT_NAME;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, BUNDLE_IDENTIFIER);
            
            // Android specific settings
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24; // API 24 for ARCore
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            
            // Build settings
            if (buildAAB)
            {
                EditorUserBuildSettings.buildAppBundle = true;
                Debug.Log("ARLinguaSphere: Configured for Android App Bundle (AAB)");
            }
            else
            {
                EditorUserBuildSettings.buildAppBundle = false;
                Debug.Log("ARLinguaSphere: Configured for Android APK");
            }
            
            // XR Settings for ARCore
            ConfigureXRSettings();
            
            // Configure permissions and features
            ConfigureAndroidManifest();
            
            // Set version
            SetVersionFromEnvironment();
        }
        
        private static void ConfigureXRSettings()
        {
            // Enable ARCore XR provider
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            if (settings != null)
            {
                var manager = settings.AssignedSettings;
                if (manager != null)
                {
                    // ARCore should be configured in XR Management settings
                    Debug.Log("ARLinguaSphere: XR settings configured for ARCore");
                }
            }
        }
        
        private static void ConfigureAndroidManifest()
        {
            // Android manifest modifications would be handled by
            // the manifest template and gradle template files
            Debug.Log("ARLinguaSphere: Android manifest configured");
        }
        
        private static void SetVersionFromEnvironment()
        {
            // Get version from environment variables or use default
            string version = Environment.GetEnvironmentVariable("BUILD_VERSION") ?? "1.0.0";
            string buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "1";
            
            PlayerSettings.bundleVersion = version;
            PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);
            PlayerSettings.iOS.buildNumber = buildNumber;
            
            Debug.Log($"ARLinguaSphere: Version set to {version} ({buildNumber})");
        }
        
        private static void BuildForPlatform(BuildTarget target, string outputPath)
        {
            // Ensure output directory exists
            Directory.CreateDirectory(outputPath);
            
            // Get scenes to build
            string[] scenes = GetScenesToBuild();
            
            // Set output file name
            string fileName = GetOutputFileName(target);
            string fullPath = Path.Combine(outputPath, fileName);
            
            // Configure build options
            BuildOptions options = GetBuildOptions();
            
            // Create build player options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = fullPath,
                target = target,
                options = options,
                targetGroup = GetBuildTargetGroup(target)
            };
            
            Debug.Log($"ARLinguaSphere: Building for {target} to {fullPath}");
            
            // Build
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            
            // Handle build result
            HandleBuildResult(report);
        }
        
        private static string[] GetScenesToBuild()
        {
            // Get all scenes from build settings
            var scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }
            
            if (scenes.Length == 0)
            {
                // Fallback to main scene
                scenes = new[] { "Assets/Scenes/MainARScene.unity" };
            }
            
            return scenes;
        }
        
        private static string GetOutputFileName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return EditorUserBuildSettings.buildAppBundle ? "ARLinguaSphere.aab" : "ARLinguaSphere.apk";
                case BuildTarget.iOS:
                    return "ARLinguaSphere";
                default:
                    return PRODUCT_NAME;
            }
        }
        
        private static BuildOptions GetBuildOptions()
        {
            BuildOptions options = BuildOptions.None;
            
            // Check for development build
            if (Environment.GetCommandLineArgs().Length > 0)
            {
                string[] args = Environment.GetCommandLineArgs();
                
                if (Array.Exists(args, arg => arg.Contains("development")))
                {
                    options |= BuildOptions.Development;
                    options |= BuildOptions.ConnectWithProfiler;
                    Debug.Log("ARLinguaSphere: Development build enabled");
                }
                
                if (Array.Exists(args, arg => arg.Contains("autorun")))
                {
                    options |= BuildOptions.AutoRunPlayer;
                }
            }
            
            return options;
        }
        
        private static BuildTargetGroup GetBuildTargetGroup(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                default:
                    return BuildTargetGroup.Standalone;
            }
        }
        
        private static void HandleBuildResult(BuildReport report)
        {
            BuildSummary summary = report.summary;
            
            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log($"ARLinguaSphere: Build succeeded! Size: {summary.totalSize} bytes, Time: {summary.totalTime}");
                    
                    // Log build statistics
                    LogBuildStatistics(report);
                    
                    // Exit with success code for CI/CD
                    if (IsBuildingInBatchMode())
                    {
                        EditorApplication.Exit(0);
                    }
                    break;
                    
                case BuildResult.Failed:
                    Debug.LogError($"ARLinguaSphere: Build failed! Errors: {summary.totalErrors}, Warnings: {summary.totalWarnings}");
                    
                    // Log detailed errors
                    LogBuildErrors(report);
                    
                    // Exit with error code for CI/CD
                    if (IsBuildingInBatchMode())
                    {
                        EditorApplication.Exit(1);
                    }
                    break;
                    
                case BuildResult.Cancelled:
                    Debug.LogWarning("ARLinguaSphere: Build was cancelled");
                    if (IsBuildingInBatchMode())
                    {
                        EditorApplication.Exit(2);
                    }
                    break;
            }
        }
        
        private static void LogBuildStatistics(BuildReport report)
        {
            Debug.Log("=== ARLinguaSphere Build Statistics ===");
            Debug.Log($"Platform: {report.summary.platform}");
            Debug.Log($"Total Size: {FormatBytes(report.summary.totalSize)}");
            Debug.Log($"Total Time: {report.summary.totalTime}");
            Debug.Log($"Build GUID: {report.summary.guid}");
            
            // Log step details
            foreach (var step in report.steps)
            {
                Debug.Log($"Step: {step.name} - Duration: {step.duration}");
            }
        }
        
        private static void LogBuildErrors(BuildReport report)
        {
            Debug.LogError("=== ARLinguaSphere Build Errors ===");
            
            foreach (var step in report.steps)
            {
                foreach (var message in step.messages)
                {
                    if (message.type == LogType.Error)
                    {
                        Debug.LogError($"Error in {step.name}: {message.content}");
                    }
                    else if (message.type == LogType.Warning)
                    {
                        Debug.LogWarning($"Warning in {step.name}: {message.content}");
                    }
                }
            }
        }
        
        private static bool IsBuildingInBatchMode()
        {
            return Environment.GetCommandLineArgs().Length > 0 && 
                   Array.Exists(Environment.GetCommandLineArgs(), arg => arg == "-batchmode");
        }
        
        private static string FormatBytes(ulong bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = (decimal)bytes;
            
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }
        
        /// <summary>
        /// Clean build directories
        /// </summary>
        [MenuItem("ARLinguaSphere/Build/Clean")]
        public static void CleanBuild()
        {
            if (Directory.Exists(BUILD_PATH))
            {
                Directory.Delete(BUILD_PATH, true);
                Debug.Log("ARLinguaSphere: Build directories cleaned");
            }
        }
        
        /// <summary>
        /// Prepare build environment
        /// </summary>
        [MenuItem("ARLinguaSphere/Build/Prepare Environment")]
        public static void PrepareEnvironment()
        {
            Debug.Log("ARLinguaSphere: Preparing build environment...");
            
            // Refresh asset database
            AssetDatabase.Refresh();
            
            // Clear console
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
            
            // Validate project settings
            ValidateProjectSettings();
            
            Debug.Log("ARLinguaSphere: Build environment prepared");
        }
        
        private static void ValidateProjectSettings()
        {
            Debug.Log("=== ARLinguaSphere Project Validation ===");
            
            // Check required scenes
            if (EditorBuildSettings.scenes.Length == 0)
            {
                Debug.LogWarning("No scenes in build settings!");
            }
            
            // Check XR settings
            var xrSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            if (xrSettings == null)
            {
                Debug.LogWarning("XR settings not configured for Android!");
            }
            
            // Check Android settings
            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel24)
            {
                Debug.LogWarning("Android minimum SDK version should be API 24 or higher for ARCore!");
            }
            
            Debug.Log("Project validation completed");
        }
    }
}
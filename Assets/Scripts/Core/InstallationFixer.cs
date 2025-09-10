using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Fixes common Android installation issues
    /// Run this before building to ensure proper installation
    /// </summary>
    public class InstallationFixer : MonoBehaviour
    {
        [ContextMenu("Fix Installation Issues")]
        public static void FixInstallationIssues()
        {
            Debug.Log("🔧 Fixing Android installation issues...");
            
            // Fix 1: Ensure proper Android settings
            FixAndroidSettings();
            
            // Fix 2: Validate manifest
            ValidateManifest();
            
            // Fix 3: Check package configuration
            ValidatePackageConfiguration();
            
            Debug.Log("✅ Installation fixes applied!");
        }
        
        private static void FixAndroidSettings()
        {
            Debug.Log("📱 Fixing Android settings...");
            
            // Set proper target SDK version
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34;
            Debug.Log("✅ Target SDK set to API Level 34");
            
            // Set minimum SDK version
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            Debug.Log("✅ Minimum SDK set to API Level 29");
            
            // Set proper scripting backend
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            Debug.Log("✅ Scripting backend set to IL2CPP");
            
            // Set target architecture
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            Debug.Log("✅ Target architecture set to ARM64");
            
            // Set proper package name
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.arlinguasphere.app");
            Debug.Log("✅ Package name set to com.arlinguasphere.app");
            
            // Set proper bundle version
            PlayerSettings.Android.bundleVersionCode = 1;
            Debug.Log("✅ Bundle version code set to 1");
        }
        
        private static void ValidateManifest()
        {
            Debug.Log("📋 Validating Android manifest...");
            
            string manifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
            
            if (File.Exists(manifestPath))
            {
                string content = File.ReadAllText(manifestPath);
                
                // Check for required elements
                if (!content.Contains("package=\"com.arlinguasphere.app\""))
                {
                    Debug.LogError("❌ Package name missing in manifest!");
                }
                else
                {
                    Debug.Log("✅ Package name found in manifest");
                }
                
                if (!content.Contains("android.permission.CAMERA"))
                {
                    Debug.LogError("❌ Camera permission missing in manifest!");
                }
                else
                {
                    Debug.Log("✅ Camera permission found in manifest");
                }
                
                if (!content.Contains("android.permission.RECORD_AUDIO"))
                {
                    Debug.LogError("❌ Microphone permission missing in manifest!");
                }
                else
                {
                    Debug.Log("✅ Microphone permission found in manifest");
                }
                
                if (!content.Contains("com.google.ar.core"))
                {
                    Debug.LogError("❌ ARCore meta-data missing in manifest!");
                }
                else
                {
                    Debug.Log("✅ ARCore meta-data found in manifest");
                }
            }
            else
            {
                Debug.LogError("❌ AndroidManifest.xml not found!");
            }
        }
        
        private static void ValidatePackageConfiguration()
        {
            Debug.Log("📦 Validating package configuration...");
            
            // Check if AR Foundation is installed
            try
            {
                var arFoundationAssembly = System.Reflection.Assembly.Load("Unity.XR.ARFoundation");
                if (arFoundationAssembly != null)
                {
                    Debug.Log("✅ AR Foundation package found");
                }
            }
            catch
            {
                Debug.LogError("❌ AR Foundation package not found!");
            }
            
            // Check if ARCore XR Plugin is installed
            try
            {
                var arCoreAssembly = System.Reflection.Assembly.Load("Unity.XR.ARCore");
                if (arCoreAssembly != null)
                {
                    Debug.Log("✅ ARCore XR Plugin found");
                }
            }
            catch
            {
                Debug.LogError("❌ ARCore XR Plugin not found!");
            }
            
            // Check if XR Core Utils is installed
            try
            {
                var xrCoreUtilsAssembly = System.Reflection.Assembly.Load("Unity.XR.CoreUtils");
                if (xrCoreUtilsAssembly != null)
                {
                    Debug.Log("✅ XR Core Utils package found");
                }
            }
            catch
            {
                Debug.LogError("❌ XR Core Utils package not found!");
            }
        }
        
        [ContextMenu("Clear Build Cache")]
        public static void ClearBuildCache()
        {
            Debug.Log("🧹 Clearing build cache...");
            
            // Clear Library folder
            if (Directory.Exists("Library"))
            {
                Directory.Delete("Library", true);
                Debug.Log("✅ Library folder cleared");
            }
            
            // Clear Temp folder
            if (Directory.Exists("Temp"))
            {
                Directory.Delete("Temp", true);
                Debug.Log("✅ Temp folder cleared");
            }
            
            // Clear Build folder
            if (Directory.Exists("Builds"))
            {
                Directory.Delete("Builds", true);
                Debug.Log("✅ Builds folder cleared");
            }
            
            Debug.Log("✅ Build cache cleared!");
        }
        
        [ContextMenu("Validate Installation")]
        public static void ValidateInstallation()
        {
            Debug.Log("🔍 Validating installation configuration...");
            
            bool allGood = true;
            
            // Check Android settings
            if (PlayerSettings.Android.targetSdkVersion != AndroidSdkVersions.AndroidApiLevel34)
            {
                Debug.LogError("❌ Target SDK version is not 34");
                allGood = false;
            }
            
            if (PlayerSettings.Android.minSdkVersion != AndroidSdkVersions.AndroidApiLevel29)
            {
                Debug.LogError("❌ Minimum SDK version is not 29");
                allGood = false;
            }
            
            if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) != ScriptingImplementation.IL2CPP)
            {
                Debug.LogError("❌ Scripting backend is not IL2CPP");
                allGood = false;
            }
            
            if (PlayerSettings.Android.targetArchitectures != AndroidArchitecture.ARM64)
            {
                Debug.LogError("❌ Target architecture is not ARM64");
                allGood = false;
            }
            
            if (PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android) != "com.arlinguasphere.app")
            {
                Debug.LogError("❌ Package name is not com.arlinguasphere.app");
                allGood = false;
            }
            
            if (allGood)
            {
                Debug.Log("✅ All installation settings are correct!");
            }
            else
            {
                Debug.LogError("❌ Installation settings need to be fixed!");
            }
        }
    }
}

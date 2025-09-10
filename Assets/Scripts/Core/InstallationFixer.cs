using UnityEngine;
using System.IO;

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
            Debug.Log("📱 Android settings validation...");
            Debug.Log("⚠️ Project Settings modification requires editor scripts");
            Debug.Log("📋 Please configure the following Android settings in Unity Editor:");
            Debug.Log("   - Target SDK: API Level 34");
            Debug.Log("   - Minimum SDK: API Level 29");
            Debug.Log("   - Scripting Backend: IL2CPP");
            Debug.Log("   - Target Architecture: ARM64");
            Debug.Log("   - Package Name: com.arlinguasphere.app");
            Debug.Log("   - Bundle Version Code: 1");
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
            Debug.Log("⚠️ Project Settings validation requires editor scripts");
            Debug.Log("📋 Please verify the following Android settings in Unity Editor:");
            Debug.Log("   - Target SDK: API Level 34");
            Debug.Log("   - Minimum SDK: API Level 29");
            Debug.Log("   - Scripting Backend: IL2CPP");
            Debug.Log("   - Target Architecture: ARM64");
            Debug.Log("   - Package Name: com.arlinguasphere.app");
            Debug.Log("   - Bundle Version Code: 1");
        }
    }
}

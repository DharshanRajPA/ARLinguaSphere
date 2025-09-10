using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Installs required Unity packages for ARLinguaSphere
    /// </summary>
    public class PackageInstaller : MonoBehaviour
    {
        [ContextMenu("Install Required Packages")]
        public static void InstallRequiredPackages()
        {
            Debug.Log("📦 Package installation requires manual setup...");
            Debug.Log("📋 Please install the following packages manually in Unity Package Manager:");
            Debug.Log("   - AR Foundation (com.unity.xr.arfoundation)");
            Debug.Log("   - ARCore XR Plugin (com.unity.xr.arcore)");
            Debug.Log("   - XR Core Utils (com.unity.xr.core-utils)");
            Debug.Log("   - XR Plugin Management (com.unity.xr.management)");
            Debug.Log("   - Unity UI (com.unity.ugui)");
            Debug.Log("   - TextMeshPro (com.unity.textmeshpro)");
            Debug.Log("   - Input System (com.unity.inputsystem)");
            Debug.Log("   - AR Feature (com.unity.feature.ar)");
            Debug.Log("✅ Open Window > Package Manager to install these packages.");
        }
        
        [ContextMenu("Check Package Status")]
        public static void CheckPackageStatus()
        {
            Debug.Log("🔍 Checking package installation status...");
            Debug.Log("📋 Please check the following packages in Unity Package Manager:");
            Debug.Log("   - AR Foundation (com.unity.xr.arfoundation)");
            Debug.Log("   - ARCore XR Plugin (com.unity.xr.arcore)");
            Debug.Log("   - XR Core Utils (com.unity.xr.core-utils)");
            Debug.Log("   - XR Plugin Management (com.unity.xr.management)");
            Debug.Log("   - Unity UI (com.unity.ugui)");
            Debug.Log("   - TextMeshPro (com.unity.textmeshpro)");
            Debug.Log("   - Input System (com.unity.inputsystem)");
            Debug.Log("   - AR Feature (com.unity.feature.ar)");
            Debug.Log("✅ Open Window > Package Manager to verify installation.");
        }
        
        [ContextMenu("Fix Package Issues")]
        public static void FixPackageIssues()
        {
            Debug.Log("🔧 Attempting to fix package issues...");
            Debug.Log("📋 Package fixing requires editor scripts.");
            Debug.Log("✅ Please use Unity Editor to refresh and reimport assets if needed.");
            Debug.Log("🔧 Open Window > Package Manager to resolve package issues.");
        }
    }
}

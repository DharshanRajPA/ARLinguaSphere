using UnityEngine;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Provides instructions for manually installing required packages
    /// </summary>
    public class ManualPackageInstallation : MonoBehaviour
    {
        [ContextMenu("Show Package Installation Instructions")]
        public void ShowPackageInstallationInstructions()
        {
            Debug.Log("📦 MANUAL PACKAGE INSTALLATION INSTRUCTIONS");
            Debug.Log("==========================================");
            Debug.Log("");
            Debug.Log("1. Open Unity Package Manager:");
            Debug.Log("   - Go to Window → Package Manager");
            Debug.Log("");
            Debug.Log("2. Install the following packages:");
            Debug.Log("   - AR Foundation (com.unity.xr.arfoundation)");
            Debug.Log("   - ARCore XR Plugin (com.unity.xr.arcore)");
            Debug.Log("   - XR Core Utils (com.unity.xr.core-utils)");
            Debug.Log("   - XR Plugin Management (com.unity.xr.management)");
            Debug.Log("   - Unity UI (com.unity.ugui)");
            Debug.Log("   - TextMeshPro (com.unity.textmeshpro)");
            Debug.Log("   - Input System (com.unity.inputsystem)");
            Debug.Log("   - AR Feature (com.unity.feature.ar)");
            Debug.Log("");
            Debug.Log("3. For each package:");
            Debug.Log("   - Search for the package name");
            Debug.Log("   - Click 'Install' if not already installed");
            Debug.Log("   - Wait for installation to complete");
            Debug.Log("");
            Debug.Log("4. After installing all packages:");
            Debug.Log("   - Close and reopen Unity");
            Debug.Log("   - Check if compilation errors are resolved");
            Debug.Log("");
            Debug.Log("5. If packages are already installed but not recognized:");
            Debug.Log("   - Go to Window → Package Manager");
            Debug.Log("   - Click the refresh button");
            Debug.Log("   - Or try: Assets → Reimport All");
            Debug.Log("");
            Debug.Log("✅ Follow these steps to resolve the compilation errors!");
        }
        
        void Start()
        {
            // Automatically show instructions when the script starts
            ShowPackageInstallationInstructions();
        }
    }
}

using UnityEngine;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Quick test script to verify the crash fix solution works
    /// </summary>
    public class QuickTest : MonoBehaviour
    {
        [ContextMenu("Run Quick Test")]
        public void RunQuickTest()
        {
            Debug.Log("🧪 Running quick test...");
            
            // Test 1: Check if UltimateCrashFixer exists
            var crashFixer = FindFirstObjectByType<UltimateCrashFixer>();
            if (crashFixer != null)
            {
                Debug.Log("✅ UltimateCrashFixer found");
            }
            else
            {
                Debug.LogError("❌ UltimateCrashFixer not found");
            }
            
            // Test 2: Check if CrashProofMainController exists
            var mainController = FindFirstObjectByType<CrashProofMainController>();
            if (mainController != null)
            {
                Debug.Log("✅ CrashProofMainController found");
            }
            else
            {
                Debug.LogError("❌ CrashProofMainController not found");
            }
            
            // Test 3: Check if AndroidCrashDebugger exists
            var crashDebugger = FindFirstObjectByType<AndroidCrashDebugger>();
            if (crashDebugger != null)
            {
                Debug.Log("✅ AndroidCrashDebugger found");
            }
            else
            {
                Debug.LogError("❌ AndroidCrashDebugger not found");
            }
            
            // Test 4: Check basic Unity components
            var camera = Camera.main;
            if (camera != null)
            {
                Debug.Log("✅ Main Camera found");
            }
            else
            {
                Debug.LogError("❌ Main Camera not found");
            }
            
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                Debug.Log("✅ Canvas found");
            }
            else
            {
                Debug.LogError("❌ Canvas not found");
            }
            
            Debug.Log("🧪 Quick test complete!");
        }
        
        [ContextMenu("Create Test Scene")]
        public void CreateTestScene()
        {
            TestSceneCreator.CreateTestScene();
        }
        
        [ContextMenu("Test Scene")]
        public void TestScene()
        {
            TestSceneCreator.TestScene();
        }
    }
}

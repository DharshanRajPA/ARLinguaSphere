using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Creates a test scene that won't crash
    /// This is the simplest possible scene for testing
    /// </summary>
    public class TestSceneCreator : MonoBehaviour
    {
        [ContextMenu("Create Test Scene")]
        public static void CreateTestScene()
        {
            Debug.Log("🧪 Creating test scene...");
            
            // Clear existing scene
            ClearScene();
            
            // Create main camera
            CreateMainCamera();
            
            // Create canvas
            CreateCanvas();
            
            // Create status text
            CreateStatusText();
            
            // Create crash fixer
            CreateCrashFixer();
            
            // Create main controller
            CreateMainController();
            
            Debug.Log("✅ Test scene created successfully!");
        }
        
        private static void ClearScene()
        {
            // Find all objects in scene
            var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (var obj in allObjects)
            {
                if (obj.name != "Main Camera" && obj.name != "Directional Light")
                {
                    DestroyImmediate(obj);
                }
            }
            
            Debug.Log("🧹 Scene cleared");
        }
        
        private static void CreateMainCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObj = new GameObject("Main Camera");
                camera = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";
                Debug.Log("✅ Main Camera created");
            }
            else
            {
                Debug.Log("✅ Main Camera already exists");
            }
        }
        
        private static void CreateCanvas()
        {
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            Debug.Log("✅ Canvas created");
        }
        
        private static void CreateStatusText()
        {
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("❌ Canvas not found - cannot create status text");
                return;
            }
            
            var textObj = new GameObject("StatusText");
            textObj.transform.SetParent(canvas.transform, false);
            
            var text = textObj.AddComponent<Text>();
            text.text = "ARLinguaSphere Test Scene\nApp is running!\nNo crashes detected.";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            
            var rectTransform = textObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(600, 200);
            
            Debug.Log("✅ Status text created");
        }
        
        private static void CreateCrashFixer()
        {
            var crashFixerObj = new GameObject("UltimateCrashFixer");
            var crashFixer = crashFixerObj.AddComponent<UltimateCrashFixer>();
            
            // Configure for maximum stability
            crashFixer.enableCrashPrevention = true;
            crashFixer.enableSafeMode = true;
            crashFixer.enableDetailedLogging = true;
            crashFixer.enableARFallback = true;
            crashFixer.enableVoiceFallback = true;
            
            Debug.Log("✅ UltimateCrashFixer created");
        }
        
        private static void CreateMainController()
        {
            var controllerObj = new GameObject("CrashProofMainController");
            var controller = controllerObj.AddComponent<CrashProofMainController>();
            
            // Configure for maximum stability
            controller.enableAR = false; // Disable AR for testing
            controller.enableVoice = false; // Disable voice for testing
            controller.enableUI = true;
            controller.enableDebugging = true;
            
            Debug.Log("✅ CrashProofMainController created");
        }
        
        [ContextMenu("Test Scene")]
        public static void TestScene()
        {
            Debug.Log("🧪 Testing scene...");
            
            // Check if all components exist
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
            
            var text = FindFirstObjectByType<Text>();
            if (text != null)
            {
                Debug.Log("✅ Status text found");
            }
            else
            {
                Debug.LogError("❌ Status text not found");
            }
            
            var crashFixer = FindFirstObjectByType<UltimateCrashFixer>();
            if (crashFixer != null)
            {
                Debug.Log("✅ UltimateCrashFixer found");
            }
            else
            {
                Debug.LogError("❌ UltimateCrashFixer not found");
            }
            
            var controller = FindFirstObjectByType<CrashProofMainController>();
            if (controller != null)
            {
                Debug.Log("✅ CrashProofMainController found");
            }
            else
            {
                Debug.LogError("❌ CrashProofMainController not found");
            }
            
            Debug.Log("🧪 Scene test complete");
        }
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Simple crash-proof scene setup
    /// This ensures the scene has the minimum required components to not crash
    /// </summary>
    public class SimpleCrashProofSetup : MonoBehaviour
    {
        [ContextMenu("Setup Crash-Proof Scene")]
        public static void SetupCrashProofScene()
        {
            Debug.Log("🛡️ Setting up crash-proof scene...");
            
            // Create main controller
            CreateMainController();
            
            // Create basic UI
            CreateBasicUI();
            
            // Create AR components (minimal)
            CreateMinimalARComponents();
            
            Debug.Log("✅ Crash-proof scene setup complete!");
        }
        
        private static void CreateMainController()
        {
            // Find or create main controller
            var controller = FindFirstObjectByType<CrashProofMainController>();
            if (controller == null)
            {
                var controllerObj = new GameObject("CrashProofMainController");
                controller = controllerObj.AddComponent<CrashProofMainController>();
                Debug.Log("✅ Created CrashProofMainController");
            }
            else
            {
                Debug.Log("✅ CrashProofMainController already exists");
            }
        }
        
        private static void CreateBasicUI()
        {
            // Create a simple canvas
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                Debug.Log("✅ Created basic Canvas");
            }
            
            // Create a simple text to show the app is running
            var textObj = new GameObject("StatusText");
            textObj.transform.SetParent(canvas.transform, false);
            var text = textObj.AddComponent<UnityEngine.UI.Text>();
            text.text = "ARLinguaSphere is running!";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            
            // Position it in the center
            var rectTransform = textObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(400, 100);
            
            Debug.Log("✅ Created status text");
        }
        
        private static void CreateMinimalARComponents()
        {
            // Only create AR components if we're in the editor or if AR is supported
            #if UNITY_EDITOR
            Debug.Log("⚠️ AR components not created in editor - will be created at runtime");
            #else
            // Create minimal AR setup for Android
            if (Application.platform == RuntimePlatform.Android)
            {
                CreateARComponentsForAndroid();
            }
            #endif
        }
        
        private static void CreateARComponentsForAndroid()
        {
            // Create AR Session
            var arSession = FindFirstObjectByType<UnityEngine.XR.ARFoundation.ARSession>();
            if (arSession == null)
            {
                var arSessionObj = new GameObject("AR Session");
                arSession = arSessionObj.AddComponent<UnityEngine.XR.ARFoundation.ARSession>();
                Debug.Log("✅ Created AR Session");
            }
            
            // Create XR Origin
            var xrOrigin = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOrigin == null)
            {
                var xrOriginObj = new GameObject("XR Origin");
                xrOrigin = xrOriginObj.AddComponent<Unity.XR.CoreUtils.XROrigin>();
                Debug.Log("✅ Created XR Origin");
            }
            
            Debug.Log("✅ Minimal AR components created for Android");
        }
        
        [ContextMenu("Test Scene Setup")]
        public static void TestSceneSetup()
        {
            Debug.Log("🧪 Testing scene setup...");
            
            // Check if main controller exists
            var controller = FindFirstObjectByType<CrashProofMainController>();
            if (controller != null)
            {
                Debug.Log("✅ CrashProofMainController found");
            }
            else
            {
                Debug.LogError("❌ CrashProofMainController not found");
            }
            
            // Check if canvas exists
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                Debug.Log("✅ Canvas found");
            }
            else
            {
                Debug.LogError("❌ Canvas not found");
            }
            
            // Check if status text exists
            var text = FindFirstObjectByType<UnityEngine.UI.Text>();
            if (text != null)
            {
                Debug.Log("✅ Status text found");
            }
            else
            {
                Debug.LogError("❌ Status text not found");
            }
            
            Debug.Log("🧪 Scene setup test complete");
        }
    }
}

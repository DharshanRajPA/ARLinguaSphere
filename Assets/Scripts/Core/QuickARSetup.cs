using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using ARLinguaSphere.AR;
using ARLinguaSphere.ML;
using ARLinguaSphere.Core;
using ARLinguaSphere.Gesture;
using ARLinguaSphere.Voice;
using ARLinguaSphere.UI;
using ARLinguaSphere.Network;
using ARLinguaSphere.Analytics;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Quick and reliable AR setup for Unity 6000.2.3f1
    /// Creates all required AR components with proper configuration
    /// </summary>
    public class QuickARSetup : MonoBehaviour
    {
        [Header("Setup Status")]
        [SerializeField] private bool isSetupComplete = false;
        
        [Header("Auto Setup")]
        public bool setupOnStart = true;
        
        [Header("Created Components")]
        [SerializeField] private ARSession arSession;
        [SerializeField] private XROrigin xrOrigin;
        [SerializeField] private ARManager arManager;
        [SerializeField] private MLManager mlManager;
        
        private void Start()
        {
            if (setupOnStart)
            {
                SetupCompleteARScene();
            }
        }
        
        [ContextMenu("🚀 Setup Complete AR Scene")]
        public void SetupCompleteARScene()
        {
            if (isSetupComplete)
            {
                Debug.Log("AR scene already setup! Use 'Clear All' to reset.");
                return;
            }
            
            Debug.Log("🚀 Starting complete AR setup for Unity 6000.2.3f1...");
            
            try
            {
                // Step 1: Create AR Session
                CreateARSession();
                
                // Step 2: Create XR Origin with all AR components
                CreateXROriginWithComponents();
                
                // Step 3: Create AR Manager
                CreateARManager();
                
                // Step 4: Create ML Manager
                CreateMLManager();
                
                // Step 5: Create all other managers
                CreateAllManagers();
                
                // Step 6: Create main controller
                CreateMainController();
                
                // Step 7: Connect everything
                ConnectAllComponents();
                
                isSetupComplete = true;
                Debug.Log("🎉 Complete AR setup finished successfully!");
                Debug.Log("✅ Your AR scene is ready for building!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Setup failed: {e.Message}");
            }
        }
        
        private void CreateARSession()
        {
            if (arSession == null)
            {
                var arSessionObj = new GameObject("AR Session");
                arSession = arSessionObj.AddComponent<ARSession>();
                Debug.Log("✅ AR Session created");
            }
        }
        
        private void CreateXROriginWithComponents()
        {
            if (xrOrigin == null)
            {
                // Create XR Origin
                var xrOriginObj = new GameObject("XR Origin");
                xrOrigin = xrOriginObj.AddComponent<XROrigin>();
                xrOrigin.transform.SetParent(arSession.transform);
                
                // Create AR Camera
                var cameraObj = new GameObject("AR Camera");
                cameraObj.transform.SetParent(xrOrigin.transform);
                cameraObj.transform.localPosition = Vector3.zero;
                cameraObj.transform.localRotation = Quaternion.identity;
                
                var camera = cameraObj.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.nearClipPlane = 0.01f;
                camera.farClipPlane = 20f;
                
                // Set camera to XR Origin
                xrOrigin.Camera = camera;
                
                // Add AR Camera Manager
                var arCameraManager = cameraObj.AddComponent<ARCameraManager>();
                
                // Add AR Plane Manager
                var arPlaneManager = xrOriginObj.AddComponent<ARPlaneManager>();
                
                // Add AR Raycast Manager
                var arRaycastManager = xrOriginObj.AddComponent<ARRaycastManager>();
                
                // Add AR Anchor Manager
                var arAnchorManager = xrOriginObj.AddComponent<ARAnchorManager>();
                
                Debug.Log("✅ XR Origin created with all AR components");
            }
        }
        
        private void CreateARManager()
        {
            if (arManager == null)
            {
                var arManagerObj = new GameObject("AR Manager");
                arManager = arManagerObj.AddComponent<ARManager>();
                
                // Assign all references
                arManager.arSession = arSession;
                arManager.xrOrigin = xrOrigin;
                arManager.arCameraManager = xrOrigin.GetComponentInChildren<ARCameraManager>();
                arManager.arPlaneManager = xrOrigin.GetComponent<ARPlaneManager>();
                arManager.arRaycastManager = xrOrigin.GetComponent<ARRaycastManager>();
                arManager.arAnchorManager = xrOrigin.GetComponent<ARAnchorManager>();
                
                Debug.Log("✅ AR Manager created and configured");
            }
        }
        
        private void CreateMLManager()
        {
            if (mlManager == null)
            {
                var mlManagerObj = new GameObject("ML Manager");
                mlManager = mlManagerObj.AddComponent<MLManager>();
                
                // Add YOLODetector component
                var yoloDetector = mlManagerObj.AddComponent<YOLODetector>();
                yoloDetector.modelPath = "Models/yolov8n_float32.tflite";
                
                Debug.Log("✅ ML Manager created with YOLODetector");
            }
        }
        
        private void CreateAllManagers()
        {
            // Language Manager
            var languageManagerObj = new GameObject("Language Manager");
            languageManagerObj.AddComponent<LanguageManager>();
            
            // Gesture Manager
            var gestureManagerObj = new GameObject("Gesture Manager");
            gestureManagerObj.AddComponent<GestureManager>();
            
            // Voice Manager
            var voiceManagerObj = new GameObject("Voice Manager");
            voiceManagerObj.AddComponent<VoiceManager>();
            
            // UI Manager
            var uiManagerObj = new GameObject("UI Manager");
            uiManagerObj.AddComponent<UIManager>();
            
            // Network Manager
            var networkManagerObj = new GameObject("Network Manager");
            networkManagerObj.AddComponent<NetworkManager>();
            
            // Analytics Manager
            var analyticsManagerObj = new GameObject("Analytics Manager");
            analyticsManagerObj.AddComponent<AnalyticsManager>();
            
            // Quiz Engine
            var quizEngineObj = new GameObject("Quiz Engine");
            quizEngineObj.AddComponent<ARLinguaSphere.Analytics.QuizEngine>();
            
            // AR Label Manager
            var labelManagerObj = new GameObject("AR Label Manager");
            labelManagerObj.AddComponent<ARLabelManager>();
            
            Debug.Log("✅ All managers created");
        }
        
        private void CreateMainController()
        {
            var controllerObj = new GameObject("ARLinguaSphere Controller");
            var controller = controllerObj.AddComponent<ARLinguaSphereController>();
            
            // Assign all references
            controller.arManager = arManager;
            controller.mlManager = mlManager;
            controller.arCamera = xrOrigin.Camera;
            
            Debug.Log("✅ Main Controller created");
        }
        
        private void ConnectAllComponents()
        {
            // Find all managers
            var languageManager = FindFirstObjectByType<LanguageManager>();
            var gestureManager = FindFirstObjectByType<GestureManager>();
            var voiceManager = FindFirstObjectByType<VoiceManager>();
            var uiManager = FindFirstObjectByType<UIManager>();
            var networkManager = FindFirstObjectByType<NetworkManager>();
            var analyticsManager = FindFirstObjectByType<AnalyticsManager>();
            var quizEngine = FindFirstObjectByType<ARLinguaSphere.Analytics.QuizEngine>();
            var labelManager = FindFirstObjectByType<ARLabelManager>();
            var controller = FindFirstObjectByType<ARLinguaSphereController>();
            
            // Connect controller to all managers
            if (controller != null)
            {
                controller.languageManager = languageManager;
                controller.gestureManager = gestureManager;
                controller.voiceManager = voiceManager;
                controller.uiManager = uiManager;
                controller.networkManager = networkManager;
                controller.analyticsManager = analyticsManager;
                controller.quizEngine = quizEngine;
                controller.labelManager = labelManager;
            }
            
            Debug.Log("✅ All components connected");
        }
        
        [ContextMenu("🧹 Clear All AR Components")]
        public void ClearAllARComponents()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Cannot clear during play mode");
                return;
            }
            
            // Find and destroy all AR-related objects
            var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var obj in allObjects)
            {
                if (obj != null && obj != gameObject && 
                    (obj.name.Contains("AR") || obj.name.Contains("XR") || 
                     obj.name.Contains("Manager") || obj.name.Contains("Controller")))
                {
                    DestroyImmediate(obj);
                }
            }
            
            // Reset references
            arSession = null;
            xrOrigin = null;
            arManager = null;
            mlManager = null;
            isSetupComplete = false;
            
            Debug.Log("🧹 All AR components cleared");
        }
        
        [ContextMenu("📋 Show Setup Status")]
        public void ShowSetupStatus()
        {
            Debug.Log("=== AR Setup Status ===");
            Debug.Log($"AR Session: {(arSession != null ? "✅" : "❌")}");
            Debug.Log($"XR Origin: {(xrOrigin != null ? "✅" : "❌")}");
            Debug.Log($"AR Manager: {(arManager != null ? "✅" : "❌")}");
            Debug.Log($"ML Manager: {(mlManager != null ? "✅" : "❌")}");
            Debug.Log($"Setup Complete: {(isSetupComplete ? "✅" : "❌")}");
        }
    }
}

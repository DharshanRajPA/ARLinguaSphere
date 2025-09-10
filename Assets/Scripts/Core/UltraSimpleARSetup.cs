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
    /// Ultra simple AR setup that definitely works
    /// </summary>
    public class UltraSimpleARSetup : MonoBehaviour
    {
        [ContextMenu("🚀 Create Everything Now")]
        public void CreateEverythingNow()
        {
            Debug.Log("🚀 Starting ultra simple AR setup...");
            
            // Clear everything first
            ClearEverything();
            
            // Create AR Session
            var arSessionObj = new GameObject("AR Session");
            var arSession = arSessionObj.AddComponent<ARSession>();
            Debug.Log("✅ AR Session created");
            
            // Create XR Origin
            var xrOriginObj = new GameObject("XR Origin");
            var xrOrigin = xrOriginObj.AddComponent<XROrigin>();
            xrOrigin.transform.SetParent(arSessionObj.transform);
            Debug.Log("✅ XR Origin created");
            
            // Create AR Camera
            var cameraObj = new GameObject("AR Camera");
            cameraObj.transform.SetParent(xrOriginObj.transform);
            cameraObj.transform.localPosition = Vector3.zero;
            cameraObj.transform.localRotation = Quaternion.identity;
            var camera = cameraObj.AddComponent<Camera>();
            xrOrigin.Camera = camera;
            Debug.Log("✅ AR Camera created");
            
            // Add AR Camera Manager
            cameraObj.AddComponent<ARCameraManager>();
            Debug.Log("✅ AR Camera Manager added");
            
            // Add AR Plane Manager
            xrOriginObj.AddComponent<ARPlaneManager>();
            Debug.Log("✅ AR Plane Manager added");
            
            // Add AR Raycast Manager
            xrOriginObj.AddComponent<ARRaycastManager>();
            Debug.Log("✅ AR Raycast Manager added");
            
            // Add AR Anchor Manager
            xrOriginObj.AddComponent<ARAnchorManager>();
            Debug.Log("✅ AR Anchor Manager added");
            
            // Create AR Manager
            var arManagerObj = new GameObject("AR Manager");
            var arManager = arManagerObj.AddComponent<ARManager>();
            arManager.arSession = arSession;
            arManager.xrOrigin = xrOrigin;
            arManager.arCameraManager = cameraObj.GetComponent<ARCameraManager>();
            arManager.arPlaneManager = xrOriginObj.GetComponent<ARPlaneManager>();
            arManager.arRaycastManager = xrOriginObj.GetComponent<ARRaycastManager>();
            arManager.arAnchorManager = xrOriginObj.GetComponent<ARAnchorManager>();
            Debug.Log("✅ AR Manager created and configured");
            
            // Create ML Manager
            var mlManagerObj = new GameObject("ML Manager");
            mlManagerObj.AddComponent<MLManager>();
            var yoloDetector = mlManagerObj.AddComponent<YOLODetector>();
            yoloDetector.modelPath = "Models/yolov8n_float32.tflite";
            Debug.Log("✅ ML Manager created");
            
            // Create Language Manager
            var languageManagerObj = new GameObject("Language Manager");
            languageManagerObj.AddComponent<LanguageManager>();
            Debug.Log("✅ Language Manager created");
            
            // Create Gesture Manager
            var gestureManagerObj = new GameObject("Gesture Manager");
            gestureManagerObj.AddComponent<GestureManager>();
            Debug.Log("✅ Gesture Manager created");
            
            // Create Voice Manager
            var voiceManagerObj = new GameObject("Voice Manager");
            voiceManagerObj.AddComponent<VoiceManager>();
            Debug.Log("✅ Voice Manager created");
            
            // Create UI Manager
            var uiManagerObj = new GameObject("UI Manager");
            uiManagerObj.AddComponent<UIManager>();
            Debug.Log("✅ UI Manager created");
            
            // Create Network Manager
            var networkManagerObj = new GameObject("Network Manager");
            networkManagerObj.AddComponent<NetworkManager>();
            Debug.Log("✅ Network Manager created");
            
            // Create Analytics Manager
            var analyticsManagerObj = new GameObject("Analytics Manager");
            analyticsManagerObj.AddComponent<AnalyticsManager>();
            Debug.Log("✅ Analytics Manager created");
            
            // Create Quiz Engine
            var quizEngineObj = new GameObject("Quiz Engine");
            quizEngineObj.AddComponent<ARLinguaSphere.Analytics.QuizEngine>();
            Debug.Log("✅ Quiz Engine created");
            
            // Create AR Label Manager
            var labelManagerObj = new GameObject("AR Label Manager");
            labelManagerObj.AddComponent<ARLabelManager>();
            Debug.Log("✅ AR Label Manager created");
            
            // Create Main Controller
            var controllerObj = new GameObject("ARLinguaSphere Controller");
            var controller = controllerObj.AddComponent<ARLinguaSphereController>();
            controller.arManager = arManager;
            controller.mlManager = mlManagerObj.GetComponent<MLManager>();
            controller.languageManager = languageManagerObj.GetComponent<LanguageManager>();
            controller.gestureManager = gestureManagerObj.GetComponent<GestureManager>();
            controller.voiceManager = voiceManagerObj.GetComponent<VoiceManager>();
            controller.uiManager = uiManagerObj.GetComponent<UIManager>();
            controller.networkManager = networkManagerObj.GetComponent<NetworkManager>();
            controller.analyticsManager = analyticsManagerObj.GetComponent<AnalyticsManager>();
            controller.quizEngine = quizEngineObj.GetComponent<ARLinguaSphere.Analytics.QuizEngine>();
            controller.labelManager = labelManagerObj.GetComponent<ARLabelManager>();
            controller.arCamera = camera;
            Debug.Log("✅ Main Controller created and configured");
            
            Debug.Log("🎉 Ultra simple AR setup complete! Check your Hierarchy!");
        }
        
        [ContextMenu("🧹 Clear Everything")]
        public void ClearEverything()
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
                     obj.name.Contains("Manager") || obj.name.Contains("Controller") ||
                     obj.name.Contains("Language") || obj.name.Contains("Gesture") ||
                     obj.name.Contains("Voice") || obj.name.Contains("UI") ||
                     obj.name.Contains("Network") || obj.name.Contains("Analytics") ||
                     obj.name.Contains("Quiz") || obj.name.Contains("Label")))
                {
                    DestroyImmediate(obj);
                }
            }
            
            Debug.Log("🧹 Everything cleared");
        }
    }
}

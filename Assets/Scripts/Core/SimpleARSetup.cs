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
    /// Simple AR setup script that creates all required components manually
    /// </summary>
    public class SimpleARSetup : MonoBehaviour
    {
        [Header("Setup Status")]
        public bool isSetupComplete = false;
        
        [Header("Created Objects")]
        public GameObject arSession;
        public GameObject xrOrigin;
        public GameObject arManager;
        public GameObject mlManager;
        public GameObject languageManager;
        public GameObject gestureManager;
        public GameObject voiceManager;
        public GameObject uiManager;
        public GameObject networkManager;
        public GameObject analyticsManager;
        public GameObject quizEngine;
        public GameObject labelManager;
        public GameObject controller;
        
        [ContextMenu("Create AR Session")]
        public void CreateARSession()
        {
            if (arSession == null)
            {
                arSession = new GameObject("AR Session");
                arSession.AddComponent<ARSession>();
                Debug.Log("✅ AR Session created");
            }
        }
        
        [ContextMenu("Create XR Origin")]
        public void CreateXROrigin()
        {
            if (xrOrigin == null)
            {
                xrOrigin = new GameObject("XR Origin");
                xrOrigin.AddComponent<XROrigin>();
                xrOrigin.transform.SetParent(arSession.transform);
                
                // Create AR Camera
                var cameraObj = new GameObject("AR Camera");
                cameraObj.transform.SetParent(xrOrigin.transform);
                cameraObj.transform.localPosition = Vector3.zero;
                cameraObj.transform.localRotation = Quaternion.identity;
                var camera = cameraObj.AddComponent<Camera>();
                xrOrigin.GetComponent<XROrigin>().Camera = camera;
                
                // Add AR Camera Manager
                cameraObj.AddComponent<ARCameraManager>();
                
                // Add AR Plane Manager
                xrOrigin.AddComponent<ARPlaneManager>();
                
                // Add AR Raycast Manager
                xrOrigin.AddComponent<ARRaycastManager>();
                
                // Add AR Anchor Manager
                xrOrigin.AddComponent<ARAnchorManager>();
                
                Debug.Log("✅ XR Origin created with AR components");
            }
        }
        
        [ContextMenu("Create AR Manager")]
        public void CreateARManager()
        {
            if (arManager == null)
            {
                arManager = new GameObject("AR Manager");
                var arManagerComp = arManager.AddComponent<ARManager>();
                
                // Assign references
                arManagerComp.arSession = arSession.GetComponent<ARSession>();
                arManagerComp.xrOrigin = xrOrigin.GetComponent<XROrigin>();
                arManagerComp.arCameraManager = xrOrigin.GetComponentInChildren<ARCameraManager>();
                arManagerComp.arPlaneManager = xrOrigin.GetComponent<ARPlaneManager>();
                arManagerComp.arRaycastManager = xrOrigin.GetComponent<ARRaycastManager>();
                arManagerComp.arAnchorManager = xrOrigin.GetComponent<ARAnchorManager>();
                
                Debug.Log("✅ AR Manager created and configured");
            }
        }
        
        [ContextMenu("Create ML Manager")]
        public void CreateMLManager()
        {
            if (mlManager == null)
            {
                mlManager = new GameObject("ML Manager");
                mlManager.AddComponent<MLManager>();
                mlManager.AddComponent<YOLODetector>();
                
                // Set model path
                var yoloDetector = mlManager.GetComponent<YOLODetector>();
                yoloDetector.modelPath = "Models/yolov8n_float32.tflite";
                
                Debug.Log("✅ ML Manager created with YOLODetector");
            }
        }
        
        [ContextMenu("Create Language Manager")]
        public void CreateLanguageManager()
        {
            if (languageManager == null)
            {
                languageManager = new GameObject("Language Manager");
                languageManager.AddComponent<LanguageManager>();
                Debug.Log("✅ Language Manager created");
            }
        }
        
        [ContextMenu("Create Gesture Manager")]
        public void CreateGestureManager()
        {
            if (gestureManager == null)
            {
                gestureManager = new GameObject("Gesture Manager");
                gestureManager.AddComponent<GestureManager>();
                Debug.Log("✅ Gesture Manager created");
            }
        }
        
        [ContextMenu("Create Voice Manager")]
        public void CreateVoiceManager()
        {
            if (voiceManager == null)
            {
                voiceManager = new GameObject("Voice Manager");
                voiceManager.AddComponent<VoiceManager>();
                Debug.Log("✅ Voice Manager created");
            }
        }
        
        [ContextMenu("Create UI Manager")]
        public void CreateUIManager()
        {
            if (uiManager == null)
            {
                uiManager = new GameObject("UI Manager");
                uiManager.AddComponent<UIManager>();
                Debug.Log("✅ UI Manager created");
            }
        }
        
        [ContextMenu("Create Network Manager")]
        public void CreateNetworkManager()
        {
            if (networkManager == null)
            {
                networkManager = new GameObject("Network Manager");
                networkManager.AddComponent<NetworkManager>();
                Debug.Log("✅ Network Manager created");
            }
        }
        
        [ContextMenu("Create Analytics Manager")]
        public void CreateAnalyticsManager()
        {
            if (analyticsManager == null)
            {
                analyticsManager = new GameObject("Analytics Manager");
                analyticsManager.AddComponent<AnalyticsManager>();
                Debug.Log("✅ Analytics Manager created");
            }
        }
        
        [ContextMenu("Create Quiz Engine")]
        public void CreateQuizEngine()
        {
            if (quizEngine == null)
            {
                quizEngine = new GameObject("Quiz Engine");
                quizEngine.AddComponent<ARLinguaSphere.Analytics.QuizEngine>();
                Debug.Log("✅ Quiz Engine created");
            }
        }
        
        [ContextMenu("Create AR Label Manager")]
        public void CreateARLabelManager()
        {
            if (labelManager == null)
            {
                labelManager = new GameObject("AR Label Manager");
                labelManager.AddComponent<ARLabelManager>();
                Debug.Log("✅ AR Label Manager created");
            }
        }
        
        [ContextMenu("Create Main Controller")]
        public void CreateMainController()
        {
            if (controller == null)
            {
                controller = new GameObject("ARLinguaSphere Controller");
                var controllerComp = controller.AddComponent<ARLinguaSphereController>();
                
                // Assign all references
                controllerComp.arManager = arManager.GetComponent<ARManager>();
                controllerComp.mlManager = mlManager.GetComponent<MLManager>();
                controllerComp.languageManager = languageManager.GetComponent<LanguageManager>();
                controllerComp.gestureManager = gestureManager.GetComponent<GestureManager>();
                controllerComp.voiceManager = voiceManager.GetComponent<VoiceManager>();
                controllerComp.uiManager = uiManager.GetComponent<UIManager>();
                controllerComp.networkManager = networkManager.GetComponent<NetworkManager>();
                controllerComp.analyticsManager = analyticsManager.GetComponent<AnalyticsManager>();
                controllerComp.quizEngine = quizEngine.GetComponent<ARLinguaSphere.Analytics.QuizEngine>();
                controllerComp.labelManager = labelManager.GetComponent<ARLabelManager>();
                controllerComp.arCamera = xrOrigin.GetComponent<XROrigin>().Camera;
                
                Debug.Log("✅ Main Controller created and configured");
            }
        }
        
        [ContextMenu("Setup Everything")]
        public void SetupEverything()
        {
            Debug.Log("🚀 Starting complete AR setup...");
            
            CreateARSession();
            CreateXROrigin();
            CreateARManager();
            CreateMLManager();
            CreateLanguageManager();
            CreateGestureManager();
            CreateVoiceManager();
            CreateUIManager();
            CreateNetworkManager();
            CreateAnalyticsManager();
            CreateQuizEngine();
            CreateARLabelManager();
            CreateMainController();
            
            isSetupComplete = true;
            Debug.Log("🎉 Complete AR setup finished! Check your Hierarchy window.");
        }
        
        [ContextMenu("Clear All")]
        public void ClearAll()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Cannot clear during play mode");
                return;
            }
            
            // Destroy all created objects
            if (arSession != null) DestroyImmediate(arSession);
            if (xrOrigin != null) DestroyImmediate(xrOrigin);
            if (arManager != null) DestroyImmediate(arManager);
            if (mlManager != null) DestroyImmediate(mlManager);
            if (languageManager != null) DestroyImmediate(languageManager);
            if (gestureManager != null) DestroyImmediate(gestureManager);
            if (voiceManager != null) DestroyImmediate(voiceManager);
            if (uiManager != null) DestroyImmediate(uiManager);
            if (networkManager != null) DestroyImmediate(networkManager);
            if (analyticsManager != null) DestroyImmediate(analyticsManager);
            if (quizEngine != null) DestroyImmediate(quizEngine);
            if (labelManager != null) DestroyImmediate(labelManager);
            if (controller != null) DestroyImmediate(controller);
            
            // Reset references
            arSession = null;
            xrOrigin = null;
            arManager = null;
            mlManager = null;
            languageManager = null;
            gestureManager = null;
            voiceManager = null;
            uiManager = null;
            networkManager = null;
            analyticsManager = null;
            quizEngine = null;
            labelManager = null;
            controller = null;
            
            isSetupComplete = false;
            Debug.Log("🧹 All AR components cleared");
        }
    }
}

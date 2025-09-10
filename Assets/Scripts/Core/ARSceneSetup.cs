using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR;
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
    /// Helper script to set up a clean AR scene with all required components
    /// </summary>
    public class ARSceneSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        public bool autoSetupOnStart = true;
        public bool createMissingComponents = true;
        
        [Header("AR Components")]
        public ARSession arSession;
        public XROrigin xrOrigin;
        public ARCameraManager arCameraManager;
        public ARPlaneManager arPlaneManager;
        public ARRaycastManager arRaycastManager;
        public ARAnchorManager arAnchorManager;
        
        [Header("Managers")]
        public ARManager arManager;
        public MLManager mlManager;
        public LanguageManager languageManager;
        public GestureManager gestureManager;
        public VoiceManager voiceManager;
        public UIManager uiManager;
        public NetworkManager networkManager;
        public AnalyticsManager analyticsManager;
        public ARLinguaSphere.Analytics.QuizEngine quizEngine;
        public ARLabelManager labelManager;
        public ARLinguaSphereController controller;
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupARScene();
            }
        }
        
        [ContextMenu("Setup AR Scene")]
        public void SetupARScene()
        {
            Debug.Log("ARSceneSetup: Setting up AR scene...");
            
            // Create AR Session
            SetupARSession();
            
            // Create XR Origin with AR components
            SetupXROrigin();
            
            // Create managers
            SetupManagers();
            
            // Create UI
            SetupUI();
            
            // Connect everything
            ConnectComponents();
            
            Debug.Log("ARSceneSetup: AR scene setup complete!");
        }
        
        private void SetupARSession()
        {
            if (arSession == null)
            {
                var arSessionObj = GameObject.Find("AR Session");
                if (arSessionObj == null)
                {
                    arSessionObj = new GameObject("AR Session");
                }
                arSession = arSessionObj.GetComponent<ARSession>();
                if (arSession == null)
                {
                    arSession = arSessionObj.AddComponent<ARSession>();
                }
            }
        }
        
        private void SetupXROrigin()
        {
            if (xrOrigin == null)
            {
                var xrOriginObj = GameObject.Find("XR Origin");
                if (xrOriginObj == null)
                {
                    xrOriginObj = new GameObject("XR Origin");
                    xrOriginObj.transform.SetParent(arSession.transform);
                }
                xrOrigin = xrOriginObj.GetComponent<XROrigin>();
                if (xrOrigin == null)
                {
                    xrOrigin = xrOriginObj.AddComponent<XROrigin>();
                }
            }
            
            // Setup AR Camera
            SetupARCamera();
            
            // Add AR managers to XR Origin
            SetupARManagers();
        }
        
        private void SetupARCamera()
        {
            var arCamera = xrOrigin.Camera;
            if (arCamera == null)
            {
                var cameraObj = new GameObject("AR Camera");
                cameraObj.transform.SetParent(xrOrigin.transform);
                cameraObj.transform.localPosition = Vector3.zero;
                cameraObj.transform.localRotation = Quaternion.identity;
                arCamera = cameraObj.AddComponent<Camera>();
                xrOrigin.Camera = arCamera;
            }
            
            // Add AR Camera Manager
            if (arCameraManager == null)
            {
                arCameraManager = arCamera.GetComponent<ARCameraManager>();
                if (arCameraManager == null)
                {
                    arCameraManager = arCamera.gameObject.AddComponent<ARCameraManager>();
                }
            }
        }
        
        private void SetupARManagers()
        {
            // AR Plane Manager
            if (arPlaneManager == null)
            {
                arPlaneManager = xrOrigin.GetComponent<ARPlaneManager>();
                if (arPlaneManager == null)
                {
                    arPlaneManager = xrOrigin.gameObject.AddComponent<ARPlaneManager>();
                }
            }
            
            // AR Raycast Manager
            if (arRaycastManager == null)
            {
                arRaycastManager = xrOrigin.GetComponent<ARRaycastManager>();
                if (arRaycastManager == null)
                {
                    arRaycastManager = xrOrigin.gameObject.AddComponent<ARRaycastManager>();
                }
            }
            
            // AR Anchor Manager
            if (arAnchorManager == null)
            {
                arAnchorManager = xrOrigin.GetComponent<ARAnchorManager>();
                if (arAnchorManager == null)
                {
                    arAnchorManager = xrOrigin.gameObject.AddComponent<ARAnchorManager>();
                }
            }
        }
        
        private void SetupManagers()
        {
            // AR Manager
            if (arManager == null && createMissingComponents)
            {
                var arManagerObj = new GameObject("AR Manager");
                arManager = arManagerObj.AddComponent<ARManager>();
            }
            
            // ML Manager
            if (mlManager == null && createMissingComponents)
            {
                var mlManagerObj = new GameObject("ML Manager");
                mlManager = mlManagerObj.AddComponent<MLManager>();
            }
            
            // Language Manager
            if (languageManager == null && createMissingComponents)
            {
                var languageManagerObj = new GameObject("Language Manager");
                languageManager = languageManagerObj.AddComponent<LanguageManager>();
            }
            
            // Gesture Manager
            if (gestureManager == null && createMissingComponents)
            {
                var gestureManagerObj = new GameObject("Gesture Manager");
                gestureManager = gestureManagerObj.AddComponent<GestureManager>();
            }
            
            // Voice Manager
            if (voiceManager == null && createMissingComponents)
            {
                var voiceManagerObj = new GameObject("Voice Manager");
                voiceManager = voiceManagerObj.AddComponent<VoiceManager>();
            }
            
            // Network Manager
            if (networkManager == null && createMissingComponents)
            {
                var networkManagerObj = new GameObject("Network Manager");
                networkManager = networkManagerObj.AddComponent<NetworkManager>();
            }
            
            // Analytics Manager
            if (analyticsManager == null && createMissingComponents)
            {
                var analyticsManagerObj = new GameObject("Analytics Manager");
                analyticsManager = analyticsManagerObj.AddComponent<AnalyticsManager>();
            }
            
            // Quiz Engine
            if (quizEngine == null && createMissingComponents)
            {
                var quizEngineObj = new GameObject("Quiz Engine");
                quizEngine = quizEngineObj.AddComponent<ARLinguaSphere.Analytics.QuizEngine>();
            }
            
            // AR Label Manager
            if (labelManager == null && createMissingComponents)
            {
                var labelManagerObj = new GameObject("AR Label Manager");
                labelManager = labelManagerObj.AddComponent<ARLabelManager>();
            }
            
            // Main Controller
            if (controller == null && createMissingComponents)
            {
                var controllerObj = new GameObject("ARLinguaSphere Controller");
                controller = controllerObj.AddComponent<ARLinguaSphereController>();
            }
        }
        
        private void SetupUI()
        {
            if (uiManager == null && createMissingComponents)
            {
                var uiManagerObj = new GameObject("UI Manager");
                uiManager = uiManagerObj.AddComponent<UIManager>();
            }
        }
        
        private void ConnectComponents()
        {
            // Connect AR Manager
            if (arManager != null)
            {
                arManager.arSession = arSession;
                arManager.xrOrigin = xrOrigin;
                arManager.arCameraManager = arCameraManager;
                arManager.arPlaneManager = arPlaneManager;
                arManager.arRaycastManager = arRaycastManager;
                arManager.arAnchorManager = arAnchorManager;
            }
            
            // Connect Main Controller
            if (controller != null)
            {
                controller.arManager = arManager;
                controller.mlManager = mlManager;
                controller.languageManager = languageManager;
                controller.gestureManager = gestureManager;
                controller.voiceManager = voiceManager;
                controller.uiManager = uiManager;
                controller.networkManager = networkManager;
                controller.analyticsManager = analyticsManager;
                controller.quizEngine = quizEngine;
                controller.labelManager = labelManager;
                controller.arCamera = xrOrigin.Camera;
            }
        }
        
        [ContextMenu("Clear Scene")]
        public void ClearScene()
        {
            // This will remove all created components
            // Use with caution - it will destroy GameObjects
            Debug.Log("ARSceneSetup: Clearing scene...");
            
            if (Application.isPlaying)
            {
                Debug.LogWarning("ARSceneSetup: Cannot clear scene during play mode");
                return;
            }
            
            // Find and destroy all manager objects
            var managers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var manager in managers)
            {
                if (manager != null && manager.gameObject.name.Contains("Manager"))
                {
                    DestroyImmediate(manager.gameObject);
                }
            }
            
            // Find and destroy AR components
            var arSessionObj = GameObject.Find("AR Session");
            if (arSessionObj != null)
            {
                DestroyImmediate(arSessionObj);
            }
            
            Debug.Log("ARSceneSetup: Scene cleared");
        }
    }
}

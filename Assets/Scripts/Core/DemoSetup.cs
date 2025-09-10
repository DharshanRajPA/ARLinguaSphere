using UnityEngine;
using UnityEngine.XR.ARFoundation;
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
    /// Demo setup script for ARLinguaSphere
    /// This script sets up a complete demo scene with all necessary components
    /// </summary>
    public class DemoSetup : MonoBehaviour
    {
        [Header("Demo Settings")]
        public bool setupOnStart = true;
        public bool createDemoObjects = true;
        public bool enableDebugMode = true;
        
        [Header("Demo Objects")]
        public GameObject[] demoObjects;
        public Vector3[] demoPositions;
        
        private ARLinguaSphereController controller;
        
        private void Start()
        {
            if (setupOnStart)
            {
                SetupDemo();
            }
        }
        
        [ContextMenu("Setup Demo")]
        public void SetupDemo()
        {
            Debug.Log("DemoSetup: Setting up ARLinguaSphere demo...");
            
            // Create main controller
            CreateMainController();
            
            // Setup AR Foundation
            SetupARFoundation();
            
            // Setup UI
            SetupUI();
            
            // Create demo objects
            if (createDemoObjects)
            {
                CreateDemoObjects();
            }
            
            // Start the demo
            StartDemo();
            
            Debug.Log("DemoSetup: Demo setup complete!");
        }
        
        private void CreateMainController()
        {
            // Find or create the main controller
            controller = FindObjectOfType<ARLinguaSphereController>();
            if (controller == null)
            {
                var controllerObj = new GameObject("ARLinguaSphereController");
                controller = controllerObj.AddComponent<ARLinguaSphereController>();
            }
            
            // Enable all features for demo
            controller.enableAutoDetection = true;
            controller.enableVoiceCommands = true;
            controller.enableGestureControls = true;
            controller.enableMultiplayer = false; // Disable for demo
            controller.enableAnalytics = true;
        }
        
        private void SetupARFoundation()
        {
            // Create AR Session
            var arSessionObj = new GameObject("AR Session");
            var arSession = arSessionObj.AddComponent<ARSession>();
            
            // Create AR Session Origin
            var arSessionOriginObj = new GameObject("AR Session Origin");
            var arSessionOrigin = arSessionOriginObj.AddComponent<ARSessionOrigin>();
            arSessionOriginObj.transform.SetParent(arSessionObj.transform);
            
            // Create AR Camera
            var arCameraObj = new GameObject("AR Camera");
            var arCamera = arCameraObj.AddComponent<Camera>();
            arCameraObj.transform.SetParent(arSessionOriginObj.transform);
            arCameraObj.transform.localPosition = Vector3.zero;
            arCameraObj.transform.localRotation = Quaternion.identity;
            
            // Add AR Camera Manager
            var arCameraManager = arCameraObj.AddComponent<ARCameraManager>();
            
            // Add AR Plane Manager
            var arPlaneManager = arSessionOriginObj.AddComponent<ARPlaneManager>();
            
            // Add AR Raycast Manager
            var arRaycastManager = arSessionOriginObj.AddComponent<ARRaycastManager>();
            
            // Add AR Anchor Manager
            var arAnchorManager = arSessionOriginObj.AddComponent<ARAnchorManager>();
            
            // Create AR Manager
            var arManagerObj = new GameObject("ARManager");
            var arManager = arManagerObj.AddComponent<ARManager>();
            arManager.arSession = arSession;
            arManager.arSessionOrigin = arSessionOrigin;
            arManager.arCameraManager = arCameraManager;
            arManager.arPlaneManager = arPlaneManager;
            arManager.arRaycastManager = arRaycastManager;
            arManager.arAnchorManager = arAnchorManager;
            
            // Assign to controller
            controller.arManager = arManager;
        }
        
        private void SetupUI()
        {
            // Create Canvas
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            // Add Canvas Scaler
            var canvasScaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add Graphic Raycaster
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Create UI Manager
            var uiManagerObj = new GameObject("UIManager");
            var uiManager = uiManagerObj.AddComponent<UIManager>();
            uiManagerObj.transform.SetParent(canvasObj.transform);
            
            // Create basic UI elements
            CreateBasicUI(canvasObj);
            
            // Assign to controller
            controller.uiManager = uiManager;
        }
        
        private void CreateBasicUI(GameObject canvas)
        {
            // Create status text
            var statusTextObj = new GameObject("StatusText");
            statusTextObj.transform.SetParent(canvas.transform);
            var statusText = statusTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            statusText.text = "ARLinguaSphere Ready";
            statusText.fontSize = 24;
            statusText.color = Color.white;
            
            // Position at top center
            var statusRect = statusTextObj.AddComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0.5f, 1f);
            statusRect.anchorMax = new Vector2(0.5f, 1f);
            statusRect.anchoredPosition = new Vector2(0, -50);
            statusRect.sizeDelta = new Vector2(400, 50);
            
            // Create language button
            var langButtonObj = new GameObject("LanguageButton");
            langButtonObj.transform.SetParent(canvas.transform);
            var langButton = langButtonObj.AddComponent<UnityEngine.UI.Button>();
            var langButtonImage = langButtonObj.AddComponent<UnityEngine.UI.Image>();
            langButtonImage.color = new Color(0.2f, 0.6f, 1f, 0.8f);
            
            // Position at top right
            var langRect = langButtonObj.AddComponent<RectTransform>();
            langRect.anchorMin = new Vector2(1f, 1f);
            langRect.anchorMax = new Vector2(1f, 1f);
            langRect.anchoredPosition = new Vector2(-100, -50);
            langRect.sizeDelta = new Vector2(150, 50);
            
            // Add text to button
            var langButtonTextObj = new GameObject("Text");
            langButtonTextObj.transform.SetParent(langButtonObj.transform);
            var langButtonText = langButtonTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            langButtonText.text = "Language";
            langButtonText.fontSize = 18;
            langButtonText.color = Color.white;
            langButtonText.alignment = TMPro.TextAlignmentOptions.Center;
            
            var langTextRect = langButtonTextObj.AddComponent<RectTransform>();
            langTextRect.anchorMin = Vector2.zero;
            langTextRect.anchorMax = Vector2.one;
            langTextRect.offsetMin = Vector2.zero;
            langTextRect.offsetMax = Vector2.zero;
            
            // Assign to UI Manager
            var uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.statusText = statusText;
                uiManager.languageButton = langButton;
            }
        }
        
        private void CreateDemoObjects()
        {
            if (demoObjects == null || demoObjects.Length == 0)
            {
                // Create some basic demo objects
                CreateDefaultDemoObjects();
            }
            else
            {
                // Create objects from the array
                for (int i = 0; i < demoObjects.Length && i < demoPositions.Length; i++)
                {
                    if (demoObjects[i] != null)
                    {
                        var obj = Instantiate(demoObjects[i], demoPositions[i], Quaternion.identity);
                        obj.name = $"DemoObject_{i}";
                    }
                }
            }
        }
        
        private void CreateDefaultDemoObjects()
        {
            // Create a simple cube as demo object
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "DemoCube";
            cube.transform.position = new Vector3(0, 0, 2);
            cube.transform.localScale = Vector3.one * 0.5f;
            
            // Add a simple material
            var renderer = cube.GetComponent<Renderer>();
            var material = new Material(Shader.Find("Standard"));
            material.color = Color.red;
            renderer.material = material;
            
            // Create a sphere as another demo object
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "DemoSphere";
            sphere.transform.position = new Vector3(1, 0, 2);
            sphere.transform.localScale = Vector3.one * 0.3f;
            
            var sphereRenderer = sphere.GetComponent<Renderer>();
            var sphereMaterial = new Material(Shader.Find("Standard"));
            sphereMaterial.color = Color.blue;
            sphereRenderer.material = sphereMaterial;
        }
        
        private void StartDemo()
        {
            if (controller != null)
            {
                // Start AR session
                controller.StartARSession();
                
                // Set initial language
                controller.SetLanguage("en");
                
                Debug.Log("DemoSetup: Demo started successfully!");
            }
        }
        
        [ContextMenu("Reset Demo")]
        public void ResetDemo()
        {
            // Stop AR session
            if (controller != null)
            {
                controller.StopARSession();
            }
            
            // Clear all labels
            var labelManager = FindObjectOfType<ARLabelManager>();
            if (labelManager != null)
            {
                labelManager.RemoveAllLabels();
            }
            
            // Restart demo
            StartDemo();
        }
        
        [ContextMenu("Test Object Detection")]
        public void TestObjectDetection()
        {
            var labelManager = FindObjectOfType<ARLabelManager>();
            if (labelManager != null)
            {
                // Place a test label
                labelManager.PlaceLabelAtPosition("test_object", new Vector3(0, 0, 2));
                Debug.Log("DemoSetup: Test label placed");
            }
        }
        
        [ContextMenu("Test Language Change")]
        public void TestLanguageChange()
        {
            if (controller != null)
            {
                controller.SetLanguage("es");
                Debug.Log("DemoSetup: Language changed to Spanish");
            }
        }
    }
}

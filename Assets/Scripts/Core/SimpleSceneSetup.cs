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
    /// Simple scene setup that ensures all components are properly connected
    /// This prevents crashes on Android by ensuring proper initialization order
    /// </summary>
    public class SimpleSceneSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        public bool setupOnStart = true;
        
        private void Start()
        {
            if (setupOnStart)
            {
                SetupScene();
            }
        }
        
        [ContextMenu("Setup Scene")]
        public void SetupScene()
        {
            Debug.Log("🔧 Setting up AR scene to prevent crashes...");
            
            // Step 1: Create AR Session
            var arSession = FindFirstObjectByType<ARSession>();
            if (arSession == null)
            {
                var arSessionObj = new GameObject("AR Session");
                arSession = arSessionObj.AddComponent<ARSession>();
                Debug.Log("✅ AR Session created");
            }
            
            // Step 2: Create XR Origin
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin == null)
            {
                var xrOriginObj = new GameObject("XR Origin");
                xrOrigin = xrOriginObj.AddComponent<XROrigin>();
                xrOrigin.transform.SetParent(arSession.transform);
                Debug.Log("✅ XR Origin created");
            }
            
            // Step 3: Create AR Camera
            var arCamera = xrOrigin.Camera;
            if (arCamera == null)
            {
                var cameraObj = new GameObject("AR Camera");
                cameraObj.transform.SetParent(xrOrigin.transform);
                cameraObj.transform.localPosition = Vector3.zero;
                cameraObj.transform.localRotation = Quaternion.identity;
                arCamera = cameraObj.AddComponent<Camera>();
                xrOrigin.Camera = arCamera;
                Debug.Log("✅ AR Camera created");
            }
            
            // Step 4: Add AR Managers
            if (arCamera.GetComponent<ARCameraManager>() == null)
            {
                arCamera.gameObject.AddComponent<ARCameraManager>();
                Debug.Log("✅ AR Camera Manager added");
            }
            
            if (xrOrigin.GetComponent<ARPlaneManager>() == null)
            {
                xrOrigin.gameObject.AddComponent<ARPlaneManager>();
                Debug.Log("✅ AR Plane Manager added");
            }
            
            if (xrOrigin.GetComponent<ARRaycastManager>() == null)
            {
                xrOrigin.gameObject.AddComponent<ARRaycastManager>();
                Debug.Log("✅ AR Raycast Manager added");
            }
            
            if (xrOrigin.GetComponent<ARAnchorManager>() == null)
            {
                xrOrigin.gameObject.AddComponent<ARAnchorManager>();
                Debug.Log("✅ AR Anchor Manager added");
            }
            
            // Step 5: Create AR Manager
            var arManager = FindFirstObjectByType<ARManager>();
            if (arManager == null)
            {
                var arManagerObj = new GameObject("AR Manager");
                arManager = arManagerObj.AddComponent<ARManager>();
                Debug.Log("✅ AR Manager created");
            }
            
            // Step 6: Connect AR Manager to components
            arManager.arSession = arSession;
            arManager.xrOrigin = xrOrigin;
            arManager.arCameraManager = arCamera.GetComponent<ARCameraManager>();
            arManager.arPlaneManager = xrOrigin.GetComponent<ARPlaneManager>();
            arManager.arRaycastManager = xrOrigin.GetComponent<ARRaycastManager>();
            arManager.arAnchorManager = xrOrigin.GetComponent<ARAnchorManager>();
            
            // Step 7: Create Main Controller
            var controller = FindFirstObjectByType<ARLinguaSphereController>();
            if (controller == null)
            {
                var controllerObj = new GameObject("ARLinguaSphere Controller");
                controller = controllerObj.AddComponent<ARLinguaSphereController>();
                Debug.Log("✅ Main Controller created");
            }
            
            // Step 8: Connect Controller to AR Manager
            controller.arManager = arManager;
            
            Debug.Log("🎉 Scene setup complete! App should not crash now.");
        }
    }
}

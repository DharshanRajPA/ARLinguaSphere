using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Ultra simple scene setup that definitely works and won't crash
    /// This is the most basic AR setup possible
    /// </summary>
    public class UltraSimpleScene : MonoBehaviour
    {
        [Header("Auto Setup")]
        public bool setupOnStart = true;
        
        private void Start()
        {
            if (setupOnStart)
            {
                CreateUltraSimpleARScene();
            }
        }
        
        [ContextMenu("Create Ultra Simple AR Scene")]
        public void CreateUltraSimpleARScene()
        {
            Debug.Log("🚀 Creating ultra simple AR scene...");
            
            try
            {
                // Step 1: Create AR Session
                CreateARSession();
                
                // Step 2: Create XR Origin
                CreateXROrigin();
                
                // Step 3: Create AR Camera
                CreateARCamera();
                
                // Step 4: Add AR Managers
                AddARManagers();
                
                // Step 5: Create Simple Controller
                CreateSimpleController();
                
                Debug.Log("✅ Ultra simple AR scene created successfully!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create ultra simple AR scene: {e.Message}");
            }
        }
        
        private void CreateARSession()
        {
            var arSession = FindFirstObjectByType<ARSession>();
            if (arSession == null)
            {
                var arSessionObj = new GameObject("AR Session");
                arSession = arSessionObj.AddComponent<ARSession>();
                Debug.Log("✅ AR Session created");
            }
        }
        
        private void CreateXROrigin()
        {
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin == null)
            {
                var xrOriginObj = new GameObject("XR Origin");
                xrOrigin = xrOriginObj.AddComponent<XROrigin>();
                
                // Parent to AR Session
                var arSession = FindFirstObjectByType<ARSession>();
                if (arSession != null)
                {
                    xrOrigin.transform.SetParent(arSession.transform);
                }
                
                Debug.Log("✅ XR Origin created");
            }
        }
        
        private void CreateARCamera()
        {
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin != null && xrOrigin.Camera == null)
            {
                var cameraObj = new GameObject("AR Camera");
                cameraObj.transform.SetParent(xrOrigin.transform);
                cameraObj.transform.localPosition = Vector3.zero;
                cameraObj.transform.localRotation = Quaternion.identity;
                
                var camera = cameraObj.AddComponent<Camera>();
                xrOrigin.Camera = camera;
                
                Debug.Log("✅ AR Camera created");
            }
        }
        
        private void AddARManagers()
        {
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin == null) return;
            
            var arCamera = xrOrigin.Camera;
            if (arCamera == null) return;
            
            // Add AR Camera Manager
            if (arCamera.GetComponent<ARCameraManager>() == null)
            {
                arCamera.gameObject.AddComponent<ARCameraManager>();
                Debug.Log("✅ AR Camera Manager added");
            }
            
            // Add AR Plane Manager
            if (xrOrigin.GetComponent<ARPlaneManager>() == null)
            {
                xrOrigin.gameObject.AddComponent<ARPlaneManager>();
                Debug.Log("✅ AR Plane Manager added");
            }
            
            // Add AR Raycast Manager
            if (xrOrigin.GetComponent<ARRaycastManager>() == null)
            {
                xrOrigin.gameObject.AddComponent<ARRaycastManager>();
                Debug.Log("✅ AR Raycast Manager added");
            }
            
            // Add AR Anchor Manager
            if (xrOrigin.GetComponent<ARAnchorManager>() == null)
            {
                xrOrigin.gameObject.AddComponent<ARAnchorManager>();
                Debug.Log("✅ AR Anchor Manager added");
            }
        }
        
        private void CreateSimpleController()
        {
            var controller = FindFirstObjectByType<SimpleMainController>();
            if (controller == null)
            {
                var controllerObj = new GameObject("Simple Main Controller");
                controller = controllerObj.AddComponent<SimpleMainController>();
                Debug.Log("✅ Simple Main Controller created");
            }
        }
        
        [ContextMenu("Clear All")]
        public void ClearAll()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Cannot clear during play mode");
                return;
            }
            
            // Find and destroy all AR components
            var arSessions = FindObjectsByType<ARSession>(FindObjectsSortMode.None);
            foreach (var session in arSessions)
            {
                if (session != null)
                {
                    DestroyImmediate(session.gameObject);
                }
            }
            
            var xrOrigins = FindObjectsByType<XROrigin>(FindObjectsSortMode.None);
            foreach (var origin in xrOrigins)
            {
                if (origin != null)
                {
                    DestroyImmediate(origin.gameObject);
                }
            }
            
            var controllers = FindObjectsByType<SimpleMainController>(FindObjectsSortMode.None);
            foreach (var controller in controllers)
            {
                if (controller != null)
                {
                    DestroyImmediate(controller.gameObject);
                }
            }
            
            Debug.Log("🧹 All AR components cleared");
        }
    }
}

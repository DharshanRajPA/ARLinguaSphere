using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using System.Collections;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Simplified main controller that prevents crashes
    /// This is a minimal version that focuses on stability over features
    /// </summary>
    public class SimpleMainController : MonoBehaviour
    {
        [Header("AR Components")]
        public ARSession arSession;
        public XROrigin xrOrigin;
        public Camera arCamera;
        
        [Header("Settings")]
        public bool enableAutoSetup = true;
        public bool enableMockFeatures = true;
        public float mockDetectionInterval = 2.0f;
        
        private bool isInitialized = false;
        private bool isARSessionRunning = false;
        
        private void Start()
        {
            if (enableAutoSetup)
            {
                StartCoroutine(InitializeSafely());
            }
        }
        
        private IEnumerator InitializeSafely()
        {
            Debug.Log("🚀 SimpleMainController: Starting safe initialization...");
            
            // Wait for Unity to fully load
            yield return new WaitForEndOfFrame();
            
            // Step 1: Find or create AR components
            yield return StartCoroutine(SetupARComponents());
            
            // Step 2: Initialize AR session
            yield return StartCoroutine(InitializeARSession());
            
            // Step 3: Start mock features
            if (enableMockFeatures)
            {
                yield return StartCoroutine(StartMockFeatures());
            }
            
            isInitialized = true;
            Debug.Log("✅ SimpleMainController: Initialization complete!");
        }
        
        private IEnumerator SetupARComponents()
        {
            Debug.Log("🔧 Setting up AR components...");
            
            // Find AR Session
            arSession = FindFirstObjectByType<ARSession>();
            if (arSession == null)
            {
                var arSessionObj = new GameObject("AR Session");
                arSession = arSessionObj.AddComponent<ARSession>();
                Debug.Log("✅ AR Session created");
            }
            
            // Find XR Origin
            xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin == null)
            {
                var xrOriginObj = new GameObject("XR Origin");
                xrOrigin = xrOriginObj.AddComponent<XROrigin>();
                xrOrigin.transform.SetParent(arSession.transform);
                Debug.Log("✅ XR Origin created");
            }
            
            // Setup AR Camera
            arCamera = xrOrigin.Camera;
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
            
            // Add essential AR managers
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
            
            yield return new WaitForSeconds(0.1f);
        }
        
        private IEnumerator InitializeARSession()
        {
            Debug.Log("🔧 Initializing AR session...");
            
            if (arSession != null)
            {
                // Subscribe to AR session events
                ARSession.stateChanged += OnARSessionStateChanged;
                
                // Start AR session
                arSession.enabled = true;
                
                yield return new WaitForSeconds(1.0f);
                
                Debug.Log("✅ AR session initialized");
            }
        }
        
        private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            Debug.Log($"AR Session State: {args.state}");
            
            switch (args.state)
            {
                case ARSessionState.Ready:
                    isARSessionRunning = true;
                    Debug.Log("✅ AR Session is ready!");
                    break;
                case ARSessionState.SessionTracking:
                    isARSessionRunning = true;
                    Debug.Log("✅ AR Session is tracking!");
                    break;
                case ARSessionState.None:
                    isARSessionRunning = false;
                    Debug.LogWarning("⚠️ AR Session not initialized");
                    break;
                case ARSessionState.Unsupported:
                    isARSessionRunning = false;
                    Debug.LogError("❌ AR not supported on this device");
                    break;
            }
        }
        
        private IEnumerator StartMockFeatures()
        {
            Debug.Log("🎭 Starting mock features...");
            
            // Start mock object detection
            StartCoroutine(MockObjectDetection());
            
            // Start mock hand gestures
            StartCoroutine(MockHandGestures());
            
            yield return null;
        }
        
        private IEnumerator MockObjectDetection()
        {
            while (isInitialized)
            {
                yield return new WaitForSeconds(mockDetectionInterval);
                
                if (isARSessionRunning)
                {
                    Debug.Log("🎯 Mock: Object detected - 'person' (confidence: 0.85)");
                    Debug.Log("🎯 Mock: Object detected - 'car' (confidence: 0.72)");
                }
            }
        }
        
        private IEnumerator MockHandGestures()
        {
            while (isInitialized)
            {
                yield return new WaitForSeconds(mockDetectionInterval + 1.0f);
                
                if (isARSessionRunning)
                {
                    Debug.Log("👋 Mock: Hand gesture detected - 'pointing'");
                    Debug.Log("👋 Mock: Hand gesture detected - 'open palm'");
                }
            }
        }
        
        private void Update()
        {
            if (isInitialized && isARSessionRunning)
            {
                // Update AR camera position
                if (arCamera != null)
                {
                    // AR Foundation handles camera positioning automatically
                }
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            ARSession.stateChanged -= OnARSessionStateChanged;
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Debug.Log("📱 App paused");
            }
            else
            {
                Debug.Log("📱 App resumed");
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Debug.Log("📱 App focused");
            }
            else
            {
                Debug.Log("📱 App lost focus");
            }
        }
        
        // Public properties
        public bool IsInitialized => isInitialized;
        public bool IsARSessionRunning => isARSessionRunning;
        public Camera ARCamera => arCamera;
    }
}

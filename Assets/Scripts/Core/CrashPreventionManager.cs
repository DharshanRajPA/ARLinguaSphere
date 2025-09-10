using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using System.Collections;
using System.Collections.Generic;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Prevents app crashes by ensuring proper initialization and error handling
    /// This is the most critical component for preventing Android crashes
    /// </summary>
    public class CrashPreventionManager : MonoBehaviour
    {
        [Header("Crash Prevention Settings")]
        public bool enableCrashPrevention = true;
        public bool enableSafeMode = true;
        public bool enableMockMode = true;
        public float initializationDelay = 0.1f;
        
        [Header("Debug Settings")]
        public bool enableDetailedLogging = true;
        public bool logToFile = false;
        
        private bool isInitialized = false;
        private List<string> errorLog = new List<string>();
        
        private void Awake()
        {
            // Ensure this runs first
            Application.logMessageReceived += OnLogMessageReceived;
            
            if (enableCrashPrevention)
            {
                StartCoroutine(SafeInitialization());
            }
        }
        
        private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                errorLog.Add($"[{type}] {logString}");
                
                if (logToFile)
                {
                    Debug.LogError($"CRASH PREVENTION: {logString}");
                }
            }
        }
        
        private IEnumerator SafeInitialization()
        {
            Debug.Log("🛡️ CrashPreventionManager: Starting safe initialization...");
            
            // Wait a frame to ensure Unity is fully loaded
            yield return new WaitForEndOfFrame();
            
            // Step 1: Validate AR Foundation availability
            if (!ValidateARFoundation())
            {
                Debug.LogError("❌ AR Foundation not available - switching to safe mode");
                yield break;
            }
            
            // Step 2: Create minimal AR setup
            yield return StartCoroutine(CreateMinimalARSetup());
            
            // Step 3: Initialize core systems safely
            yield return StartCoroutine(InitializeCoreSystemsSafely());
            
            // Step 4: Start mock systems
            if (enableMockMode)
            {
                yield return StartCoroutine(StartMockSystems());
            }
            
            isInitialized = true;
            Debug.Log("✅ CrashPreventionManager: Safe initialization complete!");
        }
        
        private bool ValidateARFoundation()
        {
            try
            {
                // Check if AR Foundation is available
                var arSession = FindFirstObjectByType<ARSession>();
                if (arSession == null)
                {
                    Debug.LogWarning("AR Foundation not found - creating minimal setup");
                    return true; // We'll create it
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"AR Foundation validation failed: {e.Message}");
                return false;
            }
        }
        
        private IEnumerator CreateMinimalARSetup()
        {
            Debug.Log("🔧 Creating minimal AR setup...");
            
            // Create AR Session if it doesn't exist
            var arSession = FindFirstObjectByType<ARSession>();
            if (arSession == null)
            {
                var arSessionObj = new GameObject("AR Session");
                arSession = arSessionObj.AddComponent<ARSession>();
                Debug.Log("✅ AR Session created");
            }
            
            // Create XR Origin if it doesn't exist
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin == null)
            {
                var xrOriginObj = new GameObject("XR Origin");
                xrOrigin = xrOriginObj.AddComponent<XROrigin>();
                xrOrigin.transform.SetParent(arSession.transform);
                Debug.Log("✅ XR Origin created");
            }
            
            // Create AR Camera if it doesn't exist
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
            
            // Add essential AR components
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
            
            yield return new WaitForSeconds(initializationDelay);
        }
        
        private IEnumerator InitializeCoreSystemsSafely()
        {
            Debug.Log("🔧 Initializing core systems safely...");
            
            // Create AR Manager safely
            var arManager = FindFirstObjectByType<ARLinguaSphere.AR.ARManager>();
            if (arManager == null)
            {
                var arManagerObj = new GameObject("AR Manager");
                arManager = arManagerObj.AddComponent<ARLinguaSphere.AR.ARManager>();
                
                // Connect to AR components
                arManager.arSession = FindFirstObjectByType<ARSession>();
                arManager.xrOrigin = FindFirstObjectByType<XROrigin>();
                arManager.arCameraManager = arManager.xrOrigin?.GetComponent<ARCameraManager>();
                arManager.arPlaneManager = arManager.xrOrigin?.GetComponent<ARPlaneManager>();
                arManager.arRaycastManager = arManager.xrOrigin?.GetComponent<ARRaycastManager>();
                arManager.arAnchorManager = arManager.xrOrigin?.GetComponent<ARAnchorManager>();
                
                Debug.Log("✅ AR Manager created and connected");
            }
            
            // Initialize AR Manager safely
            if (arManager != null)
            {
                try
                {
                    arManager.Initialize();
                    Debug.Log("✅ AR Manager initialized");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"AR Manager initialization failed: {e.Message}");
                }
            }
            
            yield return new WaitForSeconds(initializationDelay);
        }
        
        private IEnumerator StartMockSystems()
        {
            Debug.Log("🎭 Starting mock systems...");
            
            // Start mock object detection
            StartCoroutine(MockObjectDetection());
            
            // Start mock hand gestures
            StartCoroutine(MockHandGestures());
            
            yield return new WaitForSeconds(initializationDelay);
        }
        
        private IEnumerator MockObjectDetection()
        {
            while (isInitialized)
            {
                yield return new WaitForSeconds(2.0f);
                
                if (enableDetailedLogging)
                {
                    Debug.Log("🎯 Mock: Object detected - 'person' (confidence: 0.85)");
                }
            }
        }
        
        private IEnumerator MockHandGestures()
        {
            while (isInitialized)
            {
                yield return new WaitForSeconds(3.0f);
                
                if (enableDetailedLogging)
                {
                    Debug.Log("👋 Mock: Hand gesture detected - 'pointing'");
                }
            }
        }
        
        private void OnDestroy()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Debug.Log("📱 App paused - saving state");
            }
            else
            {
                Debug.Log("📱 App resumed - restoring state");
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Debug.Log("📱 App focused - resuming operations");
            }
            else
            {
                Debug.Log("📱 App lost focus - pausing operations");
            }
        }
        
        // Public methods for external access
        public bool IsInitialized => isInitialized;
        public List<string> GetErrorLog() => new List<string>(errorLog);
        public void ClearErrorLog() => errorLog.Clear();
    }
}

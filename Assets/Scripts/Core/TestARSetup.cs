using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Simple test script to verify AR setup works without crashes
    /// This is the most basic test possible
    /// </summary>
    public class TestARSetup : MonoBehaviour
    {
        [Header("Test Settings")]
        public bool runTestOnStart = true;
        public bool enableLogging = true;
        
        private void Start()
        {
            if (runTestOnStart)
            {
                StartCoroutine(RunARTest());
            }
        }
        
        [ContextMenu("Run AR Test")]
        public void RunARTestMenu()
        {
            StartCoroutine(RunARTest());
        }
        
        private System.Collections.IEnumerator RunARTest()
        {
            Log("🧪 Starting AR Test...");
            
            // Test 1: Check AR Foundation availability
            yield return StartCoroutine(TestARFoundation());
            
            // Test 2: Check XR Origin
            yield return StartCoroutine(TestXROrigin());
            
            // Test 3: Check AR Camera
            yield return StartCoroutine(TestARCamera());
            
            // Test 4: Check AR Managers
            yield return StartCoroutine(TestARManagers());
            
            Log("✅ AR Test completed successfully!");
        }
        
        private System.Collections.IEnumerator TestARFoundation()
        {
            Log("🔍 Testing AR Foundation...");
            
            var arSession = FindFirstObjectByType<ARSession>();
            if (arSession == null)
            {
                Log("❌ AR Session not found - creating one");
                var arSessionObj = new GameObject("AR Session");
                arSession = arSessionObj.AddComponent<ARSession>();
            }
            else
            {
                Log("✅ AR Session found");
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        private System.Collections.IEnumerator TestXROrigin()
        {
            Log("🔍 Testing XR Origin...");
            
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin == null)
            {
                Log("❌ XR Origin not found - creating one");
                var xrOriginObj = new GameObject("XR Origin");
                xrOrigin = xrOriginObj.AddComponent<XROrigin>();
                
                // Parent to AR Session
                var arSession = FindFirstObjectByType<ARSession>();
                if (arSession != null)
                {
                    xrOrigin.transform.SetParent(arSession.transform);
                }
            }
            else
            {
                Log("✅ XR Origin found");
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        private System.Collections.IEnumerator TestARCamera()
        {
            Log("🔍 Testing AR Camera...");
            
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin == null)
            {
                Log("❌ XR Origin not found - cannot test AR Camera");
                yield break;
            }
            
            var arCamera = xrOrigin.Camera;
            if (arCamera == null)
            {
                Log("❌ AR Camera not found - creating one");
                var cameraObj = new GameObject("AR Camera");
                cameraObj.transform.SetParent(xrOrigin.transform);
                cameraObj.transform.localPosition = Vector3.zero;
                cameraObj.transform.localRotation = Quaternion.identity;
                arCamera = cameraObj.AddComponent<Camera>();
                xrOrigin.Camera = arCamera;
            }
            else
            {
                Log("✅ AR Camera found");
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        private System.Collections.IEnumerator TestARManagers()
        {
            Log("🔍 Testing AR Managers...");
            
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin == null)
            {
                Log("❌ XR Origin not found - cannot test AR Managers");
                yield break;
            }
            
            var arCamera = xrOrigin.Camera;
            if (arCamera == null)
            {
                Log("❌ AR Camera not found - cannot test AR Managers");
                yield break;
            }
            
            // Test AR Camera Manager
            if (arCamera.GetComponent<ARCameraManager>() == null)
            {
                Log("❌ AR Camera Manager not found - adding one");
                arCamera.gameObject.AddComponent<ARCameraManager>();
            }
            else
            {
                Log("✅ AR Camera Manager found");
            }
            
            // Test AR Plane Manager
            if (xrOrigin.GetComponent<ARPlaneManager>() == null)
            {
                Log("❌ AR Plane Manager not found - adding one");
                xrOrigin.gameObject.AddComponent<ARPlaneManager>();
            }
            else
            {
                Log("✅ AR Plane Manager found");
            }
            
            // Test AR Raycast Manager
            if (xrOrigin.GetComponent<ARRaycastManager>() == null)
            {
                Log("❌ AR Raycast Manager not found - adding one");
                xrOrigin.gameObject.AddComponent<ARRaycastManager>();
            }
            else
            {
                Log("✅ AR Raycast Manager found");
            }
            
            // Test AR Anchor Manager
            if (xrOrigin.GetComponent<ARAnchorManager>() == null)
            {
                Log("❌ AR Anchor Manager not found - adding one");
                xrOrigin.gameObject.AddComponent<ARAnchorManager>();
            }
            else
            {
                Log("✅ AR Anchor Manager found");
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        private void Log(string message)
        {
            if (enableLogging)
            {
                Debug.Log(message);
            }
        }
        
        [ContextMenu("Clear All AR Components")]
        public void ClearAllARComponents()
        {
            if (Application.isPlaying)
            {
                Log("⚠️ Cannot clear during play mode");
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
            
            Log("🧹 All AR components cleared");
        }
    }
}

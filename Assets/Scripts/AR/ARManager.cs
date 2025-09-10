using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

namespace ARLinguaSphere.AR
{
    /// <summary>
    /// Manages AR Foundation components and AR session
    /// </summary>
    public class ARManager : MonoBehaviour
    {
        [Header("AR Components")]
        public ARSession arSession;
        public ARSessionOrigin arSessionOrigin;
        public ARCameraManager arCameraManager;
        public ARPlaneManager arPlaneManager;
        public ARRaycastManager arRaycastManager;
        public ARAnchorManager arAnchorManager;
        
        [Header("AR Settings")]
        public bool enablePlaneDetection = true;
        public bool enableLightEstimation = true;
        public bool enableOcclusion = false;
        
        [Header("Prefabs")]
        public GameObject labelPrefab;
        public GameObject anchorPrefab;
        
        private List<ARAnchor> activeAnchors = new List<ARAnchor>();
        private Camera arCamera;
        
        public bool IsARSessionRunning { get; private set; }
        public Camera ARCamera => arCamera;
        
        public void Initialize()
        {
            Debug.Log("ARManager: Initializing AR systems...");
            
            // Get AR camera reference
            arCamera = arSessionOrigin.camera;
            
            // Configure AR session
            ConfigureARSession();
            
            // Subscribe to AR events
            SubscribeToAREvents();
            
            Debug.Log("ARManager: AR systems initialized!");
        }
        
        private void ConfigureARSession()
        {
            if (arSession == null)
            {
                Debug.LogError("ARManager: ARSession is not assigned!");
                return;
            }
            
            // Configure AR session settings
            var sessionConfig = arSession.subsystem?.GetConfiguration();
            if (sessionConfig.HasValue)
            {
                // Enable/disable features based on settings
                // Note: This is a simplified configuration
                Debug.Log("ARManager: AR session configured");
            }
        }
        
        private void SubscribeToAREvents()
        {
            if (arSession != null)
            {
                arSession.stateChanged += OnARSessionStateChanged;
            }
            
            if (arCameraManager != null)
            {
                arCameraManager.frameReceived += OnCameraFrameReceived;
            }
        }
        
        private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            Debug.Log($"ARManager: AR Session state changed to {args.state}");
            
            switch (args.state)
            {
                case ARSessionState.SessionTracking:
                    IsARSessionRunning = true;
                    Debug.Log("ARManager: AR session is now tracking!");
                    break;
                case ARSessionState.SessionNotTracking:
                    IsARSessionRunning = false;
                    Debug.Log("ARManager: AR session lost tracking");
                    break;
                case ARSessionState.SessionPaused:
                    IsARSessionRunning = false;
                    Debug.Log("ARManager: AR session paused");
                    break;
            }
        }
        
        private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
        {
            // This is where we would process camera frames for ML inference
            // The MLManager will handle the actual processing
        }
        
        public bool TryPlaceAnchor(Vector2 screenPosition, out ARAnchor anchor)
        {
            anchor = null;
            
            if (!IsARSessionRunning)
            {
                Debug.LogWarning("ARManager: Cannot place anchor - AR session not running");
                return false;
            }
            
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (arRaycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                anchor = arAnchorManager.AddAnchor(hitPose);
                
                if (anchor != null)
                {
                    activeAnchors.Add(anchor);
                    Debug.Log($"ARManager: Anchor placed at {hitPose.position}");
                    return true;
                }
            }
            
            Debug.LogWarning("ARManager: Failed to place anchor - no valid surface found");
            return false;
        }
        
        public void RemoveAnchor(ARAnchor anchor)
        {
            if (anchor != null && activeAnchors.Contains(anchor))
            {
                activeAnchors.Remove(anchor);
                arAnchorManager.RemoveAnchor(anchor);
                Debug.Log("ARManager: Anchor removed");
            }
        }
        
        public void ClearAllAnchors()
        {
            foreach (var anchor in activeAnchors)
            {
                if (anchor != null)
                {
                    arAnchorManager.RemoveAnchor(anchor);
                }
            }
            activeAnchors.Clear();
            Debug.Log("ARManager: All anchors cleared");
        }
        
        public Vector3 ScreenToWorldPoint(Vector2 screenPoint, float distance = 1f)
        {
            if (arCamera == null) return Vector3.zero;
            
            Ray ray = arCamera.ScreenPointToRay(screenPoint);
            return ray.GetPoint(distance);
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (arSession != null)
            {
                arSession.stateChanged -= OnARSessionStateChanged;
            }
            
            if (arCameraManager != null)
            {
                arCameraManager.frameReceived -= OnCameraFrameReceived;
            }
        }
    }
}

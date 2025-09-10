using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using System.Collections.Generic;
using Unity.Collections;

namespace ARLinguaSphere.AR
{
    /// <summary>
    /// Manages AR Foundation components and AR session
    /// </summary>
    public class ARManager : MonoBehaviour
    {
        [Header("AR Components")]
        public ARSession arSession;
        public XROrigin xrOrigin; // Updated to use XROrigin instead of deprecated ARSessionOrigin
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
        private Texture2D cachedCameraTexture;
        private XRCpuImage.ConversionParams conversionParams;
        
        public bool IsARSessionRunning { get; private set; }
        public Camera ARCamera => arCamera;
        
        public void Initialize()
        {
            Debug.Log("ARManager: Initializing AR systems...");
            
            // Get AR camera reference
            arCamera = xrOrigin.Camera;
            
            // (Unity 6) Skip session configuration via subsystem.GetConfiguration (removed)
            
            // Subscribe to AR events
            SubscribeToAREvents();
            
            Debug.Log("ARManager: AR systems initialized!");
        }
        
        // (Unity 6) Removed ConfigureARSession() body; AR Foundation handles configuration internally
        
        private void SubscribeToAREvents()
        {
            // (Unity 6) subscribe to static ARSession.stateChanged
            ARSession.stateChanged += OnARSessionStateChanged;
            
            if (arCameraManager != null)
            {
                arCameraManager.frameReceived += OnCameraFrameReceived;
            }
        }
        
        private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            Debug.Log($"ARManager: AR Session state changed to {args.state}");
            
            if (args.state == ARSessionState.SessionTracking)
            {
                IsARSessionRunning = true;
                Debug.Log("ARManager: AR session is now tracking!");
            }
            else
            {
                IsARSessionRunning = false;
            }
        }
        
        private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
        {
            // This is where we would process camera frames for ML inference
            // The MLManager will handle the actual processing
        }

        /// <summary>
        /// Acquire latest camera CPU image and convert to Texture2D (RGBA32).
        /// </summary>
        public Texture2D GetLatestCameraTexture()
        {
            if (arCameraManager == null) return null;
            if (!arCameraManager.TryAcquireLatestCpuImage(out var cpuImage))
            {
                return null;
            }

            using (cpuImage)
            {
                // Set up conversion params
                conversionParams = new XRCpuImage.ConversionParams
                {
                    inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
                    outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
                    outputFormat = TextureFormat.RGBA32,
                    transformation = XRCpuImage.Transformation.MirrorY
                };

                int size = cpuImage.GetConvertedDataSize(conversionParams);
                var buffer = new NativeArray<byte>(size, Allocator.Temp);
                try
                {
                    cpuImage.Convert(conversionParams, buffer);

                    // Create/update texture
                    if (cachedCameraTexture == null ||
                        cachedCameraTexture.width != conversionParams.outputDimensions.x ||
                        cachedCameraTexture.height != conversionParams.outputDimensions.y)
                    {
                        cachedCameraTexture = new Texture2D(
                            conversionParams.outputDimensions.x,
                            conversionParams.outputDimensions.y,
                            conversionParams.outputFormat,
                            false
                        );
                    }

                    cachedCameraTexture.LoadRawTextureData(buffer);
                    cachedCameraTexture.Apply();
                    return cachedCameraTexture;
                }
                finally
                {
                    buffer.Dispose();
                }
            }
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
                var hit = hits[0];
                var plane = arPlaneManager != null ? arPlaneManager.GetPlane(hit.trackableId) : null;
                if (plane != null)
                {
                    var anchorAttached = arAnchorManager.AttachAnchor(plane, hit.pose);
                    if (anchorAttached != null)
                    {
                        anchor = anchorAttached;
                        activeAnchors.Add(anchor);
                        Debug.Log($"ARManager: Anchor placed at {hit.pose.position}");
                        return true;
                    }
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
                Destroy(anchor);
                Debug.Log("ARManager: Anchor removed");
            }
        }
        
        public void ClearAllAnchors()
        {
            foreach (var anchor in activeAnchors)
            {
                if (anchor != null) Destroy(anchor);
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
            ARSession.stateChanged -= OnARSessionStateChanged;
            
            if (arCameraManager != null)
            {
                arCameraManager.frameReceived -= OnCameraFrameReceived;
            }
        }
    }
}

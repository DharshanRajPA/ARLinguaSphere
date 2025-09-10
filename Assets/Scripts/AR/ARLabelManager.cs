using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ARLinguaSphere.ML;
using ARLinguaSphere.Core;

namespace ARLinguaSphere.AR
{
    /// <summary>
    /// Manages AR labels and their placement in AR space
    /// </summary>
    public class ARLabelManager : MonoBehaviour
    {
        [Header("Label Settings")]
        public GameObject labelPrefab;
        public float labelOffset = 0.1f;
        public float labelScale = 1f;
        public bool autoPlaceLabels = true;
        public float placementCooldown = 1f;
        
        [Header("Detection Settings")]
        public float minDetectionConfidence = 0.5f;
        public float maxDetectionDistance = 10f;
        public bool enableLabelCulling = true;
        
        [Header("UI Settings")]
        public Color labelColor = Color.white;
        public Color backgroundColor = new Color(0, 0, 0, 0.7f);
        public int maxLabelsPerObject = 1;
        
        private ARManager arManager;
        private MLManager mlManager;
        private LanguageManager languageManager;
        private ARLinguaSphere.Network.NetworkManager networkManager;
        private Camera arCamera;
        
        private List<ARLabel> activeLabels = new List<ARLabel>();
        private Dictionary<string, ARLabel> objectLabels = new Dictionary<string, ARLabel>();
        private Dictionary<string, ARLabel> anchorIdToLabel = new Dictionary<string, ARLabel>();
        private float lastPlacementTime = 0f;
        
        // Events
        public System.Action<ARLabel> OnLabelPlaced;
        public System.Action<ARLabel> OnLabelRemoved;
        public System.Action<ARLabel> OnLabelClicked;
        
        public void Initialize(ARManager arMgr, MLManager mlMgr, LanguageManager langMgr, ARLinguaSphere.Network.NetworkManager netMgr = null)
        {
            arManager = arMgr;
            mlManager = mlMgr;
            languageManager = langMgr;
            networkManager = netMgr;
            
            // Get AR camera reference
            arCamera = arManager.ARCamera;
            
            // Subscribe to ML detection events
            if (mlManager != null)
            {
                mlManager.OnObjectsDetected += OnObjectsDetected;
            }
            
            // Subscribe to language change events
            if (languageManager != null)
            {
                languageManager.OnLanguageChanged += OnLanguageChanged;
            }
            
            // Subscribe to network anchors
            if (networkManager != null)
            {
                networkManager.OnAnchorReceived += OnAnchorReceived;
            }
            
            Debug.Log("ARLabelManager: Initialized");
        }
        
        private void OnObjectsDetected(List<Detection> detections)
        {
            if (!autoPlaceLabels || detections == null) return;
            
            foreach (var detection in detections)
            {
                if (detection.confidence >= minDetectionConfidence)
                {
                    ProcessDetection(detection);
                }
            }
        }
        
        private void ProcessDetection(Detection detection)
        {
            // Check if we already have a label for this object
            string objectKey = GetObjectKey(detection);
            if (objectLabels.ContainsKey(objectKey))
            {
                return; // Already labeled
            }
            
            // Check placement cooldown
            if (Time.time - lastPlacementTime < placementCooldown)
            {
                return;
            }
            
            // Convert screen coordinates to world position
            Vector2 screenPoint = GetScreenPointFromBoundingBox(detection.boundingBox);
            Vector3 worldPosition = GetWorldPositionFromScreen(screenPoint);
            
            if (worldPosition != Vector3.zero)
            {
                PlaceLabel(detection, worldPosition);
            }
        }
        
        private string GetObjectKey(Detection detection)
        {
            // Create a unique key based on object position and type
            Vector2 center = detection.boundingBox.center;
            return $"{detection.label}_{center.x:F2}_{center.y:F2}";
        }
        
        private Vector2 GetScreenPointFromBoundingBox(Rect boundingBox)
        {
            // Convert normalized coordinates to screen coordinates
            float screenX = boundingBox.center.x * Screen.width;
            float screenY = (1f - boundingBox.center.y) * Screen.height; // Flip Y coordinate
            
            return new Vector2(screenX, screenY);
        }
        
        private Vector3 GetWorldPositionFromScreen(Vector2 screenPoint)
        {
            if (arCamera == null) return Vector3.zero;
            
            // Raycast from screen point to find AR plane
            Ray ray = arCamera.ScreenPointToRay(screenPoint);
            
            // Use AR raycast to find plane intersection
            if (arManager != null && arManager.arRaycastManager != null)
            {
                var hits = new List<UnityEngine.XR.ARFoundation.ARRaycastHit>();
                if (arManager.arRaycastManager.Raycast(screenPoint, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = hits[0].pose;
                    return hitPose.position + Vector3.up * labelOffset;
                }
            }
            
            // Fallback: place at fixed distance
            return ray.GetPoint(2f) + Vector3.up * labelOffset;
        }
        
        private void PlaceLabel(Detection detection, Vector3 worldPosition)
        {
            if (labelPrefab == null)
            {
                Debug.LogWarning("ARLabelManager: Label prefab not assigned");
                return;
            }
            
            // Create label instance
            GameObject labelObj = Instantiate(labelPrefab, worldPosition, Quaternion.identity);
            ARLabel label = labelObj.GetComponent<ARLabel>();
            
            if (label == null)
            {
                label = labelObj.AddComponent<ARLabel>();
            }
            
            // Initialize label
            string translatedText = GetTranslatedText(detection.label);
            label.Initialize(translatedText, languageManager?.currentLanguage ?? "en");
            
            // Configure label appearance
            label.SetScale(labelScale);
            label.SetColor(labelColor);
            label.SetBackgroundColor(backgroundColor);
            
            // Subscribe to label events
            label.OnLabelClicked += OnLabelClicked;
            label.OnLabelDestroyed += OnLabelDestroyed;
            
            // Add to active labels
            activeLabels.Add(label);
            objectLabels[GetObjectKey(detection)] = label;
            
            // Update placement time
            lastPlacementTime = Time.time;
            
            // Notify listeners
            OnLabelPlaced?.Invoke(label);
            
            // Sync to multiplayer room if available
            if (networkManager != null && networkManager.IsConnected && networkManager.IsInRoom)
            {
                var anchorData = new ARLinguaSphere.Network.AnchorData
                {
                    position = worldPosition,
                    rotation = Quaternion.LookRotation(arCamera.transform.forward),
                    labelKey = detection.label,
                    creatorId = SystemInfo.deviceUniqueIdentifier
                };
                networkManager.SendAnchor(anchorData);
            }
            
            Debug.Log($"ARLabelManager: Placed label '{translatedText}' at {worldPosition}");
        }
        
        private string GetTranslatedText(string objectLabel)
        {
            if (languageManager == null)
            {
                return objectLabel;
            }
            
            return languageManager.GetTranslation(objectLabel);
        }
        
        private void OnLanguageChanged(string previousLanguage, string newLanguage)
        {
            // Update all existing labels with new translations
            foreach (var label in activeLabels)
            {
                if (label != null)
                {
                    string originalText = label.GetOriginalText();
                    string translatedText = languageManager?.GetTranslation(originalText) ?? originalText;
                    label.UpdateText(translatedText, newLanguage);
                }
            }
        }
        
        private void OnLabelDestroyed(ARLabel label)
        {
            activeLabels.Remove(label);
            
            // Remove from object labels dictionary
            var keyToRemove = objectLabels.FirstOrDefault(x => x.Value == label).Key;
            if (keyToRemove != null)
            {
                objectLabels.Remove(keyToRemove);
            }
            
            // Remove from anchor map
            var anchorKey = anchorIdToLabel.FirstOrDefault(x => x.Value == label).Key;
            if (anchorKey != null)
            {
                anchorIdToLabel.Remove(anchorKey);
            }
            
            OnLabelRemoved?.Invoke(label);
        }
        
        public void PlaceLabelAtPosition(string objectLabel, Vector3 worldPosition)
        {
            var detection = new Detection
            {
                label = objectLabel,
                confidence = 1f,
                boundingBox = new Rect(0.5f, 0.5f, 0.1f, 0.1f)
            };
            
            PlaceLabel(detection, worldPosition);
        }
        
        public void RemoveLabel(ARLabel label)
        {
            if (label != null)
            {
                label.DestroyLabel();
            }
        }
        
        public void RemoveAllLabels()
        {
            var labelsToRemove = new List<ARLabel>(activeLabels);
            foreach (var label in labelsToRemove)
            {
                RemoveLabel(label);
            }
        }
        
        public void RemoveLabelsByObject(string objectLabel)
        {
            var labelsToRemove = activeLabels.Where(l => l.GetOriginalText() == objectLabel).ToList();
            foreach (var label in labelsToRemove)
            {
                RemoveLabel(label);
            }
        }
        
        public List<ARLabel> GetActiveLabels()
        {
            return new List<ARLabel>(activeLabels);
        }
        
        public int GetLabelCount()
        {
            return activeLabels.Count;
        }
        
        public void SetAutoPlaceLabels(bool enabled)
        {
            autoPlaceLabels = enabled;
        }
        
        public void SetLabelScale(float scale)
        {
            labelScale = scale;
            foreach (var label in activeLabels)
            {
                if (label != null)
                {
                    label.SetScale(scale);
                }
            }
        }
        
        public void SetLabelColor(Color color)
        {
            labelColor = color;
            foreach (var label in activeLabels)
            {
                if (label != null)
                {
                    label.SetColor(color);
                }
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (mlManager != null)
            {
                mlManager.OnObjectsDetected -= OnObjectsDetected;
            }
            
            if (languageManager != null)
            {
                languageManager.OnLanguageChanged -= OnLanguageChanged;
            }
            
            if (networkManager != null)
            {
                networkManager.OnAnchorReceived -= OnAnchorReceived;
            }
        }

        private void OnAnchorReceived(ARLinguaSphere.Network.AnchorData anchor)
        {
            if (anchor == null) return;
            if (anchorIdToLabel.ContainsKey(anchor.id)) return; // already placed
            
            PlaceLabelFromAnchor(anchor);
        }

        public void PlaceLabelFromAnchor(ARLinguaSphere.Network.AnchorData anchor)
        {
            if (anchor == null) return;
            var detection = new Detection
            {
                label = anchor.labelKey,
                confidence = 1f,
                boundingBox = new Rect(0.5f, 0.5f, 0.1f, 0.1f)
            };
            
            // Instantiate label
            if (labelPrefab == null)
            {
                Debug.LogWarning("ARLabelManager: Label prefab not assigned");
                return;
            }
            GameObject labelObj = Instantiate(labelPrefab, anchor.position + Vector3.up * labelOffset, anchor.rotation);
            ARLabel label = labelObj.GetComponent<ARLabel>();
            if (label == null) label = labelObj.AddComponent<ARLabel>();
            
            string translatedText = GetTranslatedText(anchor.labelKey);
            label.Initialize(translatedText, languageManager?.currentLanguage ?? "en");
            label.SetScale(labelScale);
            label.SetColor(labelColor);
            label.SetBackgroundColor(backgroundColor);
            label.OnLabelClicked += OnLabelClicked;
            label.OnLabelDestroyed += OnLabelDestroyed;
            
            activeLabels.Add(label);
            objectLabels[GetObjectKey(detection)] = label;
            anchorIdToLabel[anchor.id] = label;
            
            OnLabelPlaced?.Invoke(label);
            Debug.Log($"ARLabelManager: Placed network label '{translatedText}' at {anchor.position}");
        }
    }
}

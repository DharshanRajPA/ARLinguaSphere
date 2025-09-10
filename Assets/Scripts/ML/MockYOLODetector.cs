using UnityEngine;
using System.Collections.Generic;

namespace ARLinguaSphere.ML
{
    /// <summary>
    /// Mock YOLO detector for testing without native TensorFlow Lite
    /// </summary>
    public class MockYOLODetector : MonoBehaviour
    {
        [Header("Mock Detection Settings")]
        public float detectionInterval = 2f;
        public int maxDetections = 5;
        
        private float lastDetectionTime;
        private bool isInitialized = false;
        
        public bool IsInitialized => isInitialized;
        
        private void Start()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            Debug.Log("MockYOLODetector: Initializing mock detector...");
            isInitialized = true;
            Debug.Log("MockYOLODetector: Mock detector initialized successfully");
        }
        
        public List<Detection> DetectObjects(Texture2D inputTexture)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("MockYOLODetector: Not initialized");
                return new List<Detection>();
            }
            
            // Simulate detection every few seconds
            if (Time.time - lastDetectionTime < detectionInterval)
            {
                return new List<Detection>();
            }
            
            lastDetectionTime = Time.time;
            
            // Generate mock detections
            var detections = new List<Detection>();
            
            for (int i = 0; i < maxDetections; i++)
            {
                if (Random.Range(0f, 1f) > 0.7f) // 30% chance of detection
                {
                    var detection = new Detection
                    {
                        classId = Random.Range(0, 80), // COCO classes
                        confidence = Random.Range(0.5f, 0.95f),
                        boundingBox = new Rect(
                            Random.Range(0.1f, 0.7f), // x
                            Random.Range(0.1f, 0.7f), // y
                            Random.Range(0.1f, 0.3f), // width
                            Random.Range(0.1f, 0.3f)  // height
                        ),
                        label = "object" // Add the label property
                    };
                    detections.Add(detection);
                }
            }
            
            Debug.Log($"MockYOLODetector: Generated {detections.Count} mock detections");
            return detections;
        }
        
        public void SetConfidenceThreshold(float threshold)
        {
            Debug.Log($"MockYOLODetector: Confidence threshold set to {threshold}");
        }
        
        public void SetNMSThreshold(float threshold)
        {
            Debug.Log($"MockYOLODetector: NMS threshold set to {threshold}");
        }
    }
    
}

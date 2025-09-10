using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace ARLinguaSphere.ML
{
    /// <summary>
    /// Manages machine learning inference for object detection
    /// </summary>
    public class MLManager : MonoBehaviour
    {
        [Header("ML Settings")]
        public float confidenceThreshold = 0.5f;
        public int maxDetections = 10;
        public bool enableGPU = true;
        public bool enableAsyncProcessing = true;
        
        [Header("Model Settings")]
        public string modelPath = "Models/yolov8s.tflite";
        public int inputWidth = 640;
        public int inputHeight = 640;
        
        [Header("Performance Settings")]
        public float processingInterval = 0.1f; // Process every 100ms
        public bool enableFrameSkipping = true;
        public int maxFrameSkip = 3;
        
        private bool isInitialized = false;
        private bool isProcessing = false;
        private YOLODetector yoloDetector;
        private Queue<Texture2D> frameQueue;
        private int skippedFrames = 0;
        
        // Events
        public event Action<List<Detection>> OnObjectsDetected;
        
        public void Initialize()
        {
            Debug.Log("MLManager: Initializing ML systems...");
            
            // Initialize YOLO detector
            yoloDetector = gameObject.AddComponent<YOLODetector>();
            yoloDetector.modelPath = modelPath;
            yoloDetector.confidenceThreshold = confidenceThreshold;
            yoloDetector.maxDetections = maxDetections;
            yoloDetector.inputWidth = inputWidth;
            yoloDetector.inputHeight = inputHeight;
            yoloDetector.Initialize();
            
            // Initialize frame queue for async processing
            frameQueue = new Queue<Texture2D>();
            
            // Start processing coroutine
            if (enableAsyncProcessing)
            {
                StartCoroutine(ProcessFrameQueue());
            }
            
            isInitialized = true;
            Debug.Log("MLManager: ML systems initialized!");
        }
        
        public void ProcessFrame(Texture2D frame)
        {
            if (!isInitialized)
            {
                return;
            }
            
            // Frame skipping for performance
            if (enableFrameSkipping && skippedFrames < maxFrameSkip)
            {
                skippedFrames++;
                return;
            }
            
            skippedFrames = 0;
            
            if (enableAsyncProcessing)
            {
                // Add frame to queue for async processing
                frameQueue.Enqueue(frame);
            }
            else
            {
                // Process frame synchronously
                StartCoroutine(ProcessFrameAsync(frame));
            }
        }
        
        private IEnumerator ProcessFrameQueue()
        {
            while (isInitialized)
            {
                if (frameQueue.Count > 0 && !isProcessing)
                {
                    var frame = frameQueue.Dequeue();
                    yield return StartCoroutine(ProcessFrameAsync(frame));
                }
                
                yield return new WaitForSeconds(processingInterval);
            }
        }
        
        private IEnumerator ProcessFrameAsync(Texture2D frame)
        {
            isProcessing = true;
            
            // Run YOLO detection
            var detections = yoloDetector.DetectObjects(frame);
            
            // Notify listeners
            OnObjectsDetected?.Invoke(detections);
            
            isProcessing = false;
            yield return null;
        }
        
        public void SetConfidenceThreshold(float threshold)
        {
            confidenceThreshold = Mathf.Clamp01(threshold);
            if (yoloDetector != null)
            {
                yoloDetector.SetConfidenceThreshold(threshold);
            }
        }
        
        public void SetMaxDetections(int max)
        {
            maxDetections = Mathf.Max(1, max);
            if (yoloDetector != null)
            {
                yoloDetector.SetMaxDetections(max);
            }
        }
        
        public bool IsProcessing => isProcessing;
        public int QueuedFrames => frameQueue?.Count ?? 0;
    }
    
    /// <summary>
    /// Represents a detected object
    /// </summary>
    [System.Serializable]
    public class Detection
    {
        public string label;
        public float confidence;
        public Rect boundingBox; // Normalized coordinates (0-1)
        public int classId = -1;
        
        public override string ToString()
        {
            return $"{label} ({confidence:P1}) at {boundingBox}";
        }
    }
}

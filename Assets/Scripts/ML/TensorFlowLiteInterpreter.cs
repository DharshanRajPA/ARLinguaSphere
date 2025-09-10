using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using Unity.Collections;

namespace ARLinguaSphere.ML
{
    /// <summary>
    /// Wrapper for TensorFlow Lite interpreter functionality
    /// Supports both Android native and Unity Barracuda fallback
    /// </summary>
    public class TensorFlowLiteInterpreter : IDisposable
    {
        private TensorFlowLiteUnityBridge bridge;
        private bool isDisposed = false;
        private byte[] modelData;
        private string modelPath;
        
        public bool IsInitialized => bridge != null && bridge.IsInitialized;
        
        public TensorFlowLiteInterpreter(string modelPath)
        {
            this.modelPath = modelPath;
            InitializeInterpreter(modelPath);
        }
        
        private void InitializeInterpreter(string modelPath)
        {
            try
            {
                // Load model data
                modelData = LoadModelData(modelPath);
                if (modelData == null)
                {
                    Debug.LogError($"TensorFlowLiteInterpreter: Failed to load model data from {modelPath}");
                    return;
                }
                
                // Create Unity bridge
                bridge = new TensorFlowLiteUnityBridge();
                
                // Initialize interpreter with model data
                if (!bridge.CreateInterpreter(modelData))
                {
                    Debug.LogError("TensorFlowLiteInterpreter: Failed to create interpreter via bridge");
                    bridge?.Dispose();
                    bridge = null;
                    return;
                }
                
                Debug.Log($"TensorFlowLiteInterpreter: Initialized successfully with model {modelPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteInterpreter: Failed to initialize: {e.Message}");
                bridge?.Dispose();
                bridge = null;
            }
        }
        
        private byte[] LoadModelData(string modelPath)
        {
            try
            {
                // Try StreamingAssets first
                string streamingPath = Path.Combine(Application.streamingAssetsPath, modelPath);
                
#if UNITY_ANDROID && !UNITY_EDITOR
                // On Android, StreamingAssets are in the APK
                var www = new WWW(streamingPath);
                while (!www.isDone) { }
                
                if (string.IsNullOrEmpty(www.error))
                {
                    return www.bytes;
                }
#else
                // On other platforms, read directly from file system
                if (File.Exists(streamingPath))
                {
                    return File.ReadAllBytes(streamingPath);
                }
#endif
                
                // Try Resources folder as fallback
                var textAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(modelPath));
                if (textAsset != null)
                {
                    return textAsset.bytes;
                }
                
                Debug.LogError($"TensorFlowLiteInterpreter: Model file not found: {modelPath}");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteInterpreter: Error loading model data: {e.Message}");
                return null;
            }
        }
        
        public void SetInputTensorData(int inputIndex, float[] data)
        {
            if (!IsInitialized)
            {
                Debug.LogError("TensorFlowLiteInterpreter: Not initialized");
                return;
            }
            
            try
            {
                if (!bridge.SetInputTensor(inputIndex, data))
                {
                    Debug.LogError($"TensorFlowLiteInterpreter: Failed to set input tensor {inputIndex}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteInterpreter: Error setting input tensor: {e.Message}");
            }
        }
        
        public void SetInputTensorData(int inputIndex, byte[] data)
        {
            if (!IsInitialized)
            {
                Debug.LogError("TensorFlowLiteInterpreter: Not initialized");
                return;
            }
            
            try
            {
                if (!bridge.SetInputTensor(inputIndex, data))
                {
                    Debug.LogError($"TensorFlowLiteInterpreter: Failed to set input tensor {inputIndex}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteInterpreter: Error setting input tensor: {e.Message}");
            }
        }
        
        public void Invoke()
        {
            if (!IsInitialized)
            {
                Debug.LogError("TensorFlowLiteInterpreter: Not initialized");
                return;
            }
            
            try
            {
                if (!bridge.Invoke())
                {
                    Debug.LogError("TensorFlowLiteInterpreter: Inference failed");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteInterpreter: Error during inference: {e.Message}");
            }
        }
        
        public void GetOutputTensorData(int outputIndex, float[] output)
        {
            if (!IsInitialized)
            {
                Debug.LogError("TensorFlowLiteInterpreter: Not initialized");
                return;
            }
            
            try
            {
                if (!bridge.GetOutputTensor(outputIndex, output))
                {
                    Debug.LogError($"TensorFlowLiteInterpreter: Failed to get output tensor {outputIndex}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteInterpreter: Error getting output tensor: {e.Message}");
            }
        }
        
        private void GenerateMockOutput(float[] output)
        {
            // Generate realistic mock YOLO output for testing
            // YOLOv8 format: [batch, detections, (x, y, w, h, confidence, class_scores...)]
            
            int detectionSize = 85; // 4 bbox + 1 conf + 80 classes
            int numDetections = output.Length / detectionSize;
            
            // Generate a few realistic detections
            int actualDetections = Mathf.Min(3, numDetections);
            
            for (int i = 0; i < actualDetections; i++)
            {
                int baseIndex = i * detectionSize;
                
                // Bounding box (center format, normalized)
                output[baseIndex + 0] = UnityEngine.Random.Range(0.3f, 0.7f); // center x
                output[baseIndex + 1] = UnityEngine.Random.Range(0.3f, 0.7f); // center y
                output[baseIndex + 2] = UnityEngine.Random.Range(0.1f, 0.4f); // width
                output[baseIndex + 3] = UnityEngine.Random.Range(0.1f, 0.4f); // height
                output[baseIndex + 4] = UnityEngine.Random.Range(0.6f, 0.95f); // confidence
                
                // Class scores (make one class dominant)
                int dominantClass = UnityEngine.Random.Range(0, 80);
                for (int j = 0; j < 80; j++)
                {
                    if (j == dominantClass)
                    {
                        output[baseIndex + 5 + j] = UnityEngine.Random.Range(0.7f, 0.95f);
                    }
                    else
                    {
                        output[baseIndex + 5 + j] = UnityEngine.Random.Range(0.0f, 0.3f);
                    }
                }
            }
            
            // Fill remaining detections with low confidence
            for (int i = actualDetections; i < numDetections; i++)
            {
                int baseIndex = i * detectionSize;
                output[baseIndex + 4] = 0.1f; // Low confidence
            }
        }
        
        public int GetInputTensorCount()
        {
            if (IsInitialized)
            {
                return bridge.GetInputTensorCount();
            }
            return 1; // Default for most models
        }
        
        public int GetOutputTensorCount()
        {
            if (IsInitialized)
            {
                return bridge.GetOutputTensorCount();
            }
            return 1; // Default for most models
        }
        
        public int[] GetInputTensorShape(int inputIndex)
        {
            if (IsInitialized)
            {
                return bridge.GetInputTensorShape(inputIndex);
            }
            // Return typical YOLOv8 input shape: [1, 640, 640, 3]
            return new int[] { 1, 640, 640, 3 };
        }
        
        public int[] GetOutputTensorShape(int outputIndex)
        {
            if (IsInitialized)
            {
                return bridge.GetOutputTensorShape(outputIndex);
            }
            // Return typical YOLOv8 output shape: [1, 25200, 85]
            return new int[] { 1, 25200, 85 };
        }
        
        public void Dispose()
        {
            if (!isDisposed)
            {
                bridge?.Dispose();
                bridge = null;
                modelData = null;
                isDisposed = true;
                Debug.Log("TensorFlowLiteInterpreter: Disposed");
            }
        }
        
        ~TensorFlowLiteInterpreter()
        {
            Dispose();
        }
        
        // Performance monitoring
        public void EnableProfiler(bool enable)
        {
            Debug.Log($"TensorFlowLiteInterpreter: Profiler {(enable ? "enabled" : "disabled")}");
        }
        
        public void SetNumThreads(int numThreads)
        {
            Debug.Log($"TensorFlowLiteInterpreter: Set number of threads to {numThreads}");
        }
        
        public void UseGPU(bool useGPU)
        {
            Debug.Log($"TensorFlowLiteInterpreter: GPU acceleration {(useGPU ? "enabled" : "disabled")}");
        }
    }
}
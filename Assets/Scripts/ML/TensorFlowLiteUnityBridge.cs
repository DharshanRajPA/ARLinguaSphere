using UnityEngine;
using System;

namespace ARLinguaSphere.ML
{
    /// <summary>
    /// Unity bridge for TensorFlow Lite Android plugin
    /// Provides simplified interface for TensorFlow Lite operations
    /// </summary>
    public class TensorFlowLiteUnityBridge : IDisposable
    {
        private AndroidJavaObject tflitePlugin;
        private long interpreterPtr;
        private bool isInitialized = false;
        private bool isDisposed = false;
        
        public bool IsInitialized => isInitialized;
        
        public TensorFlowLiteUnityBridge()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                // Get the TensorFlow Lite plugin class
                using (var pluginClass = new AndroidJavaClass("com.arlinguasphere.TFLitePlugin"))
                {
                    tflitePlugin = pluginClass;
                }
                Debug.Log("TensorFlowLiteUnityBridge: Android plugin initialized");
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteUnityBridge: Failed to initialize Android plugin: {e.Message}");
            }
#endif
        }
        
        public bool CreateInterpreter(byte[] modelData)
        {
            if (isDisposed)
            {
                Debug.LogError("TensorFlowLiteUnityBridge: Bridge is disposed");
                return false;
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (tflitePlugin != null)
                {
                    interpreterPtr = tflitePlugin.CallStatic<long>("createInterpreter", modelData);
                    isInitialized = interpreterPtr != 0;
                    
                    if (isInitialized)
                    {
                        Debug.Log("TensorFlowLiteUnityBridge: Interpreter created successfully");
                    }
                    else
                    {
                        Debug.LogError("TensorFlowLiteUnityBridge: Failed to create interpreter");
                    }
                    
                    return isInitialized;
                }
#endif
                Debug.LogWarning("TensorFlowLiteUnityBridge: Running in Editor mode - using mock interpreter");
                interpreterPtr = 1; // Mock pointer for Editor
                isInitialized = true;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteUnityBridge: Error creating interpreter: {e.Message}");
                return false;
            }
        }
        
        public bool SetInputTensor(int inputIndex, float[] data)
        {
            if (!isInitialized)
            {
                Debug.LogError("TensorFlowLiteUnityBridge: Interpreter not initialized");
                return false;
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (tflitePlugin != null)
                {
                    int result = tflitePlugin.CallStatic<int>("setInputTensor", interpreterPtr, inputIndex, data, data.Length);
                    return result == 0;
                }
#endif
                Debug.Log($"TensorFlowLiteUnityBridge: Set input tensor {inputIndex} (mock)");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteUnityBridge: Error setting input tensor: {e.Message}");
                return false;
            }
        }
        
        public bool SetInputTensor(int inputIndex, byte[] data)
        {
            if (!isInitialized)
            {
                Debug.LogError("TensorFlowLiteUnityBridge: Interpreter not initialized");
                return false;
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (tflitePlugin != null)
                {
                    int result = tflitePlugin.CallStatic<int>("setInputTensorBytes", interpreterPtr, inputIndex, data, data.Length);
                    return result == 0;
                }
#endif
                Debug.Log($"TensorFlowLiteUnityBridge: Set input tensor {inputIndex} bytes (mock)");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteUnityBridge: Error setting input tensor: {e.Message}");
                return false;
            }
        }
        
        public bool Invoke()
        {
            if (!isInitialized)
            {
                Debug.LogError("TensorFlowLiteUnityBridge: Interpreter not initialized");
                return false;
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (tflitePlugin != null)
                {
                    int result = tflitePlugin.CallStatic<int>("invoke", interpreterPtr);
                    return result == 0;
                }
#endif
                Debug.Log("TensorFlowLiteUnityBridge: Invoke (mock)");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteUnityBridge: Error during inference: {e.Message}");
                return false;
            }
        }
        
        public bool GetOutputTensor(int outputIndex, float[] output)
        {
            if (!isInitialized)
            {
                Debug.LogError("TensorFlowLiteUnityBridge: Interpreter not initialized");
                return false;
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (tflitePlugin != null)
                {
                    int result = tflitePlugin.CallStatic<int>("getOutputTensor", interpreterPtr, outputIndex, output, output.Length);
                    return result == 0;
                }
#endif
                Debug.Log($"TensorFlowLiteUnityBridge: Get output tensor {outputIndex} (mock)");
                GenerateMockOutput(output);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteUnityBridge: Error getting output tensor: {e.Message}");
                return false;
            }
        }
        
        public int GetInputTensorCount()
        {
            if (!isInitialized)
            {
                return 0;
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (tflitePlugin != null)
                {
                    return tflitePlugin.CallStatic<int>("getInputTensorCount", interpreterPtr);
                }
#endif
                return 1; // Mock value for Editor
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteUnityBridge: Error getting input tensor count: {e.Message}");
                return 0;
            }
        }
        
        public int GetOutputTensorCount()
        {
            if (!isInitialized)
            {
                return 0;
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (tflitePlugin != null)
                {
                    return tflitePlugin.CallStatic<int>("getOutputTensorCount", interpreterPtr);
                }
#endif
                return 1; // Mock value for Editor
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteUnityBridge: Error getting output tensor count: {e.Message}");
                return 0;
            }
        }
        
        public int[] GetInputTensorShape(int inputIndex)
        {
            if (!isInitialized)
            {
                return new int[0];
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (tflitePlugin != null)
                {
                    int[] shape = new int[4]; // Assume 4D tensor
                    tflitePlugin.CallStatic("getInputTensorShape", interpreterPtr, inputIndex, shape);
                    return shape;
                }
#endif
                // Mock shape for Editor
                return new int[] { 1, 640, 640, 3 };
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteUnityBridge: Error getting input tensor shape: {e.Message}");
                return new int[0];
            }
        }
        
        public int[] GetOutputTensorShape(int outputIndex)
        {
            if (!isInitialized)
            {
                return new int[0];
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (tflitePlugin != null)
                {
                    int[] shape = new int[3]; // Assume 3D tensor for YOLO output
                    tflitePlugin.CallStatic("getOutputTensorShape", interpreterPtr, outputIndex, shape);
                    return shape;
                }
#endif
                // Mock shape for Editor
                return new int[] { 1, 25200, 85 };
            }
            catch (Exception e)
            {
                Debug.LogError($"TensorFlowLiteUnityBridge: Error getting output tensor shape: {e.Message}");
                return new int[0];
            }
        }
        
        private void GenerateMockOutput(float[] output)
        {
            // Generate realistic mock YOLO output for Editor testing
            int detectionSize = 85; // 4 bbox + 1 conf + 80 classes
            int numDetections = output.Length / detectionSize;
            
            // Generate a few realistic detections
            int actualDetections = Mathf.Min(5, numDetections);
            
            // Common object classes for testing
            int[] commonClasses = { 0, 2, 5, 15, 16, 39, 41, 56, 67 }; // person, car, bus, cat, dog, bottle, cup, chair, cell phone
            
            for (int i = 0; i < actualDetections; i++)
            {
                int baseIndex = i * detectionSize;
                
                // Bounding box (center format, normalized)
                output[baseIndex + 0] = UnityEngine.Random.Range(0.2f, 0.8f); // center x
                output[baseIndex + 1] = UnityEngine.Random.Range(0.2f, 0.8f); // center y
                output[baseIndex + 2] = UnityEngine.Random.Range(0.1f, 0.4f); // width
                output[baseIndex + 3] = UnityEngine.Random.Range(0.1f, 0.4f); // height
                output[baseIndex + 4] = UnityEngine.Random.Range(0.6f, 0.95f); // confidence
                
                // Class scores (make one class dominant)
                int dominantClass = commonClasses[UnityEngine.Random.Range(0, commonClasses.Length)];
                for (int j = 0; j < 80; j++)
                {
                    if (j == dominantClass)
                    {
                        output[baseIndex + 5 + j] = UnityEngine.Random.Range(0.7f, 0.95f);
                    }
                    else
                    {
                        output[baseIndex + 5 + j] = UnityEngine.Random.Range(0.0f, 0.2f);
                    }
                }
            }
            
            // Fill remaining detections with low confidence
            for (int i = actualDetections; i < numDetections; i++)
            {
                int baseIndex = i * detectionSize;
                output[baseIndex + 4] = 0.05f; // Very low confidence
            }
        }
        
        public void Dispose()
        {
            if (!isDisposed)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (tflitePlugin != null && interpreterPtr != 0)
                {
                    try
                    {
                        tflitePlugin.CallStatic("destroyInterpreter", interpreterPtr);
                        Debug.Log("TensorFlowLiteUnityBridge: Interpreter destroyed");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"TensorFlowLiteUnityBridge: Error destroying interpreter: {e.Message}");
                    }
                }
                
                tflitePlugin?.Dispose();
                tflitePlugin = null;
#endif
                interpreterPtr = 0;
                isInitialized = false;
                isDisposed = true;
            }
        }
        
        ~TensorFlowLiteUnityBridge()
        {
            Dispose();
        }
    }
}

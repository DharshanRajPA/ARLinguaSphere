using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ARLinguaSphere.ML
{
    /// <summary>
    /// YOLO-based object detector using TensorFlow Lite
    /// </summary>
    public class YOLODetector : MonoBehaviour
    {
        [Header("YOLO Settings")]
        public string modelPath = "Models/yolov8n_float32.tflite";
        public float confidenceThreshold = 0.5f;
        public float nmsThreshold = 0.4f;
        public int maxDetections = 100;
        
        [Header("Input Settings")]
        public int inputWidth = 640;
        public int inputHeight = 640;
        public bool normalizeInput = true;
        
        private TensorFlowLiteInterpreter interpreter;
        private bool isInitialized = false;
        
        // COCO class names (first 20 for brevity)
        private readonly string[] classNames = {
            "person", "bicycle", "car", "motorcycle", "airplane", "bus", "train", "truck", "boat",
            "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat",
            "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack",
            "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball",
            "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket",
            "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple",
            "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake",
            "chair", "couch", "potted plant", "bed", "dining table", "toilet", "tv", "laptop",
            "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink",
            "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush"
        };
        
        public void Initialize()
        {
            if (isInitialized) return;
            
            Debug.Log("YOLODetector: Initializing YOLO detector...");
            
            try
            {
                interpreter = new TensorFlowLiteInterpreter(modelPath);
                
                // For now, always succeed in mock mode
                isInitialized = true;
                Debug.Log("YOLODetector: YOLO detector initialized successfully (mock mode)");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"YOLODetector: Initialization failed: {e.Message}");
            }
        }
        
        public List<Detection> DetectObjects(Texture2D inputTexture)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("YOLODetector: Not initialized");
                return new List<Detection>();
            }
            
            // Preprocess input texture
            var preprocessedData = PreprocessTexture(inputTexture);
            
            // Run inference
            var rawDetections = RunInference(preprocessedData);
            
            // Post-process detections
            var detections = PostProcessDetections(rawDetections, inputTexture.width, inputTexture.height);
            
            return detections;
        }
        
        private float[] PreprocessTexture(Texture2D texture)
        {
            // Resize texture to model input size
            var resizedTexture = ResizeTexture(texture, inputWidth, inputHeight);
            
            // Convert to float array and normalize
            var pixels = resizedTexture.GetPixels();
            var floatArray = new float[inputWidth * inputHeight * 3];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                int baseIndex = i * 3;
                floatArray[baseIndex + 0] = pixels[i].r; // Red channel
                floatArray[baseIndex + 1] = pixels[i].g; // Green channel
                floatArray[baseIndex + 2] = pixels[i].b; // Blue channel
                
                if (normalizeInput)
                {
                    floatArray[baseIndex + 0] = (floatArray[baseIndex + 0] - 0.5f) / 0.5f;
                    floatArray[baseIndex + 1] = (floatArray[baseIndex + 1] - 0.5f) / 0.5f;
                    floatArray[baseIndex + 2] = (floatArray[baseIndex + 2] - 0.5f) / 0.5f;
                }
            }
            
            return floatArray;
        }
        
        private Texture2D ResizeTexture(Texture2D original, int width, int height)
        {
            var resized = new Texture2D(width, height, TextureFormat.RGB24, false);
            var pixels = new Color[width * height];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float u = (float)x / width;
                    float v = (float)y / height;
                    
                    int sourceX = Mathf.RoundToInt(u * (original.width - 1));
                    int sourceY = Mathf.RoundToInt(v * (original.height - 1));
                    
                    pixels[y * width + x] = original.GetPixel(sourceX, sourceY);
                }
            }
            
            resized.SetPixels(pixels);
            resized.Apply();
            
            return resized;
        }
        
        private float[] RunInference(float[] inputData)
        {
            // Set input tensor
            interpreter.SetInputTensorData(0, inputData);
            
            // Run inference
            interpreter.Invoke();
            
            // Get output tensor
            var outputShape = interpreter.GetOutputTensorShape(0);
            int outputSize = outputShape[1] * outputShape[2]; // [1, 25200, 85]
            var output = new float[outputSize];
            interpreter.GetOutputTensorData(0, output);
            
            return output;
        }
        
        private List<Detection> PostProcessDetections(float[] rawOutput, int originalWidth, int originalHeight)
        {
            var detections = new List<Detection>();
            
            // Parse YOLO output format: [x, y, w, h, confidence, class_scores...]
            int numDetections = rawOutput.Length / 85; // 85 = 4 (bbox) + 1 (confidence) + 80 (classes)
            
            for (int i = 0; i < numDetections; i++)
            {
                int baseIndex = i * 85;
                
                // Extract bounding box (center x, center y, width, height)
                float centerX = rawOutput[baseIndex + 0];
                float centerY = rawOutput[baseIndex + 1];
                float width = rawOutput[baseIndex + 2];
                float height = rawOutput[baseIndex + 3];
                float confidence = rawOutput[baseIndex + 4];
                
                // Skip low confidence detections
                if (confidence < confidenceThreshold)
                    continue;
                
                // Find best class
                int bestClassIndex = 0;
                float bestClassScore = 0f;
                
                for (int j = 0; j < 80; j++)
                {
                    float classScore = rawOutput[baseIndex + 5 + j];
                    if (classScore > bestClassScore)
                    {
                        bestClassScore = classScore;
                        bestClassIndex = j;
                    }
                }
                
                // Calculate final confidence
                float finalConfidence = confidence * bestClassScore;
                
                if (finalConfidence < confidenceThreshold)
                    continue;
                
                // Convert to corner coordinates
                float x1 = (centerX - width / 2f) / inputWidth;
                float y1 = (centerY - height / 2f) / inputHeight;
                float x2 = (centerX + width / 2f) / inputWidth;
                float y2 = (centerY + height / 2f) / inputHeight;
                
                // Clamp to [0, 1]
                x1 = Mathf.Clamp01(x1);
                y1 = Mathf.Clamp01(y1);
                x2 = Mathf.Clamp01(x2);
                y2 = Mathf.Clamp01(y2);
                
                // Create detection
                var detection = new Detection
                {
                    label = bestClassIndex < classNames.Length ? classNames[bestClassIndex] : $"class_{bestClassIndex}",
                    confidence = finalConfidence,
                    boundingBox = new Rect(x1, y1, x2 - x1, y2 - y1),
                    classId = bestClassIndex
                };
                
                detections.Add(detection);
            }
            
            // Apply Non-Maximum Suppression
            detections = ApplyNMS(detections);
            
            // Limit to max detections
            if (detections.Count > maxDetections)
            {
                detections = detections.OrderByDescending(d => d.confidence)
                                    .Take(maxDetections)
                                    .ToList();
            }
            
            return detections;
        }
        
        private List<Detection> ApplyNMS(List<Detection> detections)
        {
            var result = new List<Detection>();
            var sortedDetections = detections.OrderByDescending(d => d.confidence).ToList();
            
            while (sortedDetections.Count > 0)
            {
                var best = sortedDetections[0];
                result.Add(best);
                sortedDetections.RemoveAt(0);
                
                // Remove overlapping detections
                for (int i = sortedDetections.Count - 1; i >= 0; i--)
                {
                    float iou = CalculateIoU(best.boundingBox, sortedDetections[i].boundingBox);
                    if (iou > nmsThreshold)
                    {
                        sortedDetections.RemoveAt(i);
                    }
                }
            }
            
            return result;
        }
        
        private float CalculateIoU(Rect box1, Rect box2)
        {
            float x1 = Mathf.Max(box1.x, box2.x);
            float y1 = Mathf.Max(box1.y, box2.y);
            float x2 = Mathf.Min(box1.x + box1.width, box2.x + box2.width);
            float y2 = Mathf.Min(box1.y + box1.height, box2.y + box2.height);
            
            if (x2 <= x1 || y2 <= y1)
                return 0f;
            
            float intersection = (x2 - x1) * (y2 - y1);
            float area1 = box1.width * box1.height;
            float area2 = box2.width * box2.height;
            float union = area1 + area2 - intersection;
            
            return intersection / union;
        }
        
        public void SetConfidenceThreshold(float threshold)
        {
            confidenceThreshold = Mathf.Clamp01(threshold);
        }
        
        public void SetNMSThreshold(float threshold)
        {
            nmsThreshold = Mathf.Clamp01(threshold);
        }
        
        public void SetMaxDetections(int max)
        {
            maxDetections = Mathf.Max(1, max);
        }
        
        private void OnDestroy()
        {
            interpreter?.Dispose();
        }
    }
}

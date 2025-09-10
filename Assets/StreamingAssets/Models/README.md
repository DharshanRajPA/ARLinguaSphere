# Model Files

This directory contains the machine learning models used by ARLinguaSphere.

## Required Files

### Object Detection Model
- **File**: `yolov8n_float32.tflite`
- **Purpose**: YOLOv8 object detection for identifying objects in AR
- **Size**: ~6MB
- **Format**: TensorFlow Lite (.tflite)

## How to Get the Model

### Option 1: Download Pre-converted Model (Recommended)
1. Download the YOLOv8n TFLite model from: https://github.com/ultralytics/assets/releases
2. Look for `yolov8n.pt` or `yolov8n.tflite`
3. If you get `.pt`, convert it using the Python script below
4. Save as: `yolov8n_float32.tflite` in this directory

### Option 2: Convert from PyTorch (Advanced)
```python
# Install dependencies
pip install ultralytics tensorflow

# Convert YOLOv8 to TFLite
from ultralytics import YOLO
model = YOLO('yolov8n.pt')
model.export(format='tflite', int8=False, dynamic=False)
```

## Model Configuration

In Unity, set the model path in the YOLODetector component:
- **Model Path**: `Models/yolov8n_float32.tflite`
- **Input Size**: 640x640
- **Classes**: 80 (COCO dataset)

## Troubleshooting

- **Model not found**: Ensure the file is exactly named `yolov8n_float32.tflite`
- **Wrong format**: Make sure it's a TensorFlow Lite model, not PyTorch
- **Size issues**: The model should be around 6MB for YOLOv8n
- **Performance**: Use float32 for better accuracy, int8 for faster inference

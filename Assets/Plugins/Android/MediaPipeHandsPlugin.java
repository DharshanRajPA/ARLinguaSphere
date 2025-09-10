package com.arlinguasphere;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Log;

import com.google.mediapipe.components.FrameProcessor;
import com.google.mediapipe.framework.AndroidAssetUtil;
import com.google.mediapipe.framework.AndroidPacketCreator;
import com.google.mediapipe.framework.Packet;
import com.google.mediapipe.framework.PacketGetter;
import com.google.mediapipe.glutil.EglManager;

import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * MediaPipe Hands plugin for Unity
 * Provides real-time hand landmark detection using MediaPipe
 */
public class MediaPipeHandsPlugin {
    private static final String TAG = "MediaPipeHandsPlugin";
    private static final String BINARY_GRAPH_NAME = "hand_landmark_tracking_mobile_gpu.binarypb";
    private static final String INPUT_VIDEO_STREAM_NAME = "input_video";
    private static final String OUTPUT_LANDMARKS_STREAM_NAME = "hand_landmarks";
    
    private Context context;
    private FrameProcessor processor;
    private EglManager eglManager;
    private AndroidPacketCreator packetCreator;
    private String unityCallbackObject;
    
    private boolean initialized = false;
    private int maxHands = 1;
    private boolean useGPU = true;
    private float minDetectionConfidence = 0.5f;
    private float minTrackingConfidence = 0.5f;
    
    /**
     * Initialize the MediaPipe Hands pipeline
     */
    public boolean initialize(Context context, int maxHands, boolean useGPU, 
                            float minDetectionConfidence, float minTrackingConfidence) {
        try {
            this.context = context;
            this.maxHands = maxHands;
            this.useGPU = useGPU;
            this.minDetectionConfidence = minDetectionConfidence;
            this.minTrackingConfidence = minTrackingConfidence;
            
            // Initialize MediaPipe
            AndroidAssetUtil.initializeNativeAssetManager(context);
            
            // Initialize EGL context for GPU processing
            if (useGPU) {
                eglManager = new EglManager(null);
            }
            
            // Create packet creator
            packetCreator = new AndroidPacketCreator();
            
            // Initialize frame processor
            processor = new FrameProcessor(
                context,
                useGPU ? eglManager.getNativeContext() : 0,
                BINARY_GRAPH_NAME,
                INPUT_VIDEO_STREAM_NAME,
                OUTPUT_LANDMARKS_STREAM_NAME
            );
            
            // Set up graph options
            Map<String, Packet> inputSidePackets = new HashMap<>();
            inputSidePackets.put("num_hands", packetCreator.createInt32(maxHands));
            inputSidePackets.put("min_detection_confidence", packetCreator.createFloat32(minDetectionConfidence));
            inputSidePackets.put("min_tracking_confidence", packetCreator.createFloat32(minTrackingConfidence));
            
            processor.setInputSidePackets(inputSidePackets);
            
            // Set up output packet callback
            processor.addPacketCallback(OUTPUT_LANDMARKS_STREAM_NAME, this::onHandLandmarks);
            
            // Start the processor
            processor.startCamera();
            
            initialized = true;
            Log.d(TAG, "MediaPipe Hands initialized successfully");
            return true;
            
        } catch (Exception e) {
            Log.e(TAG, "Failed to initialize MediaPipe Hands", e);
            return false;
        }
    }
    
    /**
     * Process a camera frame for hand detection
     */
    public void processFrame(byte[] imageBytes, int width, int height) {
        if (!initialized || processor == null) {
            Log.w(TAG, "MediaPipe not initialized");
            return;
        }
        
        try {
            // Convert byte array to Bitmap
            Bitmap bitmap = BitmapFactory.decodeByteArray(imageBytes, 0, imageBytes.length);
            if (bitmap == null) {
                Log.e(TAG, "Failed to decode image bytes");
                return;
            }
            
            // Resize bitmap if necessary
            if (bitmap.getWidth() != width || bitmap.getHeight() != height) {
                bitmap = Bitmap.createScaledBitmap(bitmap, width, height, true);
            }
            
            // Send frame to MediaPipe
            long timestamp = System.currentTimeMillis() * 1000; // Convert to microseconds
            processor.onNewFrame(bitmap, timestamp);
            
        } catch (Exception e) {
            Log.e(TAG, "Error processing frame", e);
        }
    }
    
    /**
     * Callback for receiving hand landmarks from MediaPipe
     */
    private void onHandLandmarks(Packet packet) {
        try {
            List<com.google.mediapipe.formats.proto.LandmarkProto.NormalizedLandmarkList> handLandmarkLists =
                PacketGetter.getProtoVector(packet, com.google.mediapipe.formats.proto.LandmarkProto.NormalizedLandmarkList.parser());
            
            if (handLandmarkLists.isEmpty()) {
                return;
            }
            
            // Process each detected hand
            for (com.google.mediapipe.formats.proto.LandmarkProto.NormalizedLandmarkList landmarks : handLandmarkLists) {
                HandLandmarksData landmarksData = convertToHandLandmarksData(landmarks);
                String json = convertToJson(landmarksData);
                
                // Send landmarks to Unity
                if (unityCallbackObject != null) {
                    UnityPlayer.UnitySendMessage(unityCallbackObject, "OnHandLandmarksReceived", json);
                }
            }
            
        } catch (Exception e) {
            Log.e(TAG, "Error processing hand landmarks", e);
        }
    }
    
    /**
     * Convert MediaPipe landmarks to our data structure
     */
    private HandLandmarksData convertToHandLandmarksData(
        com.google.mediapipe.formats.proto.LandmarkProto.NormalizedLandmarkList landmarks) {
        
        HandLandmarksData data = new HandLandmarksData();
        data.keypoints = new float[landmarks.getLandmarkCount() * 3]; // x, y, z for each landmark
        data.confidence = 0.8f; // Default confidence
        data.isRight = true; // Default to right hand
        
        for (int i = 0; i < landmarks.getLandmarkCount(); i++) {
            com.google.mediapipe.formats.proto.LandmarkProto.NormalizedLandmark landmark = landmarks.getLandmark(i);
            int baseIndex = i * 3;
            data.keypoints[baseIndex] = landmark.getX();
            data.keypoints[baseIndex + 1] = landmark.getY();
            data.keypoints[baseIndex + 2] = landmark.getZ();
        }
        
        return data;
    }
    
    /**
     * Convert landmarks data to JSON string
     */
    private String convertToJson(HandLandmarksData data) {
        StringBuilder json = new StringBuilder();
        json.append("{");
        json.append("\"keypoints\":[");
        
        for (int i = 0; i < data.keypoints.length; i += 3) {
            if (i > 0) json.append(",");
            json.append("{\"x\":").append(data.keypoints[i])
                .append(",\"y\":").append(data.keypoints[i + 1])
                .append(",\"z\":").append(data.keypoints[i + 2])
                .append("}");
        }
        
        json.append("],");
        json.append("\"confidence\":").append(data.confidence).append(",");
        json.append("\"isRight\":").append(data.isRight);
        json.append("}");
        
        return json.toString();
    }
    
    /**
     * Set the Unity callback object name
     */
    public void setLandmarksCallback(String objectName) {
        this.unityCallbackObject = objectName;
        Log.d(TAG, "Unity callback object set to: " + objectName);
    }
    
    /**
     * Update detection confidence threshold
     */
    public void setDetectionConfidence(float confidence) {
        this.minDetectionConfidence = confidence;
        // Update graph if running
        if (initialized && processor != null) {
            Map<String, Packet> inputSidePackets = new HashMap<>();
            inputSidePackets.put("min_detection_confidence", packetCreator.createFloat32(confidence));
            processor.setInputSidePackets(inputSidePackets);
        }
    }
    
    /**
     * Update tracking confidence threshold
     */
    public void setTrackingConfidence(float confidence) {
        this.minTrackingConfidence = confidence;
        // Update graph if running
        if (initialized && processor != null) {
            Map<String, Packet> inputSidePackets = new HashMap<>();
            inputSidePackets.put("min_tracking_confidence", packetCreator.createFloat32(confidence));
            processor.setInputSidePackets(inputSidePackets);
        }
    }
    
    /**
     * Update maximum number of hands to detect
     */
    public void setMaxHands(int maxHands) {
        this.maxHands = maxHands;
        // Update graph if running
        if (initialized && processor != null) {
            Map<String, Packet> inputSidePackets = new HashMap<>();
            inputSidePackets.put("num_hands", packetCreator.createInt32(maxHands));
            processor.setInputSidePackets(inputSidePackets);
        }
    }
    
    /**
     * Clean up resources
     */
    public void cleanup() {
        try {
            if (processor != null) {
                processor.close();
                processor = null;
            }
            
            if (eglManager != null) {
                eglManager.release();
                eglManager = null;
            }
            
            if (packetCreator != null) {
                packetCreator.release();
                packetCreator = null;
            }
            
            initialized = false;
            Log.d(TAG, "MediaPipe Hands cleaned up");
            
        } catch (Exception e) {
            Log.e(TAG, "Error during cleanup", e);
        }
    }
    
    /**
     * Data structure for hand landmarks
     */
    private static class HandLandmarksData {
        public float[] keypoints; // Array of x, y, z coordinates
        public float confidence;
        public boolean isRight;
    }
}

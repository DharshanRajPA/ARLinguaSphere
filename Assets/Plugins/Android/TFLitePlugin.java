package com.arlinguasphere;

import android.util.Log;

/**
 * Java bridge for TensorFlow Lite Unity plugin
 * Provides JNI interface for native TensorFlow Lite operations
 */
public class TFLitePlugin {
    private static final String TAG = "TFLitePlugin";
    
    static {
        try {
            System.loadLibrary("tflite_unity");
            Log.d(TAG, "TensorFlow Lite native library loaded successfully");
        } catch (UnsatisfiedLinkError e) {
            Log.e(TAG, "Failed to load TensorFlow Lite native library", e);
        }
    }
    
    /**
     * Create a new TensorFlow Lite interpreter from model data
     * @param modelData The model data as byte array
     * @return Pointer to the interpreter (0 if failed)
     */
    public static native long createInterpreter(byte[] modelData);
    
    /**
     * Destroy the TensorFlow Lite interpreter
     * @param interpreterPtr Pointer to the interpreter
     */
    public static native void destroyInterpreter(long interpreterPtr);
    
    /**
     * Set input tensor data (float array)
     * @param interpreterPtr Pointer to the interpreter
     * @param inputIndex Index of the input tensor
     * @param data Input data as float array
     * @param dataSize Size of the data array
     * @return 0 if successful, -1 if failed
     */
    public static native int setInputTensor(long interpreterPtr, int inputIndex, float[] data, int dataSize);
    
    /**
     * Set input tensor data (byte array)
     * @param interpreterPtr Pointer to the interpreter
     * @param inputIndex Index of the input tensor
     * @param data Input data as byte array
     * @param dataSize Size of the data array
     * @return 0 if successful, -1 if failed
     */
    public static native int setInputTensorBytes(long interpreterPtr, int inputIndex, byte[] data, int dataSize);
    
    /**
     * Run inference
     * @param interpreterPtr Pointer to the interpreter
     * @return 0 if successful, -1 if failed
     */
    public static native int invoke(long interpreterPtr);
    
    /**
     * Get output tensor data
     * @param interpreterPtr Pointer to the interpreter
     * @param outputIndex Index of the output tensor
     * @param output Output array to fill
     * @param outputSize Size of the output array
     * @return 0 if successful, -1 if failed
     */
    public static native int getOutputTensor(long interpreterPtr, int outputIndex, float[] output, int outputSize);
    
    /**
     * Get number of input tensors
     * @param interpreterPtr Pointer to the interpreter
     * @return Number of input tensors
     */
    public static native int getInputTensorCount(long interpreterPtr);
    
    /**
     * Get number of output tensors
     * @param interpreterPtr Pointer to the interpreter
     * @return Number of output tensors
     */
    public static native int getOutputTensorCount(long interpreterPtr);
    
    /**
     * Get input tensor shape
     * @param interpreterPtr Pointer to the interpreter
     * @param inputIndex Index of the input tensor
     * @param shape Array to fill with shape information
     */
    public static native void getInputTensorShape(long interpreterPtr, int inputIndex, int[] shape);
    
    /**
     * Get output tensor shape
     * @param interpreterPtr Pointer to the interpreter
     * @param outputIndex Index of the output tensor
     * @param shape Array to fill with shape information
     */
    public static native void getOutputTensorShape(long interpreterPtr, int outputIndex, int[] shape);
}

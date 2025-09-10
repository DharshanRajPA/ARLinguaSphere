/**
 * TensorFlow Lite Unity Plugin for Android
 * Provides native TensorFlow Lite inference capabilities for ARLinguaSphere
 */

#include <jni.h>
#include <android/log.h>
#include <tensorflow/lite/interpreter.h>
#include <tensorflow/lite/kernels/register.h>
#include <tensorflow/lite/model.h>
#include <tensorflow/lite/optional_debug_tools.h>
#include <memory>
#include <vector>

#define LOG_TAG "TFLiteUnity"
#define LOGD(...) __android_log_print(ANDROID_LOG_DEBUG, LOG_TAG, __VA_ARGS__)
#define LOGE(...) __android_log_print(ANDROID_LOG_ERROR, LOG_TAG, __VA_ARGS__)

struct TFLiteContext {
    std::unique_ptr<tflite::FlatBufferModel> model;
    std::unique_ptr<tflite::Interpreter> interpreter;
    tflite::ops::builtin::BuiltinOpResolver resolver;
};

extern "C" {

JNIEXPORT jlong JNICALL
Java_com_arlinguasphere_TFLitePlugin_createInterpreter(JNIEnv *env, jobject /* this */, jbyteArray modelData) {
    jsize modelSize = env->GetArrayLength(modelData);
    jbyte* modelBytes = env->GetByteArrayElements(modelData, nullptr);
    
    if (modelBytes == nullptr) {
        LOGE("Failed to get model bytes");
        return 0;
    }
    
    try {
        auto context = std::make_unique<TFLiteContext>();
        
        // Create model from buffer
        context->model = tflite::FlatBufferModel::BuildFromBuffer(
            reinterpret_cast<const char*>(modelBytes), modelSize);
        
        if (!context->model) {
            LOGE("Failed to create TensorFlow Lite model");
            env->ReleaseByteArrayElements(modelData, modelBytes, JNI_ABORT);
            return 0;
        }
        
        // Build interpreter
        tflite::InterpreterBuilder builder(*context->model, context->resolver);
        builder(&context->interpreter);
        
        if (!context->interpreter) {
            LOGE("Failed to create TensorFlow Lite interpreter");
            env->ReleaseByteArrayElements(modelData, modelBytes, JNI_ABORT);
            return 0;
        }
        
        // Allocate tensors
        if (context->interpreter->AllocateTensors() != kTfLiteOk) {
            LOGE("Failed to allocate tensors");
            env->ReleaseByteArrayElements(modelData, modelBytes, JNI_ABORT);
            return 0;
        }
        
        // Set thread count for performance
        context->interpreter->SetNumThreads(4);
        
        LOGD("TensorFlow Lite interpreter created successfully");
        
        env->ReleaseByteArrayElements(modelData, modelBytes, JNI_ABORT);
        return reinterpret_cast<jlong>(context.release());
        
    } catch (const std::exception& e) {
        LOGE("Exception creating interpreter: %s", e.what());
        env->ReleaseByteArrayElements(modelData, modelBytes, JNI_ABORT);
        return 0;
    }
}

JNIEXPORT void JNICALL
Java_com_arlinguasphere_TFLitePlugin_destroyInterpreter(JNIEnv *env, jobject /* this */, jlong interpreterPtr) {
    if (interpreterPtr != 0) {
        auto context = reinterpret_cast<TFLiteContext*>(interpreterPtr);
        delete context;
        LOGD("TensorFlow Lite interpreter destroyed");
    }
}

JNIEXPORT jint JNICALL
Java_com_arlinguasphere_TFLitePlugin_setInputTensor(JNIEnv *env, jobject /* this */, 
                                                    jlong interpreterPtr, jint inputIndex, 
                                                    jfloatArray data, jint dataSize) {
    if (interpreterPtr == 0) {
        LOGE("Invalid interpreter pointer");
        return -1;
    }
    
    auto context = reinterpret_cast<TFLiteContext*>(interpreterPtr);
    
    try {
        TfLiteTensor* input_tensor = context->interpreter->input_tensor(inputIndex);
        if (!input_tensor) {
            LOGE("Failed to get input tensor %d", inputIndex);
            return -1;
        }
        
        jfloat* inputData = env->GetFloatArrayElements(data, nullptr);
        if (!inputData) {
            LOGE("Failed to get input data");
            return -1;
        }
        
        // Copy data to tensor
        memcpy(input_tensor->data.f, inputData, dataSize * sizeof(float));
        
        env->ReleaseFloatArrayElements(data, inputData, JNI_ABORT);
        return 0;
        
    } catch (const std::exception& e) {
        LOGE("Exception setting input tensor: %s", e.what());
        return -1;
    }
}

JNIEXPORT jint JNICALL
Java_com_arlinguasphere_TFLitePlugin_invoke(JNIEnv *env, jobject /* this */, jlong interpreterPtr) {
    if (interpreterPtr == 0) {
        LOGE("Invalid interpreter pointer");
        return -1;
    }
    
    auto context = reinterpret_cast<TFLiteContext*>(interpreterPtr);
    
    try {
        if (context->interpreter->Invoke() != kTfLiteOk) {
            LOGE("Failed to invoke interpreter");
            return -1;
        }
        
        return 0;
        
    } catch (const std::exception& e) {
        LOGE("Exception during inference: %s", e.what());
        return -1;
    }
}

JNIEXPORT jint JNICALL
Java_com_arlinguasphere_TFLitePlugin_getOutputTensor(JNIEnv *env, jobject /* this */, 
                                                     jlong interpreterPtr, jint outputIndex, 
                                                     jfloatArray output, jint outputSize) {
    if (interpreterPtr == 0) {
        LOGE("Invalid interpreter pointer");
        return -1;
    }
    
    auto context = reinterpret_cast<TFLiteContext*>(interpreterPtr);
    
    try {
        const TfLiteTensor* output_tensor = context->interpreter->output_tensor(outputIndex);
        if (!output_tensor) {
            LOGE("Failed to get output tensor %d", outputIndex);
            return -1;
        }
        
        jfloat* outputData = env->GetFloatArrayElements(output, nullptr);
        if (!outputData) {
            LOGE("Failed to get output array");
            return -1;
        }
        
        // Copy tensor data to output array
        memcpy(outputData, output_tensor->data.f, outputSize * sizeof(float));
        
        env->ReleaseFloatArrayElements(output, outputData, 0);
        return 0;
        
    } catch (const std::exception& e) {
        LOGE("Exception getting output tensor: %s", e.what());
        return -1;
    }
}

JNIEXPORT jint JNICALL
Java_com_arlinguasphere_TFLitePlugin_getInputTensorCount(JNIEnv *env, jobject /* this */, jlong interpreterPtr) {
    if (interpreterPtr == 0) {
        return 0;
    }
    
    auto context = reinterpret_cast<TFLiteContext*>(interpreterPtr);
    return context->interpreter->inputs().size();
}

JNIEXPORT jint JNICALL
Java_com_arlinguasphere_TFLitePlugin_getOutputTensorCount(JNIEnv *env, jobject /* this */, jlong interpreterPtr) {
    if (interpreterPtr == 0) {
        return 0;
    }
    
    auto context = reinterpret_cast<TFLiteContext*>(interpreterPtr);
    return context->interpreter->outputs().size();
}

JNIEXPORT void JNICALL
Java_com_arlinguasphere_TFLitePlugin_getInputTensorShape(JNIEnv *env, jobject /* this */, 
                                                         jlong interpreterPtr, jint inputIndex, 
                                                         jintArray shape) {
    if (interpreterPtr == 0) {
        return;
    }
    
    auto context = reinterpret_cast<TFLiteContext*>(interpreterPtr);
    
    try {
        const TfLiteTensor* input_tensor = context->interpreter->input_tensor(inputIndex);
        if (!input_tensor) {
            return;
        }
        
        jint* shapeData = env->GetIntArrayElements(shape, nullptr);
        if (!shapeData) {
            return;
        }
        
        for (int i = 0; i < input_tensor->dims->size && i < env->GetArrayLength(shape); i++) {
            shapeData[i] = input_tensor->dims->data[i];
        }
        
        env->ReleaseIntArrayElements(shape, shapeData, 0);
        
    } catch (const std::exception& e) {
        LOGE("Exception getting input tensor shape: %s", e.what());
    }
}

JNIEXPORT void JNICALL
Java_com_arlinguasphere_TFLitePlugin_getOutputTensorShape(JNIEnv *env, jobject /* this */, 
                                                          jlong interpreterPtr, jint outputIndex, 
                                                          jintArray shape) {
    if (interpreterPtr == 0) {
        return;
    }
    
    auto context = reinterpret_cast<TFLiteContext*>(interpreterPtr);
    
    try {
        const TfLiteTensor* output_tensor = context->interpreter->output_tensor(outputIndex);
        if (!output_tensor) {
            return;
        }
        
        jint* shapeData = env->GetIntArrayElements(shape, nullptr);
        if (!shapeData) {
            return;
        }
        
        for (int i = 0; i < output_tensor->dims->size && i < env->GetArrayLength(shape); i++) {
            shapeData[i] = output_tensor->dims->data[i];
        }
        
        env->ReleaseIntArrayElements(shape, shapeData, 0);
        
    } catch (const std::exception& e) {
        LOGE("Exception getting output tensor shape: %s", e.what());
    }
}

} // extern "C"

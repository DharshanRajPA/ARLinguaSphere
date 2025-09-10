package com.arlinguasphere;

import android.Manifest;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.speech.RecognitionListener;
import android.speech.RecognizerIntent;
import android.speech.SpeechRecognizer;
import android.speech.tts.TextToSpeech;
import android.speech.tts.UtteranceProgressListener;
import android.util.Log;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.unity3d.player.UnityPlayer;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Locale;

/**
 * Android Speech Plugin for Unity
 * Provides Speech-to-Text and Text-to-Speech functionality
 */
public class SpeechPlugin implements RecognitionListener, TextToSpeech.OnInitListener {
    private static final String TAG = "SpeechPlugin";
    private static final int PERMISSION_REQUEST_CODE = 1001;
    
    private Context context;
    private String unityCallbackObject;
    
    // Speech Recognition
    private SpeechRecognizer speechRecognizer;
    private Intent recognizerIntent;
    private boolean isListening = false;
    
    // Text-to-Speech
    private TextToSpeech textToSpeech;
    private boolean ttsInitialized = false;
    private boolean isSpeaking = false;
    
    // Settings
    private float speechRate = 1.0f;
    private float speechPitch = 1.0f;
    private float speechVolume = 1.0f;
    private String currentLanguage = "en-US";
    
    public boolean initialize(Context context, String callbackObjectName) {
        try {
            this.context = context;
            this.unityCallbackObject = callbackObjectName;
            
            // Initialize Speech Recognition
            if (SpeechRecognizer.isRecognitionAvailable(context)) {
                speechRecognizer = SpeechRecognizer.createSpeechRecognizer(context);
                speechRecognizer.setRecognitionListener(this);
                
                recognizerIntent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
                recognizerIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
                recognizerIntent.putExtra(RecognizerIntent.EXTRA_PARTIAL_RESULTS, true);
                recognizerIntent.putExtra(RecognizerIntent.EXTRA_MAX_RESULTS, 1);
                
                Log.d(TAG, "Speech Recognition initialized");
            } else {
                Log.e(TAG, "Speech Recognition not available");
            }
            
            // Initialize Text-to-Speech
            textToSpeech = new TextToSpeech(context, this);
            
            Log.d(TAG, "Speech Plugin initialized successfully");
            return true;
            
        } catch (Exception e) {
            Log.e(TAG, "Failed to initialize Speech Plugin", e);
            return false;
        }
    }
    
    public boolean startListening(String language) {
        if (speechRecognizer == null) {
            Log.e(TAG, "Speech recognizer not initialized");
            return false;
        }
        
        if (!hasPermissions()) {
            Log.w(TAG, "Missing audio recording permission");
            requestPermissions();
            return false;
        }
        
        if (isListening) {
            Log.w(TAG, "Already listening");
            return true;
        }
        
        try {
            // Set language
            Locale locale = parseLanguageCode(language);
            recognizerIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE, locale.toString());
            recognizerIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_PREFERENCE, locale.toString());
            
            // Start listening
            speechRecognizer.startListening(recognizerIntent);
            isListening = true;
            currentLanguage = language;
            
            Log.d(TAG, "Started listening in language: " + language);
            return true;
            
        } catch (Exception e) {
            Log.e(TAG, "Error starting speech recognition", e);
            return false;
        }
    }
    
    public void stopListening() {
        if (speechRecognizer != null && isListening) {
            speechRecognizer.stopListening();
            isListening = false;
            Log.d(TAG, "Stopped listening");
        }
    }
    
    public boolean speak(String text, String language, float speechRate, float pitch, float volume) {
        if (!ttsInitialized) {
            Log.e(TAG, "TTS not initialized");
            return false;
        }
        
        if (text == null || text.trim().isEmpty()) {
            Log.w(TAG, "Cannot speak empty text");
            return false;
        }
        
        try {
            // Set language
            Locale locale = parseLanguageCode(language);
            int result = textToSpeech.setLanguage(locale);
            
            if (result == TextToSpeech.LANG_MISSING_DATA || result == TextToSpeech.LANG_NOT_SUPPORTED) {
                Log.w(TAG, "Language not supported: " + language);
                textToSpeech.setLanguage(Locale.US);
            }
            
            // Set speech parameters
            textToSpeech.setSpeechRate(speechRate);
            textToSpeech.setPitch(pitch);
            
            // Create utterance parameters
            HashMap<String, String> params = new HashMap<>();
            params.put(TextToSpeech.Engine.KEY_PARAM_UTTERANCE_ID, "utterance_" + System.currentTimeMillis());
            params.put(TextToSpeech.Engine.KEY_PARAM_VOLUME, String.valueOf(volume));
            
            // Speak the text
            int speakResult = textToSpeech.speak(text, TextToSpeech.QUEUE_FLUSH, params);
            
            if (speakResult == TextToSpeech.SUCCESS) {
                isSpeaking = true;
                this.speechRate = speechRate;
                this.speechPitch = pitch;
                this.speechVolume = volume;
                
                Log.d(TAG, "Started speaking: " + text);
                sendUnityMessage("OnTTSStarted", "");
                return true;
            } else {
                Log.e(TAG, "Failed to start speaking");
                return false;
            }
            
        } catch (Exception e) {
            Log.e(TAG, "Error speaking text", e);
            return false;
        }
    }
    
    public void stopSpeaking() {
        if (textToSpeech != null && isSpeaking) {
            textToSpeech.stop();
            isSpeaking = false;
            Log.d(TAG, "Stopped speaking");
            sendUnityMessage("OnTTSEnded", "");
        }
    }
    
    public boolean hasPermissions() {
        return ContextCompat.checkSelfPermission(context, Manifest.permission.RECORD_AUDIO) 
               == PackageManager.PERMISSION_GRANTED;
    }
    
    public void requestPermissions() {
        try {
            if (context instanceof android.app.Activity) {
                ActivityCompat.requestPermissions(
                    (android.app.Activity) context,
                    new String[]{Manifest.permission.RECORD_AUDIO},
                    PERMISSION_REQUEST_CODE
                );
            }
        } catch (Exception e) {
            Log.e(TAG, "Error requesting permissions", e);
        }
    }
    
    public void cleanup() {
        try {
            stopListening();
            stopSpeaking();
            
            if (speechRecognizer != null) {
                speechRecognizer.destroy();
                speechRecognizer = null;
            }
            
            if (textToSpeech != null) {
                textToSpeech.shutdown();
                textToSpeech = null;
            }
            
            Log.d(TAG, "Speech Plugin cleaned up");
            
        } catch (Exception e) {
            Log.e(TAG, "Error during cleanup", e);
        }
    }
    
    // RecognitionListener implementation
    @Override
    public void onReadyForSpeech(Bundle params) {
        sendUnityMessage("OnSpeechStarted", "");
    }
    
    @Override
    public void onBeginningOfSpeech() {
        Log.d(TAG, "Speech input detected");
    }
    
    @Override
    public void onRmsChanged(float rmsdB) {
        // Voice level visualization
    }
    
    @Override
    public void onBufferReceived(byte[] buffer) {
        // Not used
    }
    
    @Override
    public void onEndOfSpeech() {
        Log.d(TAG, "End of speech detected");
        isListening = false;
    }
    
    @Override
    public void onError(int error) {
        String errorMessage = getSpeechErrorMessage(error);
        Log.e(TAG, "Speech recognition error: " + errorMessage);
        isListening = false;
        sendUnityMessage("OnSpeechError", errorMessage);
    }
    
    @Override
    public void onResults(Bundle results) {
        ArrayList<String> matches = results.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION);
        if (matches != null && !matches.isEmpty()) {
            String recognizedText = matches.get(0);
            Log.d(TAG, "Speech recognized: " + recognizedText);
            sendUnityMessage("OnSpeechRecognized", recognizedText);
        }
        isListening = false;
        sendUnityMessage("OnSpeechEnded", "");
    }
    
    @Override
    public void onPartialResults(Bundle partialResults) {
        ArrayList<String> matches = partialResults.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION);
        if (matches != null && !matches.isEmpty()) {
            String partialText = matches.get(0);
            sendUnityMessage("OnPartialSpeechResult", partialText);
        }
    }
    
    @Override
    public void onEvent(int eventType, Bundle params) {
        // Not used
    }
    
    // TextToSpeech.OnInitListener implementation
    @Override
    public void onInit(int status) {
        if (status == TextToSpeech.SUCCESS) {
            ttsInitialized = true;
            
            textToSpeech.setOnUtteranceProgressListener(new UtteranceProgressListener() {
                @Override
                public void onStart(String utteranceId) {
                    isSpeaking = true;
                    sendUnityMessage("OnTTSStarted", "");
                }
                
                @Override
                public void onDone(String utteranceId) {
                    isSpeaking = false;
                    sendUnityMessage("OnTTSEnded", "");
                }
                
                @Override
                public void onError(String utteranceId) {
                    isSpeaking = false;
                    sendUnityMessage("OnTTSError", "TTS playback error");
                }
            });
            
            Log.d(TAG, "TTS initialized successfully");
        } else {
            Log.e(TAG, "TTS initialization failed");
            ttsInitialized = false;
        }
    }
    
    private Locale parseLanguageCode(String languageCode) {
        try {
            if (languageCode.contains("-")) {
                String[] parts = languageCode.split("-");
                return new Locale(parts[0], parts[1]);
            } else {
                return new Locale(languageCode);
            }
        } catch (Exception e) {
            Log.w(TAG, "Invalid language code: " + languageCode + ", using default");
            return Locale.getDefault();
        }
    }
    
    private String getSpeechErrorMessage(int error) {
        switch (error) {
            case SpeechRecognizer.ERROR_AUDIO:
                return "Audio recording error";
            case SpeechRecognizer.ERROR_CLIENT:
                return "Client side error";
            case SpeechRecognizer.ERROR_INSUFFICIENT_PERMISSIONS:
                return "Insufficient permissions";
            case SpeechRecognizer.ERROR_NETWORK:
                return "Network error";
            case SpeechRecognizer.ERROR_NETWORK_TIMEOUT:
                return "Network timeout";
            case SpeechRecognizer.ERROR_NO_MATCH:
                return "No speech input matched";
            case SpeechRecognizer.ERROR_RECOGNIZER_BUSY:
                return "Recognition service busy";
            case SpeechRecognizer.ERROR_SERVER:
                return "Server error";
            case SpeechRecognizer.ERROR_SPEECH_TIMEOUT:
                return "No speech input";
            default:
                return "Unknown error: " + error;
        }
    }
    
    private void sendUnityMessage(String methodName, String message) {
        if (unityCallbackObject != null) {
            try {
                UnityPlayer.UnitySendMessage(unityCallbackObject, methodName, message);
            } catch (Exception e) {
                Log.e(TAG, "Error sending Unity message: " + methodName, e);
            }
        }
    }
}
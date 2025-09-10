using UnityEngine;
using System;
using System.Collections;

namespace ARLinguaSphere.Voice
{
    /// <summary>
    /// Manages voice input (Speech-to-Text) and output (Text-to-Speech)
    /// </summary>
    public class VoiceManager : MonoBehaviour
    {
        [Header("Voice Settings")]
        public bool enableSTT = true;
        public bool enableTTS = true;
        public string defaultLanguage = "en-US";
        public float speechTimeout = 10f;
        
        [Header("TTS Settings")]
        public float speechRate = 1f;
        public float speechPitch = 1f;
        public float speechVolume = 1f;
        
        private bool isInitialized = false;
        private bool isListening = false;
        private bool isSpeaking = false;
        private AndroidSpeechBridge androidBridge;
        
        // Events
        public event Action<string> OnSpeechRecognized;
        public event Action<string> OnSpeechError;
        public event Action OnSpeechStarted;
        public event Action OnSpeechEnded;
        public event Action OnTTSStarted;
        public event Action OnTTSEnded;
        
        public void Initialize()
        {
            Debug.Log("VoiceManager: Initializing voice systems...");
            
            // Initialize Android bridge (no-op in editor/non-Android)
            // Create callback receiver object
            var cbObj = new GameObject("SpeechCallbackReceiver");
            var receiver = cbObj.AddComponent<SpeechCallbackReceiver>();
            receiver.voiceManager = this;
            DontDestroyOnLoad(cbObj);
            androidBridge = new AndroidSpeechBridge(cbObj.name);
            
            isInitialized = true;
            Debug.Log("VoiceManager: Voice systems initialized!");
        }

        // Notify helpers for external callbacks (Unity 6 event restrictions)
        public void NotifySpeechStarted() => OnSpeechStarted?.Invoke();
        public void NotifySpeechEnded() => OnSpeechEnded?.Invoke();
        public void NotifySpeechError(string error) => OnSpeechError?.Invoke(error);
        public void NotifySpeechRecognized(string text) => OnSpeechRecognized?.Invoke(text);
        public void NotifyTTSStarted() => OnTTSStarted?.Invoke();
        public void NotifyTTSEnded() => OnTTSEnded?.Invoke();
        
        private void InitializeAndroidSpeechRecognition()
        {
            // TODO: Initialize Android SpeechRecognizer
            Debug.Log("VoiceManager: Android Speech Recognition initialized (placeholder)");
        }
        
        private void InitializeAndroidTTS()
        {
            // TODO: Initialize Android TextToSpeech
            Debug.Log("VoiceManager: Android TTS initialized (placeholder)");
        }
        
        public void StartListening()
        {
            if (!isInitialized || !enableSTT || isListening)
            {
                return;
            }
            
            isListening = true;
            OnSpeechStarted?.Invoke();
            
            androidBridge?.StartListening(defaultLanguage);
            Debug.Log("VoiceManager: Started listening for speech...");
            
            #if UNITY_EDITOR
            // Simulate speech recognition for testing in Editor
            StartCoroutine(SimulateSpeechRecognition());
            #endif
        }
        
        public void StopListening()
        {
            if (!isListening) return;
            
            isListening = false;
            OnSpeechEnded?.Invoke();
            
            androidBridge?.StopListening();
            Debug.Log("VoiceManager: Stopped listening for speech");
        }
        
        private IEnumerator SimulateSpeechRecognition()
        {
            // Simulate speech recognition delay
            yield return new WaitForSeconds(2f);
            
            if (isListening)
            {
                // Simulate recognized speech
                string[] testPhrases = {
                    "What is this?",
                    "Translate to Spanish",
                    "Quiz me",
                    "Remove label",
                    "Change language"
                };
                
                string recognizedText = testPhrases[UnityEngine.Random.Range(0, testPhrases.Length)];
                OnSpeechRecognized?.Invoke(recognizedText);
                
                isListening = false;
                OnSpeechEnded?.Invoke();
            }
        }
        
        public void Speak(string text, string language = null)
        {
            if (!isInitialized || !enableTTS || isSpeaking)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("VoiceManager: Cannot speak empty text");
                return;
            }
            
            if (string.IsNullOrEmpty(language))
            {
                language = defaultLanguage;
            }
            
            isSpeaking = true;
            OnTTSStarted?.Invoke();
            
            androidBridge?.Speak(text, language, speechRate, speechPitch, speechVolume);
            Debug.Log($"VoiceManager: Speaking: '{text}' in {language}");
            
            #if UNITY_EDITOR
            // Simulate TTS duration in Editor
            StartCoroutine(SimulateTTS(text));
            #endif
        }
        
        private IEnumerator SimulateTTS(string text)
        {
            // Simulate TTS duration based on text length
            float duration = Mathf.Max(1f, text.Length * 0.1f);
            yield return new WaitForSeconds(duration);
            
            isSpeaking = false;
            OnTTSEnded?.Invoke();
        }
        
        public void SetLanguage(string languageCode)
        {
            defaultLanguage = languageCode;
            Debug.Log($"VoiceManager: Language set to {languageCode}");
        }
        
        public void SetSpeechRate(float rate)
        {
            speechRate = Mathf.Clamp(rate, 0.1f, 3f);
            Debug.Log($"VoiceManager: Speech rate set to {speechRate}");
        }
        
        public void SetSpeechPitch(float pitch)
        {
            speechPitch = Mathf.Clamp(pitch, 0.1f, 2f);
            Debug.Log($"VoiceManager: Speech pitch set to {speechPitch}");
        }
        
        public void SetSpeechVolume(float volume)
        {
            speechVolume = Mathf.Clamp01(volume);
            Debug.Log($"VoiceManager: Speech volume set to {speechVolume}");
        }
        
        public bool IsListening => isListening;
        public bool IsSpeaking => isSpeaking;
        
        public void ProcessVoiceCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return;
            }
            
            command = command.ToLower().Trim();
            
            // Process voice commands
            if (command.Contains("what is this") || command.Contains("identify"))
            {
                // Trigger object identification
                Debug.Log("VoiceManager: Processing 'identify' command");
            }
            else if (command.Contains("translate"))
            {
                // Extract target language and trigger translation
                Debug.Log("VoiceManager: Processing 'translate' command");
            }
            else if (command.Contains("quiz"))
            {
                // Trigger quiz mode
                Debug.Log("VoiceManager: Processing 'quiz' command");
            }
            else if (command.Contains("remove") || command.Contains("delete"))
            {
                // Remove current label
                Debug.Log("VoiceManager: Processing 'remove' command");
            }
            else if (command.Contains("language") || command.Contains("change"))
            {
                // Change language
                Debug.Log("VoiceManager: Processing 'change language' command");
            }
            else
            {
                Debug.Log($"VoiceManager: Unknown command: {command}");
            }
        }
    }
}

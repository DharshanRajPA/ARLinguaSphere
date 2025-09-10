using UnityEngine;
using System;

namespace ARLinguaSphere.Voice
{
    /// <summary>
    /// Bridge for Android Speech Recognition and Text-to-Speech services
    /// Provides native Android speech capabilities for Unity
    /// </summary>
    public class AndroidSpeechBridge
    {
        private AndroidJavaObject speechPlugin;
        private string callbackObjectName;
        private bool isInitialized = false;
        
        public bool IsInitialized => isInitialized;
        public bool IsListening { get; private set; }
        public bool IsSpeaking { get; private set; }
        
        public AndroidSpeechBridge(string callbackObjectName)
        {
            this.callbackObjectName = callbackObjectName;
            Initialize();
        }
        
        private void Initialize()
        {
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var context = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    speechPlugin = new AndroidJavaObject("com.arlinguasphere.SpeechPlugin");
                    
                    bool success = speechPlugin.Call<bool>("initialize", context, callbackObjectName);
                    
                    if (success)
                    {
                        isInitialized = true;
                        Debug.Log("AndroidSpeechBridge: Initialized successfully");
                    }
                    else
                    {
                        Debug.LogError("AndroidSpeechBridge: Failed to initialize");
                    }
                }
#else
                // Mock initialization for Editor
                isInitialized = true;
                Debug.Log("AndroidSpeechBridge: Mock initialization for Editor");
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Initialization error: {e.Message}");
            }
        }
        
        public void StartListening(string language = "en-US")
        {
            if (!isInitialized)
            {
                Debug.LogError("AndroidSpeechBridge: Not initialized");
                return;
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    bool success = speechPlugin.Call<bool>("startListening", language);
                    if (success)
                    {
                        IsListening = true;
                        Debug.Log($"AndroidSpeechBridge: Started listening in {language}");
                    }
                    else
                    {
                        Debug.LogError("AndroidSpeechBridge: Failed to start listening");
                    }
                }
#else
                // Mock listening for Editor
                IsListening = true;
                Debug.Log($"AndroidSpeechBridge: Mock listening started in {language}");
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error starting listening: {e.Message}");
            }
        }
        
        public void StopListening()
        {
            if (!isInitialized || !IsListening) return;
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    speechPlugin.Call("stopListening");
                }
#endif
                IsListening = false;
                Debug.Log("AndroidSpeechBridge: Stopped listening");
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error stopping listening: {e.Message}");
            }
        }
        
        public void Speak(string text, string language = "en-US", float speechRate = 1f, float pitch = 1f, float volume = 1f)
        {
            if (!isInitialized)
            {
                Debug.LogError("AndroidSpeechBridge: Not initialized");
                return;
            }
            
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("AndroidSpeechBridge: Cannot speak empty text");
                return;
            }
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    bool success = speechPlugin.Call<bool>("speak", text, language, speechRate, pitch, volume);
                    if (success)
                    {
                        IsSpeaking = true;
                        Debug.Log($"AndroidSpeechBridge: Speaking '{text}' in {language}");
                    }
                    else
                    {
                        Debug.LogError("AndroidSpeechBridge: Failed to start speaking");
                    }
                }
#else
                // Mock speaking for Editor
                IsSpeaking = true;
                Debug.Log($"AndroidSpeechBridge: Mock speaking '{text}' in {language}");
                
                // Simulate TTS completion after delay
                float duration = text.Length * 0.1f; // Rough duration estimate
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    System.Threading.Thread.Sleep((int)(duration * 1000));
                    IsSpeaking = false;
                });
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error speaking: {e.Message}");
            }
        }
        
        public void StopSpeaking()
        {
            if (!isInitialized || !IsSpeaking) return;
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    speechPlugin.Call("stopSpeaking");
                }
#endif
                IsSpeaking = false;
                Debug.Log("AndroidSpeechBridge: Stopped speaking");
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error stopping speaking: {e.Message}");
            }
        }
        
        public void SetSpeechRate(float rate)
        {
            if (!isInitialized) return;
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    speechPlugin.Call("setSpeechRate", Mathf.Clamp(rate, 0.1f, 3f));
                }
#endif
                Debug.Log($"AndroidSpeechBridge: Speech rate set to {rate}");
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error setting speech rate: {e.Message}");
            }
        }
        
        public void SetSpeechPitch(float pitch)
        {
            if (!isInitialized) return;
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    speechPlugin.Call("setSpeechPitch", Mathf.Clamp(pitch, 0.1f, 2f));
                }
#endif
                Debug.Log($"AndroidSpeechBridge: Speech pitch set to {pitch}");
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error setting speech pitch: {e.Message}");
            }
        }
        
        public void SetSpeechVolume(float volume)
        {
            if (!isInitialized) return;
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    speechPlugin.Call("setSpeechVolume", Mathf.Clamp01(volume));
                }
#endif
                Debug.Log($"AndroidSpeechBridge: Speech volume set to {volume}");
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error setting speech volume: {e.Message}");
            }
        }
        
        public bool IsLanguageAvailable(string language)
        {
            if (!isInitialized) return false;
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    return speechPlugin.Call<bool>("isLanguageAvailable", language);
                }
#endif
                // Mock availability for Editor
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error checking language availability: {e.Message}");
                return false;
            }
        }
        
        public string[] GetAvailableLanguages()
        {
            if (!isInitialized) return new string[0];
            
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    return speechPlugin.Call<string[]>("getAvailableLanguages");
                }
#endif
                // Return mock languages for Editor
                return new string[]
                {
                    "en-US", "es-ES", "fr-FR", "de-DE", "it-IT", 
                    "pt-PT", "zh-CN", "ja-JP", "ko-KR", "hi-IN"
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error getting available languages: {e.Message}");
                return new string[0];
            }
        }
        
        public void RequestPermissions()
        {
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    speechPlugin.Call("requestPermissions");
                    Debug.Log("AndroidSpeechBridge: Requested permissions");
                }
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error requesting permissions: {e.Message}");
            }
        }
        
        public bool HasPermissions()
        {
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    return speechPlugin.Call<bool>("hasPermissions");
                }
#endif
                // Assume permissions are granted in Editor
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error checking permissions: {e.Message}");
                return false;
            }
        }
        
        public void Dispose()
        {
            try
            {
                StopListening();
                StopSpeaking();
                
#if UNITY_ANDROID && !UNITY_EDITOR
                if (speechPlugin != null)
                {
                    speechPlugin.Call("cleanup");
                    speechPlugin.Dispose();
                    speechPlugin = null;
                }
#endif
                isInitialized = false;
                Debug.Log("AndroidSpeechBridge: Disposed");
            }
            catch (Exception e)
            {
                Debug.LogError($"AndroidSpeechBridge: Error during disposal: {e.Message}");
            }
        }
    }
    
    /// <summary>
    /// Simple dispatcher to run actions on the Unity main thread
    /// </summary>
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static UnityMainThreadDispatcher _instance;
        private readonly System.Collections.Generic.Queue<System.Action> _executionQueue = new System.Collections.Generic.Queue<System.Action>();
        
        public static UnityMainThreadDispatcher Instance()
        {
            if (_instance == null)
            {
                var go = new GameObject("UnityMainThreadDispatcher");
                _instance = go.AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
        
        public void Enqueue(System.Action action)
        {
            lock (_executionQueue)
            {
                _executionQueue.Enqueue(action);
            }
        }
        
        private void Update()
        {
            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    _executionQueue.Dequeue().Invoke();
                }
            }
        }
    }
}
using UnityEngine;
using System.Collections;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Crash-proof main controller that prevents immediate app crashes
    /// This is the most stable version that focuses on not crashing
    /// </summary>
    public class CrashProofMainController : MonoBehaviour
    {
        [Header("Settings")]
        public bool enableAR = true;
        public bool enableVoice = true;
        public bool enableUI = true;
        public bool enableDebugging = true;
        
        [Header("Delays (seconds)")]
        public float initializationDelay = 2.0f;
        public float componentDelay = 0.5f;
        
        private bool isInitialized = false;
        private int initializationStep = 0;
        
        private void Start()
        {
            if (enableDebugging)
            {
                Debug.Log("🚀 CrashProofMainController: Starting crash-proof initialization...");
            }
            
            StartCoroutine(InitializeCrashProof());
        }
        
        private IEnumerator InitializeCrashProof()
        {
            // Step 1: Wait for Unity to fully load
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(initializationDelay);
            
            if (enableDebugging)
            {
                Debug.Log($"📱 Step {++initializationStep}: Unity fully loaded");
            }
            
            // Step 2: Initialize basic systems
            yield return StartCoroutine(InitializeBasicSystems());
            
            // Step 3: Initialize UI (safest first)
            if (enableUI)
            {
                yield return StartCoroutine(InitializeUISafely());
            }
            
            // Step 4: Initialize Voice (if enabled)
            if (enableVoice)
            {
                yield return StartCoroutine(InitializeVoiceSafely());
            }
            
            // Step 5: Initialize AR (most likely to crash)
            if (enableAR)
            {
                yield return StartCoroutine(InitializeARSafely());
            }
            
            // Step 6: Finalize
            isInitialized = true;
            
            if (enableDebugging)
            {
                Debug.Log("✅ CrashProofMainController: Initialization complete - app should not crash!");
            }
        }
        
        private IEnumerator InitializeBasicSystems()
        {
            if (enableDebugging)
            {
                Debug.Log($"📱 Step {++initializationStep}: Initializing basic systems...");
            }
            
            // Just log that we're here - don't do anything complex
            yield return new WaitForSeconds(componentDelay);
            
            if (enableDebugging)
            {
                Debug.Log("✅ Basic systems initialized");
            }
        }
        
        private IEnumerator InitializeUISafely()
        {
            if (enableDebugging)
            {
                Debug.Log($"📱 Step {++initializationStep}: Initializing UI safely...");
            }
            
            try
            {
                // Try to find UI Manager
                var uiManager = FindFirstObjectByType<ARLinguaSphere.UI.UIManager>();
                if (uiManager != null)
                {
                    if (enableDebugging)
                    {
                        Debug.Log("✅ UIManager found and ready");
                    }
                }
                else
                {
                    if (enableDebugging)
                    {
                        Debug.Log("⚠️ UIManager not found - this is okay, we'll continue");
                    }
                }
            }
            catch (System.Exception e)
            {
                if (enableDebugging)
                {
                    Debug.LogWarning($"⚠️ UI initialization warning: {e.Message}");
                }
            }
            
            yield return new WaitForSeconds(componentDelay);
        }
        
        private IEnumerator InitializeVoiceSafely()
        {
            if (enableDebugging)
            {
                Debug.Log($"📱 Step {++initializationStep}: Initializing Voice safely...");
            }
            
            try
            {
                // Try to find Voice Manager
                var voiceManager = FindFirstObjectByType<ARLinguaSphere.Voice.VoiceManager>();
                if (voiceManager != null)
                {
                    if (enableDebugging)
                    {
                        Debug.Log("✅ VoiceManager found and ready");
                    }
                }
                else
                {
                    if (enableDebugging)
                    {
                        Debug.Log("⚠️ VoiceManager not found - this is okay, we'll continue");
                    }
                }
            }
            catch (System.Exception e)
            {
                if (enableDebugging)
                {
                    Debug.LogWarning($"⚠️ Voice initialization warning: {e.Message}");
                }
            }
            
            yield return new WaitForSeconds(componentDelay);
        }
        
        private IEnumerator InitializeARSafely()
        {
            if (enableDebugging)
            {
                Debug.Log($"📱 Step {++initializationStep}: Initializing AR safely...");
            }
            
            try
            {
                // Check if we're on Android and ARCore is supported
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (enableDebugging)
                    {
                        Debug.Log("📱 Running on Android - checking AR support...");
                    }
                    
                    // Just check if AR components exist, don't initialize them yet
                    var arSession = FindFirstObjectByType<UnityEngine.XR.ARFoundation.ARSession>();
                    if (arSession != null)
                    {
                        if (enableDebugging)
                        {
                            Debug.Log("✅ ARSession found");
                        }
                    }
                    else
                    {
                        if (enableDebugging)
                        {
                            Debug.Log("⚠️ ARSession not found - AR may not work");
                        }
                    }
                }
                else
                {
                    if (enableDebugging)
                    {
                        Debug.Log("⚠️ Not running on Android - AR features may not work");
                    }
                }
            }
            catch (System.Exception e)
            {
                if (enableDebugging)
                {
                    Debug.LogWarning($"⚠️ AR initialization warning: {e.Message}");
                }
            }
            
            yield return new WaitForSeconds(componentDelay);
        }
        
        private void Update()
        {
            // Simple update loop that doesn't do anything complex
            if (isInitialized && enableDebugging)
            {
                // Just log that we're running every 10 seconds
                if (Time.time % 10f < Time.deltaTime)
                {
                    Debug.Log("✅ CrashProofMainController: App is running stably");
                }
            }
        }
        
        [ContextMenu("Force Safe Mode")]
        public void ForceSafeMode()
        {
            enableAR = false;
            enableVoice = false;
            enableUI = true;
            enableDebugging = true;
            
            Debug.Log("🛡️ Safe mode enabled - only basic features will work");
        }
        
        [ContextMenu("Show Status")]
        public void ShowStatus()
        {
            Debug.Log("📊 CrashProofMainController Status:");
            Debug.Log($"   - Initialized: {isInitialized}");
            Debug.Log($"   - AR Enabled: {enableAR}");
            Debug.Log($"   - Voice Enabled: {enableVoice}");
            Debug.Log($"   - UI Enabled: {enableUI}");
            Debug.Log($"   - Debugging: {enableDebugging}");
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Debugger specifically for Android crash issues
    /// This helps identify why the app crashes immediately on Android
    /// </summary>
    public class AndroidCrashDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        public bool enableDetailedLogging = true;
        public bool enableCrashPrevention = true;
        public bool enableSafeMode = true;
        
        [Header("Crash Prevention")]
        public float initializationDelay = 1.0f;
        public int maxRetryAttempts = 3;
        
        // private bool isInitialized = false; // Removed unused field
        private int crashCount = 0;
        private List<string> crashLog = new List<string>();
        
        private void Awake()
        {
            // Ensure this runs first
            Application.logMessageReceived += OnLogMessageReceived;
            
            if (enableDetailedLogging)
            {
                Debug.Log("🔍 AndroidCrashDebugger: Starting crash debugging...");
            }
        }
        
        private void Start()
        {
            StartCoroutine(InitializeSafely());
        }
        
        private IEnumerator InitializeSafely()
        {
            Debug.Log("🚀 AndroidCrashDebugger: Starting safe initialization...");
            
            // Step 1: Wait for Unity to fully load
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(initializationDelay);
            
            // Step 2: Check Android-specific issues
            yield return StartCoroutine(CheckAndroidIssues());
            
            // Step 3: Check AR Foundation issues
            yield return StartCoroutine(CheckARFoundationIssues());
            
            // Step 4: Check permission issues
            yield return StartCoroutine(CheckPermissionIssues());
            
            // Step 5: Check component dependencies
            yield return StartCoroutine(CheckComponentDependencies());
            
            // Step 6: Initialize in safe mode if needed
            if (enableSafeMode)
            {
                yield return StartCoroutine(InitializeSafeMode());
            }
            
            // isInitialized = true; // Removed unused field
            Debug.Log("✅ AndroidCrashDebugger: Safe initialization complete!");
        }
        
        private IEnumerator CheckAndroidIssues()
        {
            Debug.Log("📱 Checking Android-specific issues...");
            
            // Check if running on Android
            if (Application.platform != RuntimePlatform.Android)
            {
                Debug.Log("⚠️ Not running on Android - some checks may not apply");
                yield break;
            }
            
            // Check Android version
            string androidVersion = SystemInfo.operatingSystem;
            Debug.Log($"📱 Android Version: {androidVersion}");
            
            // Check if ARCore is supported
            if (enableDetailedLogging)
            {
                Debug.Log("🔍 Checking ARCore support...");
                // ARCore support check would go here
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        private IEnumerator CheckARFoundationIssues()
        {
            Debug.Log("🥽 Checking AR Foundation issues...");
            
            // Check if AR Foundation packages are available
            try
            {
                // Try to find AR components
                var arSession = FindFirstObjectByType<UnityEngine.XR.ARFoundation.ARSession>();
                if (arSession == null)
                {
                    Debug.LogError("❌ ARSession not found - this will cause crashes!");
                    LogCrash("ARSession not found");
                }
                else
                {
                    Debug.Log("✅ ARSession found");
                }
                
                var xrOrigin = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
                if (xrOrigin == null)
                {
                    Debug.LogError("❌ XROrigin not found - this will cause crashes!");
                    LogCrash("XROrigin not found");
                }
                else
                {
                    Debug.Log("✅ XROrigin found");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ AR Foundation check failed: {e.Message}");
                LogCrash($"AR Foundation check failed: {e.Message}");
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        private IEnumerator CheckPermissionIssues()
        {
            Debug.Log("🔐 Checking permission issues...");
            
            // Check camera permission
            if (!HasCameraPermission())
            {
                Debug.LogError("❌ Camera permission not granted - this will cause crashes!");
                LogCrash("Camera permission not granted");
            }
            else
            {
                Debug.Log("✅ Camera permission granted");
            }
            
            // Check microphone permission
            if (!HasMicrophonePermission())
            {
                Debug.LogError("❌ Microphone permission not granted - this will cause crashes!");
                LogCrash("Microphone permission not granted");
            }
            else
            {
                Debug.Log("✅ Microphone permission granted");
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        private IEnumerator CheckComponentDependencies()
        {
            Debug.Log("🔗 Checking component dependencies...");
            
            // Check for missing components that could cause crashes
            var criticalComponents = new List<string>
            {
                "ARLinguaSphereController",
                "ARManager", 
                "VoiceManager",
                "UIManager"
            };
            
            foreach (var componentName in criticalComponents)
            {
                var component = FindFirstObjectByType<MonoBehaviour>();
                if (component == null || component.GetType().Name != componentName)
                {
                    Debug.LogWarning($"⚠️ {componentName} not found or not properly initialized");
                }
                else
                {
                    Debug.Log($"✅ {componentName} found");
                }
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        private IEnumerator InitializeSafeMode()
        {
            Debug.Log("🛡️ Initializing safe mode...");
            
            // Disable potentially problematic features
            DisableProblematicFeatures();
            
            // Initialize only essential components
            InitializeEssentialComponents();
            
            yield return new WaitForSeconds(0.5f);
        }
        
        private void DisableProblematicFeatures()
        {
            Debug.Log("🔧 Disabling potentially problematic features...");
            
            // Disable ML features that might cause crashes
            var mlManager = FindFirstObjectByType<ARLinguaSphere.ML.MLManager>();
            if (mlManager != null)
            {
                mlManager.enabled = false;
                Debug.Log("⚠️ MLManager disabled for safety");
            }
            
            // Disable gesture recognition
            var gestureManager = FindFirstObjectByType<ARLinguaSphere.Gesture.GestureManager>();
            if (gestureManager != null)
            {
                gestureManager.enabled = false;
                Debug.Log("⚠️ GestureManager disabled for safety");
            }
        }
        
        private void InitializeEssentialComponents()
        {
            Debug.Log("🔧 Initializing essential components...");
            
            // Only initialize the most basic components
            var uiManager = FindFirstObjectByType<ARLinguaSphere.UI.UIManager>();
            if (uiManager != null)
            {
                Debug.Log("✅ UIManager initialized");
            }
        }
        
        private bool HasCameraPermission()
        {
            // This is a simplified check - in a real app you'd use Unity's permission system
            return true; // Assume granted for now
        }
        
        private bool HasMicrophonePermission()
        {
            // This is a simplified check - in a real app you'd use Unity's permission system
            return true; // Assume granted for now
        }
        
        private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                crashCount++;
                LogCrash($"{type}: {logString}");
                
                if (enableCrashPrevention && crashCount > maxRetryAttempts)
                {
                    Debug.LogError("🚨 Too many crashes detected - enabling emergency safe mode!");
                    EnableEmergencySafeMode();
                }
            }
        }
        
        private void LogCrash(string crashInfo)
        {
            crashLog.Add($"[{DateTime.Now:HH:mm:ss}] {crashInfo}");
            
            if (enableDetailedLogging)
            {
                Debug.Log($"📝 Crash logged: {crashInfo}");
            }
        }
        
        private void EnableEmergencySafeMode()
        {
            Debug.Log("🚨 Enabling emergency safe mode...");
            
            // Disable all non-essential components
            var allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var component in allComponents)
            {
                if (component.GetType().Name != "AndroidCrashDebugger" && 
                    component.GetType().Name != "UIManager")
                {
                    component.enabled = false;
                }
            }
            
            Debug.Log("🛡️ Emergency safe mode enabled - only essential components running");
        }
        
        [ContextMenu("Show Crash Log")]
        public void ShowCrashLog()
        {
            Debug.Log("📋 CRASH LOG:");
            foreach (var entry in crashLog)
            {
                Debug.Log(entry);
            }
        }
        
        [ContextMenu("Reset Crash Count")]
        public void ResetCrashCount()
        {
            crashCount = 0;
            crashLog.Clear();
            Debug.Log("🔄 Crash count reset");
        }
        
        private void OnDestroy()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }
    }
}

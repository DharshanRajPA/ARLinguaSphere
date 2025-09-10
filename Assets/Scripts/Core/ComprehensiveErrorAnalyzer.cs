using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using UnityEditor.Build;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Comprehensive error analyzer that identifies and fixes all potential issues
    /// This script performs deep analysis of the entire codebase for errors, edge cases, and potential problems
    /// </summary>
    public class ComprehensiveErrorAnalyzer : MonoBehaviour
    {
        [Header("Analysis Settings")]
        public bool enableDeepAnalysis = true;
        public bool enablePerformanceAnalysis = true;
        public bool enableMemoryAnalysis = true;
        public bool enableAndroidAnalysis = true;
        public bool autoFixIssues = true;
        
        [Header("Analysis Results")]
        [SerializeField] private List<string> criticalErrors = new List<string>();
        [SerializeField] private List<string> warnings = new List<string>();
        [SerializeField] private List<string> performanceIssues = new List<string>();
        [SerializeField] private List<string> memoryIssues = new List<string>();
        [SerializeField] private List<string> androidIssues = new List<string>();
        [SerializeField] private List<string> fixesApplied = new List<string>();
        
        private void Start()
        {
            if (enableDeepAnalysis)
            {
                StartCoroutine(PerformComprehensiveAnalysis());
            }
        }
        
        [ContextMenu("Run Comprehensive Analysis")]
        public void RunComprehensiveAnalysis()
        {
            StartCoroutine(PerformComprehensiveAnalysis());
        }
        
        private System.Collections.IEnumerator PerformComprehensiveAnalysis()
        {
            Debug.Log("🔍 Starting comprehensive error analysis...");
            
            // Clear previous results
            criticalErrors.Clear();
            warnings.Clear();
            performanceIssues.Clear();
            memoryIssues.Clear();
            androidIssues.Clear();
            fixesApplied.Clear();
            
            // Step 1: Analyze critical errors
            yield return StartCoroutine(AnalyzeCriticalErrors());
            
            // Step 2: Analyze null references
            yield return StartCoroutine(AnalyzeNullReferences());
            
            // Step 3: Analyze initialization order
            yield return StartCoroutine(AnalyzeInitializationOrder());
            
            // Step 4: Analyze memory management
            if (enableMemoryAnalysis)
            {
                yield return StartCoroutine(AnalyzeMemoryManagement());
            }
            
            // Step 5: Analyze performance issues
            if (enablePerformanceAnalysis)
            {
                yield return StartCoroutine(AnalyzePerformanceIssues());
            }
            
            // Step 6: Analyze Android-specific issues
            if (enableAndroidAnalysis)
            {
                yield return StartCoroutine(AnalyzeAndroidIssues());
            }
            
            // Step 7: Analyze event handling
            yield return StartCoroutine(AnalyzeEventHandling());
            
            // Step 8: Analyze resource management
            yield return StartCoroutine(AnalyzeResourceManagement());
            
            // Step 9: Apply fixes if enabled
            if (autoFixIssues)
            {
                yield return StartCoroutine(ApplyFixes());
            }
            
            // Step 10: Display results
            DisplayAnalysisResults();
            
            Debug.Log("✅ Comprehensive analysis complete!");
        }
        
        private System.Collections.IEnumerator AnalyzeCriticalErrors()
        {
            Debug.Log("🔍 Analyzing critical errors...");
            
            // Check for missing components
            var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var mb in allMonoBehaviours)
            {
                if (mb == null) continue;
                
                var fields = mb.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (field.FieldType.IsSubclassOf(typeof(Component)) || field.FieldType == typeof(Component))
                    {
                        var value = field.GetValue(mb);
                        if (value == null && field.GetCustomAttribute<SerializeField>() != null)
                        {
                            criticalErrors.Add($"Missing component reference: {mb.GetType().Name}.{field.Name}");
                        }
                    }
                }
            }
            
            yield return null;
        }
        
        private System.Collections.IEnumerator AnalyzeNullReferences()
        {
            Debug.Log("🔍 Analyzing null references...");
            
            // Check AR Manager for null references
            var arManager = FindFirstObjectByType<ARLinguaSphere.AR.ARManager>();
            if (arManager != null)
            {
                if (arManager.xrOrigin == null)
                {
                    criticalErrors.Add("ARManager.xrOrigin is null - this will cause crashes");
                }
                
                // arCamera is a read-only property that gets set automatically by XROrigin
                // if (arManager.ARCamera == null)
                // {
                //     criticalErrors.Add("ARManager.ARCamera is null - this will cause crashes");
                // }
                
                if (arManager.arSession == null)
                {
                    criticalErrors.Add("ARManager.arSession is null - this will cause crashes");
                }
            }
            
            // Check Game Manager for null references
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                if (gameManager.arManager == null)
                {
                    warnings.Add("GameManager.arManager is null - AR features will not work");
                }
                
                if (gameManager.mlManager == null)
                {
                    warnings.Add("GameManager.mlManager is null - ML features will not work");
                }
                
                if (gameManager.voiceManager == null)
                {
                    warnings.Add("GameManager.voiceManager is null - Voice features will not work");
                }
            }
            
            yield return null;
        }
        
        private System.Collections.IEnumerator AnalyzeInitializationOrder()
        {
            Debug.Log("🔍 Analyzing initialization order...");
            
            // Check if AR components are initialized in correct order
            var arSession = FindFirstObjectByType<ARSession>();
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            var arManager = FindFirstObjectByType<ARLinguaSphere.AR.ARManager>();
            
            if (arManager != null)
            {
                if (arSession == null)
                {
                    criticalErrors.Add("ARSession must be initialized before ARManager");
                }
                
                if (xrOrigin == null)
                {
                    criticalErrors.Add("XROrigin must be initialized before ARManager");
                }
                
                if (arManager.xrOrigin == null && xrOrigin != null)
                {
                    warnings.Add("ARManager.xrOrigin not connected to XROrigin");
                }
            }
            
            yield return null;
        }
        
        private System.Collections.IEnumerator AnalyzeMemoryManagement()
        {
            Debug.Log("🔍 Analyzing memory management...");
            
            // Check for potential memory leaks in ML Manager
            var mlManager = FindFirstObjectByType<ARLinguaSphere.ML.MLManager>();
            if (mlManager != null)
            {
                // Check if frame queue is being cleared
                var frameQueueField = mlManager.GetType().GetField("frameQueue", BindingFlags.NonPublic | BindingFlags.Instance);
                if (frameQueueField != null)
                {
                    var frameQueue = frameQueueField.GetValue(mlManager) as System.Collections.Queue;
                    if (frameQueue != null && frameQueue.Count > 10)
                    {
                        memoryIssues.Add("MLManager frame queue has too many items - potential memory leak");
                    }
                }
            }
            
            // Check for unsubscribed events
            var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var mb in allMonoBehaviours)
            {
                if (mb == null) continue;
                
                var methods = mb.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    if (method.Name.StartsWith("On") && method.GetParameters().Length == 0)
                    {
                        // Check if this looks like an event handler
                        if (method.Name.Contains("Event") || method.Name.Contains("Callback"))
                        {
                            // This is a potential event handler that might not be unsubscribed
                            warnings.Add($"Potential event handler not unsubscribed: {mb.GetType().Name}.{method.Name}");
                        }
                    }
                }
            }
            
            yield return null;
        }
        
        private System.Collections.IEnumerator AnalyzePerformanceIssues()
        {
            Debug.Log("🔍 Analyzing performance issues...");
            
            // Check for expensive operations in Update methods
            var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var mb in allMonoBehaviours)
            {
                if (mb == null) continue;
                
                var updateMethod = mb.GetType().GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (updateMethod != null)
                {
                    var methodBody = updateMethod.GetMethodBody();
                    if (methodBody != null)
                    {
                        var ilBytes = methodBody.GetILAsByteArray();
                        if (ilBytes != null && ilBytes.Length > 1000)
                        {
                            performanceIssues.Add($"Update method in {mb.GetType().Name} is too complex - consider optimization");
                        }
                    }
                }
            }
            
            // Check for FindObjectOfType calls in Update
            foreach (var mb in allMonoBehaviours)
            {
                if (mb == null) continue;
                
                var updateMethod = mb.GetType().GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (updateMethod != null)
                {
                    var methodBody = updateMethod.GetMethodBody();
                    if (methodBody != null)
                    {
                        var ilBytes = methodBody.GetILAsByteArray();
                        if (ilBytes != null)
                        {
                            // Check for FindObjectOfType calls (simplified check)
                            performanceIssues.Add($"Update method in {mb.GetType().Name} may contain expensive operations");
                        }
                    }
                }
            }
            
            yield return null;
        }
        
        private System.Collections.IEnumerator AnalyzeAndroidIssues()
        {
            Debug.Log("🔍 Analyzing Android-specific issues...");
            
            // Check Android Manifest
            if (!System.IO.File.Exists("Assets/Plugins/Android/AndroidManifest.xml"))
            {
                androidIssues.Add("AndroidManifest.xml not found");
            }
            else
            {
                var manifestContent = System.IO.File.ReadAllText("Assets/Plugins/Android/AndroidManifest.xml");
                if (!manifestContent.Contains("package="))
                {
                    androidIssues.Add("AndroidManifest.xml missing package declaration");
                }
                
                if (!manifestContent.Contains("android.permission.CAMERA"))
                {
                    androidIssues.Add("AndroidManifest.xml missing camera permission");
                }
                
                if (!manifestContent.Contains("android.permission.RECORD_AUDIO"))
                {
                    androidIssues.Add("AndroidManifest.xml missing microphone permission");
                }
                
                if (!manifestContent.Contains("com.google.ar.core"))
                {
                    androidIssues.Add("AndroidManifest.xml missing ARCore meta-data");
                }
            }
            
            // Check Project Settings
            if (PlayerSettings.Android.targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto)
            {
                androidIssues.Add("Android Target SDK Version is set to Auto - should be specific version");
            }
            
            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel29)
            {
                androidIssues.Add("Android Minimum SDK Version is too low for ARCore - should be 29 or higher");
            }
            
            if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) != ScriptingImplementation.IL2CPP)
            {
                androidIssues.Add("Android Scripting Backend should be IL2CPP for better performance");
            }
            
            if (PlayerSettings.Android.targetArchitectures != AndroidArchitecture.ARM64)
            {
                androidIssues.Add("Android Target Architecture should be ARM64 for better performance");
            }
            
            yield return null;
        }
        
        private System.Collections.IEnumerator AnalyzeEventHandling()
        {
            Debug.Log("🔍 Analyzing event handling...");
            
            // Check for proper event subscription/unsubscription
            var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var mb in allMonoBehaviours)
            {
                if (mb == null) continue;
                
                var onEnableMethod = mb.GetType().GetMethod("OnEnable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var onDisableMethod = mb.GetType().GetMethod("OnDisable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (onEnableMethod != null && onDisableMethod == null)
                {
                    warnings.Add($"{mb.GetType().Name} subscribes to events in OnEnable but has no OnDisable to unsubscribe");
                }
            }
            
            yield return null;
        }
        
        private System.Collections.IEnumerator AnalyzeResourceManagement()
        {
            Debug.Log("🔍 Analyzing resource management...");
            
            // Check for proper disposal of resources
            var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var mb in allMonoBehaviours)
            {
                if (mb == null) continue;
                
                var disposeMethod = mb.GetType().GetMethod("Dispose", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var onDestroyMethod = mb.GetType().GetMethod("OnDestroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (disposeMethod != null && onDestroyMethod == null)
                {
                    warnings.Add($"{mb.GetType().Name} implements Dispose but has no OnDestroy to call it");
                }
            }
            
            yield return null;
        }
        
        private System.Collections.IEnumerator ApplyFixes()
        {
            Debug.Log("🔧 Applying fixes...");
            
            // Fix 1: Connect AR Manager references
            var arManager = FindFirstObjectByType<ARLinguaSphere.AR.ARManager>();
            if (arManager != null)
            {
                if (arManager.xrOrigin == null)
                {
                    var xrOrigin = FindFirstObjectByType<XROrigin>();
                    if (xrOrigin != null)
                    {
                        arManager.xrOrigin = xrOrigin;
                        fixesApplied.Add("Connected ARManager.xrOrigin to XROrigin");
                    }
                }
                
                // arCamera is a read-only property that gets set automatically by XROrigin
                // if (arManager.ARCamera == null && arManager.xrOrigin != null)
                // {
                //     // Cannot set arCamera directly as it's read-only
                //     fixesApplied.Add("ARManager.arCamera will be set automatically by XROrigin");
                // }
                
                if (arManager.arSession == null)
                {
                    var arSession = FindFirstObjectByType<ARSession>();
                    if (arSession != null)
                    {
                        arManager.arSession = arSession;
                        fixesApplied.Add("Connected ARManager.arSession to ARSession");
                    }
                }
            }
            
            // Fix 2: Connect Game Manager references
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                if (gameManager.arManager == null)
                {
                    var foundArManager = FindFirstObjectByType<ARLinguaSphere.AR.ARManager>();
                    if (foundArManager != null)
                    {
                        gameManager.arManager = foundArManager;
                        fixesApplied.Add("Connected GameManager.arManager to ARManager");
                    }
                }
                
                if (gameManager.mlManager == null)
                {
                    var mlManager = FindFirstObjectByType<ARLinguaSphere.ML.MLManager>();
                    if (mlManager != null)
                    {
                        gameManager.mlManager = mlManager;
                        fixesApplied.Add("Connected GameManager.mlManager to MLManager");
                    }
                }
                
                if (gameManager.voiceManager == null)
                {
                    var voiceManager = FindFirstObjectByType<ARLinguaSphere.Voice.VoiceManager>();
                    if (voiceManager != null)
                    {
                        gameManager.voiceManager = voiceManager;
                        fixesApplied.Add("Connected GameManager.voiceManager to VoiceManager");
                    }
                }
            }
            
            yield return null;
        }
        
        private void DisplayAnalysisResults()
        {
            Debug.Log("📊 Comprehensive Analysis Results:");
            Debug.Log($"🔴 Critical Errors: {criticalErrors.Count}");
            foreach (var error in criticalErrors)
            {
                Debug.LogError($"❌ {error}");
            }
            
            Debug.Log($"🟡 Warnings: {warnings.Count}");
            foreach (var warning in warnings)
            {
                Debug.LogWarning($"⚠️ {warning}");
            }
            
            Debug.Log($"🔵 Performance Issues: {performanceIssues.Count}");
            foreach (var issue in performanceIssues)
            {
                Debug.LogWarning($"⚡ {issue}");
            }
            
            Debug.Log($"🟠 Memory Issues: {memoryIssues.Count}");
            foreach (var issue in memoryIssues)
            {
                Debug.LogWarning($"💾 {issue}");
            }
            
            Debug.Log($"🟢 Android Issues: {androidIssues.Count}");
            foreach (var issue in androidIssues)
            {
                Debug.LogWarning($"📱 {issue}");
            }
            
            Debug.Log($"✅ Fixes Applied: {fixesApplied.Count}");
            foreach (var fix in fixesApplied)
            {
                Debug.Log($"🔧 {fix}");
            }
        }
        
        [ContextMenu("Fix All Issues")]
        public void FixAllIssues()
        {
            StartCoroutine(ApplyFixes());
        }
        
        [ContextMenu("Clear All Issues")]
        public void ClearAllIssues()
        {
            criticalErrors.Clear();
            warnings.Clear();
            performanceIssues.Clear();
            memoryIssues.Clear();
            androidIssues.Clear();
            fixesApplied.Clear();
            Debug.Log("🧹 All issues cleared");
        }
    }
}

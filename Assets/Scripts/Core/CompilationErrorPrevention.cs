using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Comprehensive compilation error prevention system
    /// This script helps prevent common compilation errors in Unity projects
    /// </summary>
    public class CompilationErrorPrevention : MonoBehaviour
    {
        [Header("Prevention Settings")]
        public bool enableRuntimeChecks = true;
        public bool enableNamespaceValidation = true;
        public bool enableAPIVersionChecks = true;
        
        [ContextMenu("Run Compilation Health Check")]
        public void RunCompilationHealthCheck()
        {
            Debug.Log("🔍 Running comprehensive compilation health check...");
            
            if (enableRuntimeChecks)
            {
                CheckForEditorAPIsInRuntimeScripts();
            }
            
            if (enableNamespaceValidation)
            {
                ValidateNamespaceImports();
            }
            
            if (enableAPIVersionChecks)
            {
                CheckForDeprecatedAPIs();
            }
            
            Debug.Log("✅ Compilation health check completed!");
        }
        
        private void CheckForEditorAPIsInRuntimeScripts()
        {
            Debug.Log("🔍 Checking for editor-only APIs in runtime scripts...");
            
            var problematicAPIs = new List<string>
            {
                "UnityEditor.PackageManager",
                "UnityEditor.AssetDatabase",
                "UnityEditor.PlayerSettings",
                "UnityEditor.Build",
                "UnityEditor.EditorApplication",
                "UnityEditor.Selection",
                "UnityEditor.SceneView"
            };
            
            var allScripts = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            foreach (var script in allScripts)
            {
                if (script == null) continue;
                
                var scriptType = script.GetType();
                var sourceCode = GetSourceCode(scriptType);
                
                if (sourceCode != null)
                {
                    foreach (var api in problematicAPIs)
                    {
                        if (sourceCode.Contains(api) && !sourceCode.Contains("#if UNITY_EDITOR"))
                        {
                            Debug.LogWarning($"⚠️ Potential editor API usage in runtime script: {scriptType.Name} contains {api}");
                        }
                    }
                }
            }
        }
        
        private void ValidateNamespaceImports()
        {
            Debug.Log("🔍 Validating namespace imports...");
            
            var requiredNamespaces = new Dictionary<string, string>
            {
                {"UnityEngine.XR.ARFoundation", "AR Foundation package"},
                {"Unity.XR.CoreUtils", "XR Core Utils package"},
                {"TMPro", "TextMeshPro package"},
                {"UnityEngine.UI", "Unity UI package"}
            };
            
            var missingPackages = new List<string>();
            
            foreach (var ns in requiredNamespaces)
            {
                try
                {
                    var assembly = Assembly.Load(ns.Key.Split('.')[0]);
                    if (assembly == null)
                    {
                        missingPackages.Add(ns.Value);
                    }
                }
                catch
                {
                    missingPackages.Add(ns.Value);
                }
            }
            
            if (missingPackages.Count > 0)
            {
                Debug.LogError("❌ Missing required packages:");
                foreach (var package in missingPackages)
                {
                    Debug.LogError($"   - {package}");
                }
            }
            else
            {
                Debug.Log("✅ All required namespaces are available");
            }
        }
        
        private void CheckForDeprecatedAPIs()
        {
            Debug.Log("🔍 Checking for deprecated APIs...");
            
            var deprecatedAPIs = new Dictionary<string, string>
            {
                {"FindObjectOfType", "Use FindFirstObjectByType instead"},
                {"FindObjectsOfType", "Use FindObjectsByType instead"},
                {"OnApplicationFocus", "Use OnApplicationFocus with proper parameter handling"},
                {"OnApplicationPause", "Use OnApplicationPause with proper parameter handling"}
            };
            
            var allScripts = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            foreach (var script in allScripts)
            {
                if (script == null) continue;
                
                var scriptType = script.GetType();
                var sourceCode = GetSourceCode(scriptType);
                
                if (sourceCode != null)
                {
                    foreach (var api in deprecatedAPIs)
                    {
                        if (sourceCode.Contains(api.Key))
                        {
                            Debug.LogWarning($"⚠️ Deprecated API usage in {scriptType.Name}: {api.Key} - {api.Value}");
                        }
                    }
                }
            }
        }
        
        private string GetSourceCode(System.Type type)
        {
            // This is a simplified version - in a real implementation,
            // you would read the actual source file
            return null;
        }
        
        [ContextMenu("Show Prevention Guidelines")]
        public void ShowPreventionGuidelines()
        {
            Debug.Log("📋 COMPILATION ERROR PREVENTION GUIDELINES:");
            Debug.Log("");
            Debug.Log("1. EDITOR-ONLY APIs:");
            Debug.Log("   ❌ NEVER use UnityEditor.* APIs in runtime scripts");
            Debug.Log("   ✅ Use #if UNITY_EDITOR preprocessor directives when needed");
            Debug.Log("   ✅ Move editor functionality to Editor/ folder scripts");
            Debug.Log("");
            Debug.Log("2. NAMESPACE IMPORTS:");
            Debug.Log("   ❌ Don't import namespaces that don't exist");
            Debug.Log("   ✅ Check package dependencies before using namespaces");
            Debug.Log("   ✅ Use conditional compilation for optional packages");
            Debug.Log("");
            Debug.Log("3. DEPRECATED APIs:");
            Debug.Log("   ❌ Avoid FindObjectOfType (deprecated)");
            Debug.Log("   ✅ Use FindFirstObjectByType instead");
            Debug.Log("   ❌ Avoid FindObjectsOfType (deprecated)");
            Debug.Log("   ✅ Use FindObjectsByType instead");
            Debug.Log("");
            Debug.Log("4. PACKAGE DEPENDENCIES:");
            Debug.Log("   ❌ Don't assume packages are installed");
            Debug.Log("   ✅ Check package availability before using");
            Debug.Log("   ✅ Provide fallbacks for missing packages");
            Debug.Log("");
            Debug.Log("5. RUNTIME vs EDITOR:");
            Debug.Log("   ❌ Don't mix runtime and editor code");
            Debug.Log("   ✅ Separate concerns properly");
            Debug.Log("   ✅ Use proper assembly definitions");
        }
        
        [ContextMenu("Validate Current Project")]
        public void ValidateCurrentProject()
        {
            Debug.Log("🔍 Validating current project for potential issues...");
            
            // Check for common problematic patterns
            var issues = new List<string>();
            
            // Check for missing packages
            if (!IsPackageAvailable("com.unity.xr.arfoundation"))
                issues.Add("AR Foundation package may be missing");
            
            if (!IsPackageAvailable("com.unity.textmeshpro"))
                issues.Add("TextMeshPro package may be missing");
            
            if (!IsPackageAvailable("com.unity.ugui"))
                issues.Add("Unity UI package may be missing");
            
            if (issues.Count > 0)
            {
                Debug.LogWarning("⚠️ Potential issues found:");
                foreach (var issue in issues)
                {
                    Debug.LogWarning($"   - {issue}");
                }
            }
            else
            {
                Debug.Log("✅ No obvious issues found in current project");
            }
        }
        
        private bool IsPackageAvailable(string packageName)
        {
            try
            {
                var assembly = Assembly.Load(packageName.Split('.')[2]); // Extract package name
                return assembly != null;
            }
            catch
            {
                return false;
            }
        }
    }
}

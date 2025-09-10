using UnityEngine;
using System.IO;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Cleans up XR simulation warnings and temporary files
    /// </summary>
    public class XRSimulationCleaner : MonoBehaviour
    {
        [ContextMenu("Clean XR Simulation Files")]
        public static void CleanXRSimulationFiles()
        {
            Debug.Log("🧹 Cleaning XR simulation files...");
            
            // Clean up XR simulation temporary files
            CleanDirectory("Assets/XR/Temp");
            CleanDirectory("Assets/XR/UserSimulationSettings/Resources");
            CleanDirectory("Assets/XR/Resources");
            
            // Clean up Library cache
            CleanDirectory("Library/Bee");
            CleanDirectory("Library/ScriptAssemblies");
            
            Debug.Log("✅ XR simulation files cleaned!");
        }
        
        private static void CleanDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                    Debug.Log($"✅ Cleaned directory: {path}");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"⚠️ Could not clean directory {path}: {e.Message}");
                }
            }
        }
        
        [ContextMenu("Fix XR Simulation Warnings")]
        public static void FixXRSimulationWarnings()
        {
            Debug.Log("🔧 Fixing XR simulation warnings...");
            
            // Create necessary directories
            CreateDirectoryIfNotExists("Assets/XR/Temp");
            CreateDirectoryIfNotExists("Assets/XR/UserSimulationSettings/Resources");
            CreateDirectoryIfNotExists("Assets/XR/Resources");
            
            // Create placeholder files to prevent warnings
            CreatePlaceholderFile("Assets/XR/Temp/XRSimulationPreferences.asset");
            CreatePlaceholderFile("Assets/XR/UserSimulationSettings/Resources/XRSimulationPreferences.asset");
            CreatePlaceholderFile("Assets/XR/Resources/XRSimulationRuntimeSettings.asset");
            
            Debug.Log("✅ XR simulation warnings fixed!");
        }
        
        private static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"✅ Created directory: {path}");
            }
        }
        
        private static void CreatePlaceholderFile(string path)
        {
            if (!File.Exists(path))
            {
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                File.WriteAllText(path, "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n--- !u!114\nMonoBehaviour:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 0}\n  m_Enabled: 1\n  m_EditorHideFlags: 0\n  m_Script: {fileID: 0}\n  m_Name: XRSimulationPlaceholder\n");
                Debug.Log($"✅ Created placeholder file: {path}");
            }
        }
    }
}

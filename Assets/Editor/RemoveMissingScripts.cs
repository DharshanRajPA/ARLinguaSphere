using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ARLinguaSphere.Editor
{
    /// <summary>
    /// Utility to remove missing script components from GameObjects
    /// </summary>
    public class RemoveMissingScripts : EditorWindow
    {
        [MenuItem("ARLinguaSphere/Tools/Remove Missing Scripts")]
        public static void ShowWindow()
        {
            GetWindow<RemoveMissingScripts>("Remove Missing Scripts");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Remove Missing Scripts", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            GUILayout.Label("This tool will remove all missing script components from GameObjects in the scene.");
            GUILayout.Space(10);
            
            if (GUILayout.Button("Remove Missing Scripts from Scene"))
            {
                RemoveMissingScriptsFromScene();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Remove Missing Scripts from All Prefabs"))
            {
                RemoveMissingScriptsFromPrefabs();
            }
        }
        
        private void RemoveMissingScriptsFromScene()
        {
            int removedCount = 0;
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (GameObject obj in allObjects)
            {
                removedCount += RemoveMissingScriptsFromGameObject(obj);
            }
            
            Debug.Log($"Removed {removedCount} missing script components from scene.");
            EditorUtility.DisplayDialog("Complete", $"Removed {removedCount} missing script components from scene.", "OK");
        }
        
        private void RemoveMissingScriptsFromPrefabs()
        {
            int removedCount = 0;
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            
            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    removedCount += RemoveMissingScriptsFromGameObject(prefab);
                }
            }
            
            AssetDatabase.SaveAssets();
            Debug.Log($"Removed {removedCount} missing script components from prefabs.");
            EditorUtility.DisplayDialog("Complete", $"Removed {removedCount} missing script components from prefabs.", "OK");
        }
        
        private int RemoveMissingScriptsFromGameObject(GameObject obj)
        {
            int removedCount = 0;
            Component[] components = obj.GetComponents<Component>();
            List<Component> componentsToRemove = new List<Component>();
            
            foreach (Component component in components)
            {
                if (component == null)
                {
                    componentsToRemove.Add(component);
                    removedCount++;
                }
            }
            
            foreach (Component component in componentsToRemove)
            {
                if (component != null)
                {
                    DestroyImmediate(component);
                }
            }
            
            return removedCount;
        }
    }
}

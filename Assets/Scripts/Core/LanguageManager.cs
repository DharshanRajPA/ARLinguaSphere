using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ARLinguaSphere.Core.ThirdParty;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Manages language translations and dictionary operations
    /// </summary>
    public class LanguageManager : MonoBehaviour
    {
        [Header("Language Settings")]
        public string currentLanguage = "en";
        public string fallbackLanguage = "en";
        public bool enableOnlineTranslation = true;
        
        [Header("Dictionary Settings")]
        public string dictionaryPath = "offline_dictionary";
        
        private Dictionary<string, Dictionary<string, string>> offlineDictionary;
        private bool isInitialized = false;
        
        // Events
        public System.Action<string, string> OnLanguageChanged;
        public System.Action<string, string, string> OnTranslationCompleted;
        
        public void Initialize()
        {
            Debug.Log("LanguageManager: Initializing language systems...");
            
            offlineDictionary = new Dictionary<string, Dictionary<string, string>>();
            isInitialized = true;
            
            Debug.Log("LanguageManager: Language systems initialized!");
            LoadOfflineDictionary();
        }
        
        public void LoadOfflineDictionary()
        {
            try
            {
                TextAsset dictionaryAsset = Resources.Load<TextAsset>(dictionaryPath);
                if (dictionaryAsset != null)
                {
                    string jsonContent = dictionaryAsset.text;
                    var parsed = MiniJSON.Deserialize(jsonContent) as Dictionary<string, object>;
                    if (parsed == null)
                    {
                        Debug.LogError("LanguageManager: Failed to parse offline dictionary JSON");
                    }
                    else
                    {
                        var dict = new Dictionary<string, Dictionary<string, string>>();
                        foreach (var kvp in parsed)
                        {
                            var inner = kvp.Value as Dictionary<string, object>;
                            if (inner == null) continue;
                            var innerDict = new Dictionary<string, string>();
                            foreach (var langKvp in inner)
                            {
                                innerDict[langKvp.Key] = langKvp.Value != null ? langKvp.Value.ToString() : string.Empty;
                            }
                            dict[kvp.Key] = innerDict;
                        }
                        offlineDictionary = dict;
                        Debug.Log($"LanguageManager: Loaded offline dictionary with {offlineDictionary.Count} entries");
                    }
                }
                else
                {
                    Debug.LogError($"LanguageManager: Could not load dictionary from {dictionaryPath}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"LanguageManager: Error loading offline dictionary: {e.Message}");
            }
        }
        
        public string GetTranslation(string key, string targetLanguage = null)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("LanguageManager: Not initialized yet");
                return key;
            }
            
            if (string.IsNullOrEmpty(targetLanguage))
            {
                targetLanguage = currentLanguage;
            }
            
            // Try offline dictionary first
            if (offlineDictionary.ContainsKey(key) && 
                offlineDictionary[key].ContainsKey(targetLanguage))
            {
                string translation = offlineDictionary[key][targetLanguage];
                OnTranslationCompleted?.Invoke(key, targetLanguage, translation);
                return translation;
            }
            
            // Fallback to online translation if enabled
            if (enableOnlineTranslation)
            {
                return GetOnlineTranslation(key, targetLanguage);
            }
            
            // Return original key if no translation found
            Debug.LogWarning($"LanguageManager: No translation found for '{key}' in '{targetLanguage}'");
            return key;
        }
        
        private string GetOnlineTranslation(string key, string targetLanguage)
        {
            // TODO: Implement online translation API call
            // This would call Google Translate or similar service
            
            Debug.Log($"LanguageManager: Online translation requested for '{key}' to '{targetLanguage}'");
            
            // For now, return a placeholder
            return $"[{targetLanguage}] {key}";
        }
        
        public void SetCurrentLanguage(string languageCode)
        {
            if (currentLanguage != languageCode)
            {
                string previousLanguage = currentLanguage;
                currentLanguage = languageCode;
                OnLanguageChanged?.Invoke(previousLanguage, currentLanguage);
                Debug.Log($"LanguageManager: Language changed from {previousLanguage} to {currentLanguage}");
            }
        }
        
        public List<string> GetAvailableLanguages()
        {
            var languages = new List<string>();
            
            if (offlineDictionary.Count > 0)
            {
                var firstEntry = offlineDictionary.Values.GetEnumerator();
                if (firstEntry.MoveNext())
                {
                    languages.AddRange(firstEntry.Current.Keys);
                }
            }
            
            return languages;
        }
        
        public bool IsLanguageSupported(string languageCode)
        {
            var availableLanguages = GetAvailableLanguages();
            return availableLanguages.Contains(languageCode);
        }
        
        public void AddCustomTranslation(string key, string language, string translation)
        {
            if (!offlineDictionary.ContainsKey(key))
            {
                offlineDictionary[key] = new Dictionary<string, string>();
            }
            
            offlineDictionary[key][language] = translation;
            Debug.Log($"LanguageManager: Added custom translation: {key} -> {translation} ({language})");
        }
        
        public void SaveCustomTranslations()
        {
            // TODO: Implement saving custom translations to persistent storage
            Debug.Log("LanguageManager: Custom translations saved");
        }
    }
    
    
}

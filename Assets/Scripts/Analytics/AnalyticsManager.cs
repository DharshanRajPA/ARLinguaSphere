using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace ARLinguaSphere.Analytics
{
    /// <summary>
    /// Manages learning analytics and adaptive learning algorithms
    /// </summary>
    public class AnalyticsManager : MonoBehaviour
    {
        [Header("Analytics Settings")]
        public bool enableAnalytics = true;
        public bool enableLocalLogging = true;
        public bool enableCloudSync = true;
        public float syncInterval = 30f;
        
        [Header("Adaptive Learning Settings")]
        public float difficultyAdjustmentRate = 0.1f;
        public int minSamplesForAdaptation = 5;
        public float errorRateThreshold = 0.3f;
        
        private bool isInitialized = false;
        private List<InteractionData> localInteractions;
        private Dictionary<string, WordStats> wordStatistics;
        private string userId;
        
        // Events
        public event Action<InteractionData> OnInteractionLogged;
        public event Action<string, float> OnDifficultyAdjusted;
        
        public void Initialize()
        {
            Debug.Log("AnalyticsManager: Initializing analytics systems...");
            
            localInteractions = new List<InteractionData>();
            wordStatistics = new Dictionary<string, WordStats>();
            userId = SystemInfo.deviceUniqueIdentifier;
            
            // Load existing data
            LoadLocalData();
            
            // Initialize Firebase Analytics
            InitializeFirebaseAnalytics();
            
            isInitialized = true;
            Debug.Log("AnalyticsManager: Analytics systems initialized!");
        }
        
        private void InitializeFirebaseAnalytics()
        {
            // TODO: Initialize Firebase Analytics
            Debug.Log("AnalyticsManager: Firebase Analytics initialized (placeholder)");
        }
        
        private void LoadLocalData()
        {
            if (!enableLocalLogging)
            {
                return;
            }
            try
            {
                // Lightweight JSON persistence using PlayerPrefs for demo
                string json = PlayerPrefs.GetString("ALS_Analytics_Local", string.Empty);
                if (!string.IsNullOrEmpty(json))
                {
                    var parsed = ARLinguaSphere.Core.ThirdParty.MiniJSON.Deserialize(json) as Dictionary<string, object>;
                    if (parsed != null && parsed.ContainsKey("wordStats"))
                    {
                        var statsMap = parsed["wordStats"] as Dictionary<string, object>;
                        foreach (var kv in statsMap)
                        {
                            var m = kv.Value as Dictionary<string, object>;
                            var ws = new WordStats
                            {
                                wordKey = kv.Key,
                                totalInteractions = m.ContainsKey("totalInteractions") ? System.Convert.ToInt32(m["totalInteractions"]) : 0,
                                successfulInteractions = m.ContainsKey("successfulInteractions") ? System.Convert.ToInt32(m["successfulInteractions"]) : 0,
                                averageResponseTime = m.ContainsKey("averageResponseTime") ? System.Convert.ToSingle(m["averageResponseTime"]) : 0f,
                                difficultyLevel = m.ContainsKey("difficultyLevel") ? System.Convert.ToSingle(m["difficultyLevel"]) : 1f,
                                lastSeen = m.ContainsKey("lastSeen") ? System.Convert.ToInt64(m["lastSeen"]) : 0
                            };
                            wordStatistics[ws.wordKey] = ws;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"AnalyticsManager: Failed to load local data: {e.Message}");
            }
            Debug.Log("AnalyticsManager: Local data loaded");
        }
        
        public void LogInteraction(string anchorId, string labelKey, InteractionType action, bool success, float duration = 0f)
        {
            if (!isInitialized || !enableAnalytics)
            {
                return;
            }
            
            var interaction = new InteractionData
            {
                id = System.Guid.NewGuid().ToString(),
                userId = userId,
                anchorId = anchorId,
                labelKey = labelKey,
                action = action,
                success = success,
                duration = duration,
                timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            
            localInteractions.Add(interaction);
            OnInteractionLogged?.Invoke(interaction);
            
            // Update word statistics
            UpdateWordStatistics(interaction);
            
            // Check for adaptive learning opportunities
            CheckAdaptiveLearning(interaction);
            
            Debug.Log($"AnalyticsManager: Logged interaction - {action} for {labelKey} (success: {success})");
        }
        
        private void UpdateWordStatistics(InteractionData interaction)
        {
            if (!wordStatistics.ContainsKey(interaction.labelKey))
            {
                wordStatistics[interaction.labelKey] = new WordStats
                {
                    wordKey = interaction.labelKey,
                    totalInteractions = 0,
                    successfulInteractions = 0,
                    averageResponseTime = 0f,
                    difficultyLevel = 1f,
                    lastSeen = interaction.timestamp
                };
            }
            
            var stats = wordStatistics[interaction.labelKey];
            stats.totalInteractions++;
            stats.lastSeen = interaction.timestamp;
            
            if (interaction.success)
            {
                stats.successfulInteractions++;
            }
            
            // Update average response time
            if (interaction.duration > 0)
            {
                stats.averageResponseTime = (stats.averageResponseTime * (stats.totalInteractions - 1) + interaction.duration) / stats.totalInteractions;
            }
        }
        
        private void CheckAdaptiveLearning(InteractionData interaction)
        {
            if (!wordStatistics.ContainsKey(interaction.labelKey))
            {
                return;
            }
            
            var stats = wordStatistics[interaction.labelKey];
            
            // Only adapt if we have enough samples
            if (stats.totalInteractions < minSamplesForAdaptation)
            {
                return;
            }
            
            float errorRate = 1f - (float)stats.successfulInteractions / stats.totalInteractions;
            
            if (errorRate > errorRateThreshold)
            {
                // Increase difficulty for this word
                stats.difficultyLevel = Mathf.Min(3f, stats.difficultyLevel + difficultyAdjustmentRate);
                OnDifficultyAdjusted?.Invoke(interaction.labelKey, stats.difficultyLevel);
                Debug.Log($"AnalyticsManager: Increased difficulty for '{interaction.labelKey}' to {stats.difficultyLevel:F2}");
            }
            else if (errorRate < errorRateThreshold * 0.5f)
            {
                // Decrease difficulty for this word
                stats.difficultyLevel = Mathf.Max(0.5f, stats.difficultyLevel - difficultyAdjustmentRate);
                OnDifficultyAdjusted?.Invoke(interaction.labelKey, stats.difficultyLevel);
                Debug.Log($"AnalyticsManager: Decreased difficulty for '{interaction.labelKey}' to {stats.difficultyLevel:F2}");
            }
        }
        
        public List<string> GetWordsForQuiz(int count = 5)
        {
            var quizWords = new List<string>();
            
            // Sort words by difficulty and recent performance
            var sortedWords = new List<KeyValuePair<string, WordStats>>(wordStatistics);
            sortedWords.Sort((a, b) => 
            {
                // Prioritize words with higher difficulty and lower success rate
                float scoreA = a.Value.difficultyLevel * (1f - (float)a.Value.successfulInteractions / a.Value.totalInteractions);
                float scoreB = b.Value.difficultyLevel * (1f - (float)b.Value.successfulInteractions / b.Value.totalInteractions);
                return scoreB.CompareTo(scoreA);
            });
            
            // Select top words for quiz
            for (int i = 0; i < Mathf.Min(count, sortedWords.Count); i++)
            {
                quizWords.Add(sortedWords[i].Key);
            }
            
            return quizWords;
        }
        
        public WordStats GetWordStats(string wordKey)
        {
            return wordStatistics.ContainsKey(wordKey) ? wordStatistics[wordKey] : null;
        }
        
        public void SaveSessionData()
        {
            if (!enableLocalLogging)
            {
                return;
            }
            try
            {
                // Persist word stats only (compact)
                var root = new System.Text.StringBuilder();
                root.Append("{\"wordStats\":{");
                bool first = true;
                foreach (var kv in wordStatistics)
                {
                    if (!first) root.Append(",");
                    first = false;
                    var ws = kv.Value;
                    root.Append($"\"{ws.wordKey}\":{{\"totalInteractions\":{ws.totalInteractions},\"successfulInteractions\":{ws.successfulInteractions},\"averageResponseTime\":{ws.averageResponseTime},\"difficultyLevel\":{ws.difficultyLevel},\"lastSeen\":{ws.lastSeen}}}");
                }
                root.Append("}}");
                PlayerPrefs.SetString("ALS_Analytics_Local", root.ToString());
                PlayerPrefs.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"AnalyticsManager: Failed to save local data: {e.Message}");
            }
            Debug.Log("AnalyticsManager: Session data saved");
        }
        
        public void SyncToCloud()
        {
            if (!enableCloudSync || !isInitialized)
            {
                return;
            }
            
            // TODO: Sync data to Firebase Analytics
            Debug.Log("AnalyticsManager: Syncing data to cloud...");
        }
        
        public void ExportUserData()
        {
            // TODO: Export user data for GDPR compliance
            Debug.Log("AnalyticsManager: User data exported");
        }
        
        public void DeleteUserData()
        {
            // TODO: Delete user data for GDPR compliance
            localInteractions.Clear();
            wordStatistics.Clear();
            Debug.Log("AnalyticsManager: User data deleted");
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveSessionData();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveSessionData();
            }
        }
    }
    
    /// <summary>
    /// Types of user interactions
    /// </summary>
    public enum InteractionType
    {
        ObjectDetected,
        LabelPlaced,
        LabelRemoved,
        TranslationRequested,
        VoiceCommand,
        GesturePerformed,
        QuizAnswered,
        LanguageChanged
    }
    
    /// <summary>
    /// Data structure for interaction logging
    /// </summary>
    [System.Serializable]
    public class InteractionData
    {
        public string id;
        public string userId;
        public string anchorId;
        public string labelKey;
        public InteractionType action;
        public bool success;
        public float duration;
        public long timestamp;
    }
    
    /// <summary>
    /// Statistics for a specific word
    /// </summary>
    [System.Serializable]
    public class WordStats
    {
        public string wordKey;
        public int totalInteractions;
        public int successfulInteractions;
        public float averageResponseTime;
        public float difficultyLevel;
        public long lastSeen;
    }
}

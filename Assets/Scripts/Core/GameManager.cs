using UnityEngine;
using UnityEngine.SceneManagement;
using ARLinguaSphere.AR;
using ARLinguaSphere.ML;
using ARLinguaSphere.Gesture;
using ARLinguaSphere.Voice;
using ARLinguaSphere.Network;
using ARLinguaSphere.Analytics;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Main game manager that coordinates all systems
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        public bool debugMode = false;
        public string defaultLanguage = "en";
        
        [Header("System References")]
        public ARManager arManager;
        public MLManager mlManager;
        public GestureManager gestureManager;
        public VoiceManager voiceManager;
        public LanguageManager languageManager;
        public NetworkManager networkManager;
        public AnalyticsManager analyticsManager;
        
        public static GameManager Instance { get; private set; }
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeGame()
        {
            Debug.Log("ARLinguaSphere: Initializing game systems...");
            
            // Initialize all managers
            InitializeManagers();
            
            // Load offline dictionary
            LoadOfflineDictionary();
            
            Debug.Log("ARLinguaSphere: Game initialization complete!");
        }
        
        private void InitializeManagers()
        {
            // Initialize each manager if not already initialized
            arManager?.Initialize();
            mlManager?.Initialize();
            gestureManager?.Initialize();
            voiceManager?.Initialize();
            languageManager?.Initialize();
            networkManager?.Initialize();
            analyticsManager?.Initialize();
        }
        
        private void LoadOfflineDictionary()
        {
            languageManager?.LoadOfflineDictionary();
        }
        
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // Save game state when app is paused
                SaveGameState();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                // Save game state when app loses focus
                SaveGameState();
            }
        }
        
        private void SaveGameState()
        {
            // Save current game state
            analyticsManager?.SaveSessionData();
        }
    }
}

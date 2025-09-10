using UnityEngine;
using UnityEngine.XR.ARFoundation;
using ARLinguaSphere.AR;
using ARLinguaSphere.ML;
using ARLinguaSphere.Core;
using ARLinguaSphere.Gesture;
using ARLinguaSphere.Voice;
using ARLinguaSphere.UI;
using ARLinguaSphere.Network;
using ARLinguaSphere.Analytics;
using System.Collections;

namespace ARLinguaSphere.Core
{
    /// <summary>
    /// Main controller that integrates all ARLinguaSphere systems
    /// </summary>
    public class ARLinguaSphereController : MonoBehaviour
    {
        [Header("System References")]
        public ARManager arManager;
        public MLManager mlManager;
        public LanguageManager languageManager;
        public GestureManager gestureManager;
        public VoiceManager voiceManager;
        public UIManager uiManager;
        public NetworkManager networkManager;
        public AnalyticsManager analyticsManager;
        public ARLinguaSphere.Analytics.QuizEngine quizEngine;
        public ARLabelManager labelManager;
        
        [Header("AR Camera")]
        public Camera arCamera;
        
        [Header("Settings")]
        public bool enableAutoDetection = true;
        public bool enableVoiceCommands = true;
        public bool enableGestureControls = true;
        public bool enableMultiplayer = true;
        public bool enableAnalytics = true;
        
        private bool isInitialized = false;
        private bool isARSessionActive = false;
        
        // Events
        public System.Action OnSystemInitialized;
        public System.Action OnARSessionStarted;
        public System.Action OnARSessionStopped;
        
        private void Awake()
        {
            // Ensure this is a singleton
            if (FindObjectsOfType<ARLinguaSphereController>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            InitializeSystems();
        }
        
        private void InitializeSystems()
        {
            Debug.Log("ARLinguaSphereController: Initializing all systems...");
            
            // Initialize core systems
            InitializeCoreSystems();
            
            // Initialize AR systems
            InitializeARSystems();
            
            // Initialize ML systems
            InitializeMLSystems();
            
            // Initialize UI systems
            InitializeUISystems();
            
            // Initialize input systems
            InitializeInputSystems();
            
            // Initialize network systems
            InitializeNetworkSystems();
            
            // Initialize analytics
            InitializeAnalytics();
            
            // Initialize quiz engine
            InitializeQuizEngine();
            
            // Connect all systems
            ConnectSystems();
            
            isInitialized = true;
            OnSystemInitialized?.Invoke();
            
            Debug.Log("ARLinguaSphereController: All systems initialized successfully!");
        }
        
        private void InitializeCoreSystems()
        {
            // Initialize Language Manager
            if (languageManager == null)
            {
                languageManager = FindObjectOfType<LanguageManager>();
                if (languageManager == null)
                {
                    var langObj = new GameObject("LanguageManager");
                    languageManager = langObj.AddComponent<LanguageManager>();
                }
            }
            languageManager.Initialize();
            
            // Initialize Analytics Manager
            if (analyticsManager == null)
            {
                analyticsManager = FindObjectOfType<AnalyticsManager>();
                if (analyticsManager == null)
                {
                    var analyticsObj = new GameObject("AnalyticsManager");
                    analyticsManager = analyticsObj.AddComponent<AnalyticsManager>();
                }
            }
            analyticsManager.Initialize();
        }
        
        private void InitializeARSystems()
        {
            // Initialize AR Manager
            if (arManager == null)
            {
                arManager = FindObjectOfType<ARManager>();
                if (arManager == null)
                {
                    var arObj = new GameObject("ARManager");
                    arManager = arObj.AddComponent<ARManager>();
                }
            }
            arManager.Initialize();
            
            // Initialize AR Label Manager
            if (labelManager == null)
            {
                labelManager = FindObjectOfType<ARLabelManager>();
                if (labelManager == null)
                {
                    var labelObj = new GameObject("ARLabelManager");
                    labelManager = labelObj.AddComponent<ARLabelManager>();
                }
            }
            labelManager.Initialize(arManager, mlManager, languageManager, networkManager);
            
            // Get AR Camera reference
            if (arCamera == null)
            {
                arCamera = arManager.ARCamera;
            }
        }
        
        private void InitializeMLSystems()
        {
            // Initialize ML Manager
            if (mlManager == null)
            {
                mlManager = FindObjectOfType<MLManager>();
                if (mlManager == null)
                {
                    var mlObj = new GameObject("MLManager");
                    mlManager = mlObj.AddComponent<MLManager>();
                }
            }
            mlManager.Initialize();
        }
        
        private void InitializeUISystems()
        {
            // Initialize UI Manager
            if (uiManager == null)
            {
                uiManager = FindObjectOfType<UIManager>();
                if (uiManager == null)
                {
                    var uiObj = new GameObject("UIManager");
                    uiManager = uiObj.AddComponent<UIManager>();
                }
            }
            uiManager.Initialize();
        }
        
        private void InitializeInputSystems()
        {
            // Initialize Gesture Manager
            if (gestureManager == null && enableGestureControls)
            {
                gestureManager = FindObjectOfType<GestureManager>();
                if (gestureManager == null)
                {
                    var gestureObj = new GameObject("GestureManager");
                    gestureManager = gestureObj.AddComponent<GestureManager>();
                }
            }
            if (gestureManager != null)
            {
                gestureManager.Initialize();
            }
            
            // Initialize Voice Manager
            if (voiceManager == null && enableVoiceCommands)
            {
                voiceManager = FindObjectOfType<VoiceManager>();
                if (voiceManager == null)
                {
                    var voiceObj = new GameObject("VoiceManager");
                    voiceManager = voiceObj.AddComponent<VoiceManager>();
                }
            }
            if (voiceManager != null)
            {
                voiceManager.Initialize();
            }
        }
        
        private void InitializeNetworkSystems()
        {
            // Initialize Network Manager
            if (networkManager == null && enableMultiplayer)
            {
                networkManager = FindObjectOfType<NetworkManager>();
                if (networkManager == null)
                {
                    var networkObj = new GameObject("NetworkManager");
                    networkManager = networkObj.AddComponent<NetworkManager>();
                }
            }
            if (networkManager != null)
            {
                networkManager.Initialize();
            }
        }
        
        private void InitializeAnalytics()
        {
            if (enableAnalytics && analyticsManager != null)
            {
                // Analytics is already initialized in InitializeCoreSystems
                Debug.Log("ARLinguaSphereController: Analytics enabled");
            }
        }
        
        private void InitializeQuizEngine()
        {
            if (quizEngine == null)
            {
                quizEngine = FindObjectOfType<ARLinguaSphere.Analytics.QuizEngine>();
                if (quizEngine == null)
                {
                    var quizObj = new GameObject("QuizEngine");
                    quizEngine = quizObj.AddComponent<ARLinguaSphere.Analytics.QuizEngine>();
                }
            }
            quizEngine.Initialize(analyticsManager);
        }
        
        private void ConnectSystems()
        {
            // Connect AR and ML systems
            if (arManager != null && mlManager != null)
            {
                // Subscribe to camera frame events for ML processing
                arManager.arCameraManager.frameReceived += OnCameraFrameReceived;
            }
            
            // Connect gesture controls
            if (gestureManager != null)
            {
                gestureManager.OnGestureDetected += OnGestureDetected;
            }
            
            // Connect voice controls
            if (voiceManager != null)
            {
                voiceManager.OnSpeechRecognized += OnSpeechRecognized;
            }
            
            // Connect UI events
            if (uiManager != null)
            {
                uiManager.OnLanguageButtonClicked += OnLanguageButtonClicked;
                uiManager.OnQuizButtonClicked += OnQuizButtonClicked;
                uiManager.OnSettingsButtonClicked += OnSettingsButtonClicked;
                uiManager.OnClearAnchorsButtonClicked += OnClearAnchorsButtonClicked;
            }
            
            // Connect label events
            if (labelManager != null)
            {
                labelManager.OnLabelClicked += OnLabelClicked;
                labelManager.OnLabelPlaced += OnLabelPlaced;
                labelManager.OnLabelRemoved += OnLabelRemoved;
            }
        }
        
        private void OnCameraFrameReceived(UnityEngine.XR.ARFoundation.ARCameraFrameEventArgs args)
        {
            if (!enableAutoDetection || !isARSessionActive) return;
            
            // Get camera texture for ML processing
            if (arCamera != null && mlManager != null)
            {
                // Convert camera frame to texture for ML processing
                var cameraTexture = GetCameraTexture();
                if (cameraTexture != null)
                {
                    mlManager.ProcessFrame(cameraTexture);
                }
            }
        }
        
        private Texture2D GetCameraTexture()
        {
            // This would get the actual camera texture from AR Foundation
            // For now, we'll return null as this requires more complex setup
            return null;
        }
        
        private void OnGestureDetected(GestureType gestureType, Vector2 position)
        {
            Debug.Log($"ARLinguaSphereController: Gesture detected: {gestureType} at {position}");
            
            switch (gestureType)
            {
                case GestureType.Tap:
                    OnScreenTap(position);
                    break;
                case GestureType.ThumbsUp:
                    OnThumbsUpGesture();
                    break;
                case GestureType.OpenPalm:
                    OnOpenPalmGesture();
                    break;
                case GestureType.PinchIn:
                    OnPinchInGesture(position);
                    break;
                case GestureType.PinchOut:
                    OnPinchOutGesture(position);
                    break;
            }
        }
        
        private void OnSpeechRecognized(string speechText)
        {
            Debug.Log($"ARLinguaSphereController: Speech recognized: {speechText}");
            
            if (voiceManager != null)
            {
                voiceManager.ProcessVoiceCommand(speechText);
            }
        }
        
        private void OnScreenTap(Vector2 position)
        {
            // Try to place a label at the tapped position
            if (arManager != null && labelManager != null)
            {
                Vector3 worldPosition = arManager.ScreenToWorldPoint(position, 2f);
                labelManager.PlaceLabelAtPosition("test_object", worldPosition);
            }
        }
        
        private void OnThumbsUpGesture()
        {
            // Cycle through languages
            if (languageManager != null)
            {
                var availableLanguages = languageManager.GetAvailableLanguages();
                if (availableLanguages.Count > 1)
                {
                    int currentIndex = availableLanguages.IndexOf(languageManager.currentLanguage);
                    int nextIndex = (currentIndex + 1) % availableLanguages.Count;
                    languageManager.SetCurrentLanguage(availableLanguages[nextIndex]);
                }
            }
        }
        
        private void OnOpenPalmGesture()
        {
            // Toggle quiz mode
            if (uiManager != null)
            {
                uiManager.ShowQuizPanel();
            }
        }
        
        private void OnPinchInGesture(Vector2 position)
        {
            // Remove label at position
            if (labelManager != null)
            {
                // Find label near position and remove it
                var labels = labelManager.GetActiveLabels();
                foreach (var label in labels)
                {
                    if (label != null)
                    {
                        Vector3 screenPos = arCamera.WorldToScreenPoint(label.transform.position);
                        if (Vector2.Distance(position, screenPos) < 100f)
                        {
                            labelManager.RemoveLabel(label);
                            break;
                        }
                    }
                }
            }
        }
        
        private void OnPinchOutGesture(Vector2 position)
        {
            // Place new label at position
            OnScreenTap(position);
        }
        
        private void OnLanguageButtonClicked()
        {
            OnThumbsUpGesture();
        }
        
        private void OnQuizButtonClicked()
        {
            OnOpenPalmGesture();
        }
        
        private void OnSettingsButtonClicked()
        {
            if (uiManager != null)
            {
                uiManager.ShowSettingsPanel();
            }
        }
        
        private void OnClearAnchorsButtonClicked()
        {
            if (labelManager != null)
            {
                labelManager.RemoveAllLabels();
            }
        }
        
        private void OnLabelClicked(ARLabel label)
        {
            Debug.Log($"ARLinguaSphereController: Label clicked: {label.GetLabelText()}");
            
            // Speak the label text
            if (voiceManager != null)
            {
                voiceManager.Speak(label.GetLabelText());
            }
            
            // Log analytics
            if (analyticsManager != null)
            {
                analyticsManager.LogInteraction(
                    label.GetOriginalText(),
                    label.GetOriginalText(),
                    Analytics.InteractionType.LabelPlaced,
                    true
                );
            }
        }
        
        private void OnLabelPlaced(ARLabel label)
        {
            Debug.Log($"ARLinguaSphereController: Label placed: {label.GetLabelText()}");
            
            // Log analytics
            if (analyticsManager != null)
            {
                analyticsManager.LogInteraction(
                    label.GetOriginalText(),
                    label.GetOriginalText(),
                    Analytics.InteractionType.LabelPlaced,
                    true
                );
            }
        }
        
        private void OnLabelRemoved(ARLabel label)
        {
            Debug.Log($"ARLinguaSphereController: Label removed: {label.GetLabelText()}");
            
            // Log analytics
            if (analyticsManager != null)
            {
                analyticsManager.LogInteraction(
                    label.GetOriginalText(),
                    label.GetOriginalText(),
                    Analytics.InteractionType.LabelRemoved,
                    true
                );
            }
        }
        
        public void StartARSession()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("ARLinguaSphereController: Systems not initialized yet");
                return;
            }
            
            if (arManager != null)
            {
                // Start AR session
                isARSessionActive = true;
                OnARSessionStarted?.Invoke();
                Debug.Log("ARLinguaSphereController: AR session started");
                
                // Connect and auto-join a room for demo
                if (enableMultiplayer && networkManager != null)
                {
                    networkManager.Connect();
                    StartCoroutine(JoinRoomAfterConnect());
                }
            }
        }
        
        private System.Collections.IEnumerator JoinRoomAfterConnect()
        {
            float start = Time.time;
            while (networkManager != null && !networkManager.IsConnected && Time.time - start < 5f)
            {
                yield return null;
            }
            if (networkManager != null && networkManager.IsConnected)
            {
                networkManager.CreateRoom();
            }
        }
        
        public void StopARSession()
        {
            isARSessionActive = false;
            OnARSessionStopped?.Invoke();
            Debug.Log("ARLinguaSphereController: AR session stopped");
        }
        
        public void SetLanguage(string languageCode)
        {
            if (languageManager != null)
            {
                languageManager.SetCurrentLanguage(languageCode);
            }
        }
        
        public void SetAutoDetection(bool enabled)
        {
            enableAutoDetection = enabled;
        }
        
        public void SetVoiceCommands(bool enabled)
        {
            enableVoiceCommands = enabled;
            if (voiceManager != null)
            {
                voiceManager.enableSTT = enabled;
            }
        }
        
        public void SetGestureControls(bool enabled)
        {
            enableGestureControls = enabled;
            if (gestureManager != null)
            {
                gestureManager.enableTouchGestures = enabled;
                gestureManager.enableHandGestures = enabled;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (arManager != null && arManager.arCameraManager != null)
            {
                arManager.arCameraManager.frameReceived -= OnCameraFrameReceived;
            }
            
            if (gestureManager != null)
            {
                gestureManager.OnGestureDetected -= OnGestureDetected;
            }
            
            if (voiceManager != null)
            {
                voiceManager.OnSpeechRecognized -= OnSpeechRecognized;
            }
        }
    }
}

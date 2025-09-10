using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ARLinguaSphere.UI
{
    /// <summary>
    /// Manages UI elements and user interface interactions
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject mainMenuPanel;
        public GameObject arPanel;
        public GameObject settingsPanel;
        public GameObject quizPanel;
        public GameObject loadingPanel;
        
        [Header("AR UI Elements")]
        public TextMeshProUGUI statusText;
        public TextMeshProUGUI languageText;
        public Button languageButton;
        public Button quizButton;
        public Button settingsButton;
        public Button clearAnchorsButton;
        
        [Header("Quiz UI Elements")]
        public TextMeshProUGUI quizQuestionText;
        public Button[] quizAnswerButtons;
        public TextMeshProUGUI quizScoreText;
        public Button quizCloseButton;
        
        [Header("Settings UI Elements")]
        public Slider confidenceSlider;
        public Slider speechRateSlider;
        public Slider speechPitchSlider;
        public Toggle gestureToggle;
        public Toggle voiceToggle;
        public Dropdown languageDropdown;
        
        [Header("Loading UI Elements")]
        public TextMeshProUGUI loadingText;
        public Slider loadingProgressBar;
        
        private bool isInitialized = false;
        private int currentQuizScore = 0;
        private int totalQuizQuestions = 0;
        
        // Events
        public event Action OnLanguageButtonClicked;
        public event Action OnQuizButtonClicked;
        public event Action OnSettingsButtonClicked;
        public event Action OnClearAnchorsButtonClicked;
        public event Action OnQuizAnswerSelected;
        public event Action OnSettingsChanged;
        
        public void Initialize()
        {
            Debug.Log("UIManager: Initializing UI systems...");
            
            // Initialize UI elements
            InitializeUIElements();
            
            // Subscribe to events
            SubscribeToEvents();
            
            // Set initial state
            SetInitialUIState();
            
            isInitialized = true;
            Debug.Log("UIManager: UI systems initialized!");
        }
        
        private void InitializeUIElements()
        {
            // Initialize buttons
            if (languageButton != null)
                languageButton.onClick.AddListener(() => OnLanguageButtonClicked?.Invoke());
            
            if (quizButton != null)
                quizButton.onClick.AddListener(() => OnQuizButtonClicked?.Invoke());
            
            if (settingsButton != null)
                settingsButton.onClick.AddListener(() => OnSettingsButtonClicked?.Invoke());
            
            if (clearAnchorsButton != null)
                clearAnchorsButton.onClick.AddListener(() => OnClearAnchorsButtonClicked?.Invoke());
            
            if (quizCloseButton != null)
                quizCloseButton.onClick.AddListener(() => CloseQuizPanel());
            
            // Initialize quiz answer buttons
            for (int i = 0; i < quizAnswerButtons.Length; i++)
            {
                int buttonIndex = i; // Capture for closure
                quizAnswerButtons[i].onClick.AddListener(() => OnQuizAnswerSelected?.Invoke());
            }
            
            // Initialize settings sliders
            if (confidenceSlider != null)
                confidenceSlider.onValueChanged.AddListener(OnConfidenceSliderChanged);
            
            if (speechRateSlider != null)
                speechRateSlider.onValueChanged.AddListener(OnSpeechRateSliderChanged);
            
            if (speechPitchSlider != null)
                speechPitchSlider.onValueChanged.AddListener(OnSpeechPitchSliderChanged);
            
            // Initialize toggles
            if (gestureToggle != null)
                gestureToggle.onValueChanged.AddListener(OnGestureToggleChanged);
            
            if (voiceToggle != null)
                voiceToggle.onValueChanged.AddListener(OnVoiceToggleChanged);
            
            // Initialize language dropdown
            if (languageDropdown != null)
                languageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);
        }
        
        private void SubscribeToEvents()
        {
            // Subscribe to game events
            var gameManager = FindFirstObjectByType<Core.GameManager>();
            if (gameManager != null)
            {
                // Subscribe to language changes
                if (gameManager.languageManager != null)
                {
                    gameManager.languageManager.OnLanguageChanged += OnLanguageChanged;
                }
            }
        }
        
        private void SetInitialUIState()
        {
            // Show main menu initially
            ShowMainMenu();
            
            // Set initial values
            if (statusText != null)
                statusText.text = "Ready";
            
            if (languageText != null)
                languageText.text = "English";
            
            // Set initial slider values
            if (confidenceSlider != null)
                confidenceSlider.value = 0.5f;
            
            if (speechRateSlider != null)
                speechRateSlider.value = 1f;
            
            if (speechPitchSlider != null)
                speechPitchSlider.value = 1f;
        }
        
        public void ShowMainMenu()
        {
            SetPanelActive(mainMenuPanel, true);
            SetPanelActive(arPanel, false);
            SetPanelActive(settingsPanel, false);
            SetPanelActive(quizPanel, false);
            SetPanelActive(loadingPanel, false);
        }
        
        public void ShowARPanel()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(arPanel, true);
            SetPanelActive(settingsPanel, false);
            SetPanelActive(quizPanel, false);
            SetPanelActive(loadingPanel, false);
        }
        
        public void ShowSettingsPanel()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(arPanel, false);
            SetPanelActive(settingsPanel, true);
            SetPanelActive(quizPanel, false);
            SetPanelActive(loadingPanel, false);
        }
        
        public void ShowQuizPanel()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(arPanel, false);
            SetPanelActive(settingsPanel, false);
            SetPanelActive(quizPanel, true);
            SetPanelActive(loadingPanel, false);
            
            // Reset quiz state
            currentQuizScore = 0;
            totalQuizQuestions = 0;
            UpdateQuizScore();
        }
        
        public void ShowLoadingPanel(string message = "Loading...")
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(arPanel, false);
            SetPanelActive(settingsPanel, false);
            SetPanelActive(quizPanel, false);
            SetPanelActive(loadingPanel, true);
            
            if (loadingText != null)
                loadingText.text = message;
        }
        
        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
                panel.SetActive(active);
        }
        
        public void UpdateStatusText(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }
        
        public void UpdateLanguageText(string language)
        {
            if (languageText != null)
                languageText.text = language;
        }
        
        private void OnLanguageChanged(string previousLanguage, string newLanguage)
        {
            UpdateLanguageText(newLanguage);
        }
        
        public void ShowQuizQuestion(string question, string[] answers)
        {
            if (quizQuestionText != null)
                quizQuestionText.text = question;
            
            for (int i = 0; i < quizAnswerButtons.Length && i < answers.Length; i++)
            {
                if (quizAnswerButtons[i] != null)
                {
                    var buttonText = quizAnswerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                        buttonText.text = answers[i];
                }
            }
        }
        
        public void UpdateQuizScore()
        {
            if (quizScoreText != null)
                quizScoreText.text = $"Score: {currentQuizScore}/{totalQuizQuestions}";
        }
        
        public void OnQuizAnswerCorrect()
        {
            currentQuizScore++;
            totalQuizQuestions++;
            UpdateQuizScore();
        }
        
        public void OnQuizAnswerIncorrect()
        {
            totalQuizQuestions++;
            UpdateQuizScore();
        }
        
        private void CloseQuizPanel()
        {
            ShowARPanel();
        }
        
        // Settings event handlers
        private void OnConfidenceSliderChanged(float value)
        {
            OnSettingsChanged?.Invoke();
        }
        
        private void OnSpeechRateSliderChanged(float value)
        {
            OnSettingsChanged?.Invoke();
        }
        
        private void OnSpeechPitchSliderChanged(float value)
        {
            OnSettingsChanged?.Invoke();
        }
        
        private void OnGestureToggleChanged(bool enabled)
        {
            OnSettingsChanged?.Invoke();
        }
        
        private void OnVoiceToggleChanged(bool enabled)
        {
            OnSettingsChanged?.Invoke();
        }
        
        private void OnLanguageDropdownChanged(int index)
        {
            OnSettingsChanged?.Invoke();
        }
        
        public void ShowMessage(string message, float duration = 3f)
        {
            // TODO: Implement message display system
            Debug.Log($"UIManager: {message}");
        }
        
        public void ShowError(string error)
        {
            // TODO: Implement error display system
            Debug.LogError($"UIManager: {error}");
        }
    }
}

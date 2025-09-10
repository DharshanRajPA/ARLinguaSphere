# ARLinguaSphere API Specification

## Overview

This document describes the internal APIs and interfaces used within the ARLinguaSphere Unity application. These APIs facilitate communication between different modules and provide extension points for future development.

## Table of Contents

1. [Core Interfaces](#core-interfaces)
2. [ML Detection API](#ml-detection-api)
3. [AR Management API](#ar-management-api)
4. [Language Management API](#language-management-api)
5. [Voice Processing API](#voice-processing-api)
6. [Gesture Recognition API](#gesture-recognition-api)
7. [Analytics API](#analytics-api)
8. [Network API](#network-api)
9. [Event System](#event-system)
10. [Data Models](#data-models)

---

## Core Interfaces

### IARLinguaSphereManager

Base interface for all manager components in the system.

```csharp
public interface IARLinguaSphereManager
{
    bool IsInitialized { get; }
    void Initialize();
    void Cleanup();
}
```

### IDataPersistence

Interface for components that need to persist data.

```csharp
public interface IDataPersistence
{
    void SaveData();
    void LoadData();
    void ClearData();
}
```

---

## ML Detection API

### MLManager

Main class for managing machine learning inference.

#### Methods

```csharp
public class MLManager : MonoBehaviour, IARLinguaSphereManager
{
    // Initialization
    public void Initialize();
    
    // Frame processing
    public void ProcessFrame(Texture2D frame);
    
    // Configuration
    public void SetConfidenceThreshold(float threshold);
    public void SetMaxDetections(int maxDetections);
    
    // Properties
    public bool IsProcessing { get; }
    public int QueuedFrames { get; }
    
    // Events
    public event Action<List<Detection>> OnObjectsDetected;
}
```

### Detection Model

```csharp
[Serializable]
public class Detection
{
    public string label;           // Object class name
    public float confidence;       // Detection confidence (0-1)
    public Rect boundingBox;      // Normalized bounding box
    public int classId;           // COCO class ID
}
```

#### Usage Example

```csharp
mlManager.OnObjectsDetected += (detections) => {
    foreach (var detection in detections) {
        if (detection.confidence > 0.7f) {
            PlaceLabel(detection);
        }
    }
};
```

---

## AR Management API

### ARManager

Manages AR Foundation components and AR session lifecycle.

#### Methods

```csharp
public class ARManager : MonoBehaviour
{
    // Session management
    public void StartARSession();
    public void StopARSession();
    
    // Anchor management
    public bool TryPlaceAnchor(Vector2 screenPosition, out ARAnchor anchor);
    public void RemoveAnchor(ARAnchor anchor);
    public void ClearAllAnchors();
    
    // Coordinate conversion
    public Vector3 ScreenToWorldPoint(Vector2 screenPoint, float distance = 1f);
    
    // Camera access
    public Texture2D GetLatestCameraTexture();
    
    // Properties
    public bool IsARSessionRunning { get; }
    public Camera ARCamera { get; }
}
```

### ARLabel

3D label component for displaying text in AR space.

#### Methods

```csharp
public class ARLabel : MonoBehaviour
{
    // Initialization
    public void Initialize(string text, string language = "en");
    
    // Text management
    public void UpdateText(string newText = null, string language = null);
    
    // Appearance
    public void SetVisible(bool visible);
    public void SetScale(float scale);
    public void SetColor(Color color);
    public void SetBackgroundColor(Color color);
    
    // Interaction
    public void OnPointerClick();
    public void DestroyLabel();
    
    // Properties
    public string GetLabelText();
    public string GetOriginalText();
    public string GetLanguage();
    public bool IsVisible();
    
    // Events
    public event Action<ARLabel> OnLabelClicked;
    public event Action<ARLabel> OnLabelDestroyed;
}
```

---

## Language Management API

### LanguageManager

Handles translation and language operations.

#### Methods

```csharp
public class LanguageManager : MonoBehaviour
{
    // Translation
    public string GetTranslation(string key, string targetLanguage = null);
    
    // Language management
    public void SetCurrentLanguage(string languageCode);
    public List<string> GetAvailableLanguages();
    public bool IsLanguageSupported(string languageCode);
    
    // Custom translations
    public void AddCustomTranslation(string key, string language, string translation);
    public void SaveCustomTranslations();
    
    // Data management
    public void LoadOfflineDictionary();
    
    // Properties
    public string currentLanguage;
    public string fallbackLanguage;
    public bool enableOnlineTranslation;
    
    // Events
    public event Action<string, string> OnLanguageChanged;
    public event Action<string, string, string> OnTranslationCompleted;
}
```

#### Translation Dictionary Format

```json
{
  "object_key": {
    "en": "English translation",
    "es": "Spanish translation",
    "fr": "French translation"
  }
}
```

---

## Voice Processing API

### VoiceManager

Manages speech-to-text and text-to-speech functionality.

#### Methods

```csharp
public class VoiceManager : MonoBehaviour
{
    // Speech Recognition
    public void StartListening();
    public void StopListening();
    
    // Text-to-Speech
    public void Speak(string text, string language = null);
    public void StopSpeaking();
    
    // Configuration
    public void SetSpeechRate(float rate);
    public void SetSpeechPitch(float pitch);
    public void SetSpeechVolume(float volume);
    public void SetLanguage(string languageCode);
    
    // Voice command processing
    public void ProcessVoiceCommand(string command);
    
    // Properties
    public bool IsListening { get; }
    public bool IsSpeaking { get; }
    
    // Events
    public event Action<string> OnSpeechRecognized;
    public event Action<string> OnSpeechError;
    public event Action OnSpeechStarted;
    public event Action OnSpeechEnded;
    public event Action OnTTSStarted;
    public event Action OnTTSEnded;
}
```

### Voice Commands

Supported voice commands:

- `"What is this?"` - Trigger object identification
- `"Translate to [language]"` - Change target language
- `"Quiz me"` - Start quiz mode
- `"Remove label"` - Remove current label
- `"Change language"` - Cycle through languages

---

## Gesture Recognition API

### GestureManager

Handles touch and hand gesture recognition.

#### Methods

```csharp
public class GestureManager : MonoBehaviour
{
    // Configuration
    public void SetGestureEnabled(GestureType gestureType, bool enabled);
    public void SetGestureSensitivity(GestureType gestureType, float sensitivity);
    
    // Properties
    public bool enableTouchGestures;
    public bool enableHandGestures;
    
    // Events
    public event Action<GestureType, Vector2> OnGestureDetected;
    public event Action<GestureType, float> OnGestureHeld;
}
```

### GestureType Enumeration

```csharp
public enum GestureType
{
    Tap,              // Single finger tap
    PinchIn,          // Two finger pinch inward
    PinchOut,         // Two finger pinch outward
    SwipeLeft,        // Horizontal swipe left
    SwipeRight,       // Horizontal swipe right
    SwipeUp,          // Vertical swipe up
    SwipeDown,        // Vertical swipe down
    ThumbsUp,         // Hand gesture: thumbs up
    OpenPalm,         // Hand gesture: open palm
    TwoFingerRotate   // Two finger rotation
}
```

---

## Analytics API

### AnalyticsManager

Manages learning analytics and adaptive algorithms.

#### Methods

```csharp
public class AnalyticsManager : MonoBehaviour
{
    // Interaction logging
    public void LogInteraction(string anchorId, string labelKey, 
                              InteractionType action, bool success, 
                              float duration = 0f);
    
    // Word statistics
    public List<string> GetWordsForQuiz(int count = 5);
    public WordStats GetWordStats(string wordKey);
    
    // Data management
    public void SaveSessionData();
    public void SyncToCloud();
    public void ExportUserData();
    public void DeleteUserData();
    
    // Events
    public event Action<InteractionData> OnInteractionLogged;
    public event Action<string, float> OnDifficultyAdjusted;
}
```

### Data Models

```csharp
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

[Serializable]
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

[Serializable]
public class WordStats
{
    public string wordKey;
    public int totalInteractions;
    public int successfulInteractions;
    public float averageResponseTime;
    public float difficultyLevel;
    public long lastSeen;
}
```

---

## Network API

### NetworkManager

Manages multi-user collaboration and cloud synchronization.

#### Methods

```csharp
public class NetworkManager : MonoBehaviour
{
    // Connection management
    public void Connect();
    public void Disconnect();
    
    // Room management
    public void CreateRoom(string roomName = null);
    public void JoinRoom(string roomId);
    public void LeaveRoom();
    
    // Anchor synchronization
    public void SendAnchor(AnchorData anchorData);
    public void RequestAnchors();
    
    // Properties
    public bool IsConnected { get; }
    public bool IsInRoom { get; }
    public string CurrentRoomId { get; }
    
    // Events
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<string> OnRoomJoined;
    public event Action OnRoomLeft;
    public event Action<AnchorData> OnAnchorReceived;
    public event Action<string> OnNetworkError;
}
```

### AnchorData Model

```csharp
[Serializable]
public class AnchorData
{
    public string id;
    public Vector3 position;
    public Quaternion rotation;
    public string labelKey;
    public string creatorId;
    public long timestamp;
}
```

---

## Event System

### Global Events

The system uses a centralized event system for loose coupling between components.

#### Event Categories

1. **System Events**
   - `OnSystemInitialized`
   - `OnARSessionStarted`
   - `OnARSessionStopped`

2. **Interaction Events**
   - `OnObjectDetected`
   - `OnLabelPlaced`
   - `OnLabelClicked`
   - `OnGesturePerformed`

3. **Language Events**
   - `OnLanguageChanged`
   - `OnTranslationCompleted`

4. **Network Events**
   - `OnConnected`
   - `OnRoomJoined`
   - `OnAnchorReceived`

### Event Usage Example

```csharp
// Subscribe to events
arManager.OnObjectDetected += HandleObjectDetection;
languageManager.OnLanguageChanged += HandleLanguageChange;

// Unsubscribe (important for cleanup)
arManager.OnObjectDetected -= HandleObjectDetection;
```

---

## Error Handling

### Error Codes

```csharp
public enum ARLinguaSphereErrorCode
{
    None = 0,
    InitializationFailed = 1001,
    ARSessionFailed = 1002,
    MLInferenceFailed = 1003,
    NetworkConnectionFailed = 1004,
    VoiceRecognitionFailed = 1005,
    TranslationFailed = 1006,
    DataPersistenceFailed = 1007,
    PermissionDenied = 1008,
    ModelLoadFailed = 1009,
    InvalidConfiguration = 1010
}
```

### Error Handling Pattern

```csharp
try
{
    // Operation
    result = PerformOperation();
}
catch (ARLinguaSphereException ex)
{
    Debug.LogError($"ARLinguaSphere Error {ex.ErrorCode}: {ex.Message}");
    // Handle specific error
}
catch (Exception ex)
{
    Debug.LogError($"Unexpected error: {ex.Message}");
    // Handle general error
}
```

---

## Performance Considerations

### Frame Rate Targets

- **ML Inference**: ≤ 60ms per frame
- **AR Tracking**: 30+ FPS
- **UI Responsiveness**: 60 FPS
- **Network Sync**: < 300ms latency

### Memory Management

- **Texture Pooling**: Reuse camera textures
- **Model Caching**: Cache ML models in memory
- **Object Pooling**: Pool AR labels and UI elements
- **Garbage Collection**: Minimize allocations in Update loops

### Threading

- **Main Thread**: UI updates, Unity API calls
- **Background Thread**: ML inference, network requests
- **Coroutines**: Async operations, animations

---

## Security Considerations

### Data Protection

- **API Keys**: Stored server-side, never in client
- **User Data**: Encrypted at rest and in transit
- **Session Management**: Secure token-based authentication
- **Input Validation**: Sanitize all user inputs

### Privacy Compliance

- **GDPR**: Data export and deletion capabilities
- **CCPA**: User consent and opt-out mechanisms
- **Data Minimization**: Collect only necessary data
- **Anonymization**: Remove PII from analytics

---

## Versioning

### API Version: 1.0.0

Breaking changes will increment the major version. All APIs maintain backward compatibility within major versions.

### Deprecation Policy

Deprecated APIs will be marked with `[Obsolete]` attribute and removed in the next major version.

---

## Support

For technical support or questions about these APIs, please refer to:

- **Documentation**: `/Documentation/`
- **Examples**: `/Examples/`
- **Issues**: GitHub Issues
- **Community**: Discord Server

---

*This API specification is subject to change. Please check for updates regularly.*

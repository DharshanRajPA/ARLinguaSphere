# ARLinguaSphere 🌍

**ARLinguaSphere** (a.k.a. LinguaSphere) is a production-grade, Android-first, cross-platform AR language learning application built with Unity and AR Foundation. It combines on-device object detection, AR anchors, gesture controls, voice I/O, offline-first multilingual dictionaries, and collaborative multi-user sessions to create an immersive language learning experience.

## 🚀 Features

- **On-device Object Detection**: YOLOv8/MobileNet integrated with TensorFlow Lite for real-time object recognition
- **AR Anchors & Rendering**: Robust label placement using ARCore with shared/cloud anchors for multi-user sync
- **Gesture Recognition**: MediaPipe Hands integration for intuitive controls (pinch, tap, swipe, thumbs-up)
- **Voice I/O**: Speech-to-Text (STT) and Text-to-Speech (TTS) with Android native bridge and offline fallback
- **Offline-first Dictionary**: 1,500-3,000 core words across 10 languages with online GPT/Translate fallback
- **Multi-user Collaboration**: Real-time anchor sharing using Firebase Realtime DB and WebRTC
- **Adaptive Learning Analytics**: Local SQLite + Firebase Analytics with intelligent quiz adaptation
- **Cross-platform**: Unity 2022 LTS with AR Foundation (Android-first, iOS support)

## 🛠️ Tech Stack

- **Engine**: Unity 2022 LTS with AR Foundation
- **AR Runtime**: ARCore (Android), ARKit (iOS)
- **ML Inference**: TensorFlow Lite / ONNX
- **Object Detection**: YOLOv8 small / MobileNetV2 / EfficientNet-lite
- **Hand Gestures**: MediaPipe Hands
- **Voice**: Android SpeechRecognizer + TTS
- **Backend**: Firebase (Auth, Realtime DB, Analytics, Crashlytics)
- **Database**: SQLite (local), Firebase Firestore (cloud)
- **CI/CD**: GitHub Actions
- **Translation**: Google Cloud Translate + GPT-4 fallback

## 📋 Prerequisites

- Unity 2022.3 LTS or newer
- Android SDK (API level 24+)
- ARCore supported Android device
- Visual Studio 2022 or JetBrains Rider
- Git

## 🚀 Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/arlinguasphere.git
   cd arlinguasphere
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Open the project folder in Unity 2022.3 LTS

3. **Configure Android Build Settings**
   - File → Build Settings
   - Select Android platform
   - Switch Platform
   - Configure Player Settings for ARCore

4. **Build and Deploy**
   - Connect ARCore-compatible Android device
   - Build and Run

## 📁 Project Structure

```
/Assets
  /Scripts
    /Core/           # Core game logic and managers
    /AR/             # AR Foundation integration
    /ML/             # Machine learning and object detection
    /Gesture/        # Gesture recognition
    /Voice/          # Speech-to-Text and Text-to-Speech
    /UI/             # User interface components
    /Network/        # Multi-user networking
    /Analytics/      # Learning analytics and adaptation
  /Prefabs
    LabelPrefab.prefab
    AnchorPrefab.prefab
  /Resources
    offline_dictionary.json
  /Scenes
    MainARScene.unity
/BuildScripts        # CI/CD build scripts
/Documentation       # Architecture and API docs
/Tests              # Unit and integration tests
```

## 🧪 Testing

Run the test suite:
```bash
# Unit tests
Unity -batchmode -quit -projectPath . -runTests -testPlatform playmode

# Integration tests
Unity -batchmode -quit -projectPath . -runTests -testPlatform editmode
```

## 📊 Performance Targets

- **ML Inference**: ≤ 60ms per frame (target 25+ FPS)
- **Memory Footprint**: ≤ 200MB additional
- **AR Anchor Drift**: < 20cm after 5m movement
- **Multi-user Sync**: < 300ms local, < 1s mobile network
- **Gesture Recognition**: < 100ms latency, ≥ 90% accuracy
- **Voice Recognition**: ≥ 85% accuracy in various environments

## 🔧 Configuration

1. **API Keys**: Copy `.env.example` to `.env` and fill in your API keys
2. **Firebase**: Configure Firebase project and add `google-services.json`
3. **Model Files**: Place TensorFlow Lite models in `Assets/StreamingAssets/Models/`

## 📱 Supported Languages

- English (en)
- Spanish (es)
- French (fr)
- German (de)
- Italian (it)
- Portuguese (pt)
- Chinese (zh)
- Japanese (ja)
- Korean (ko)
- Hindi (hi)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🎯 Roadmap

- [x] Sprint 0: Project scaffolding
- [ ] Sprint 1: Core detection & labels
- [ ] Sprint 2: Gesture & Voice
- [ ] Sprint 3: Multi-user & Sync
- [ ] Sprint 4: Analytics & Adaptive Quiz
- [ ] Sprint 5: Polish, testing & CI

## 📞 Support

For support, email support@arlinguasphere.com or join our Discord community.

## 🙏 Acknowledgments

- Unity Technologies for AR Foundation
- Google for ARCore and TensorFlow Lite
- MediaPipe team for hand gesture recognition
- Firebase team for backend services

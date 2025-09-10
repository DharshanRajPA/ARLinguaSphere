# ARLinguaSphere Deployment Guide

## Overview

This guide covers the complete deployment process for ARLinguaSphere, from development environment setup to production release on Google Play Store.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Environment Setup](#environment-setup)
3. [Development Deployment](#development-deployment)
4. [Staging Deployment](#staging-deployment)
5. [Production Deployment](#production-deployment)
6. [CI/CD Pipeline](#cicd-pipeline)
7. [Monitoring & Analytics](#monitoring--analytics)
8. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Software

- **Unity 2022.3 LTS** or newer
- **Android Studio** with Android SDK API 24+
- **Git** for version control
- **Node.js** (for Firebase CLI)
- **Python 3.8+** (for build scripts)

### Required Accounts

- **Unity Account** with Pro license
- **Google Cloud Platform** account
- **Firebase** project
- **Google Play Console** developer account
- **GitHub** account for CI/CD

### Hardware Requirements

- **Development Machine**: 16GB RAM, 500GB SSD
- **Test Devices**: ARCore-compatible Android devices
  - Google Pixel 6a or newer
  - Samsung Galaxy S21 or newer
  - OnePlus 9 or newer

---

## Environment Setup

### 1. Clone Repository

```bash
git clone https://github.com/yourusername/arlinguasphere.git
cd arlinguasphere
```

### 2. Configure Environment Variables

```bash
# Copy environment template
cp env.example .env

# Edit .env file with your configuration
nano .env
```

### 3. Install Dependencies

```bash
# Install Firebase CLI
npm install -g firebase-tools

# Install Unity Cloud Build CLI (optional)
npm install -g unity-cloud-build-api

# Install Python dependencies
pip install -r requirements.txt
```

### 4. Firebase Setup

```bash
# Login to Firebase
firebase login

# Initialize Firebase project
firebase init

# Deploy Firebase rules and functions
firebase deploy --only firestore:rules,functions
```

### 5. Unity Project Setup

1. Open Unity Hub
2. Add project from cloned directory
3. Open project in Unity 2022.3 LTS
4. Configure build settings:
   - Platform: Android
   - API Level: 24 (Android 7.0)
   - Target Architecture: ARM64
   - Scripting Backend: IL2CPP

---

## Development Deployment

### Local Development

1. **Configure Unity Settings**
   ```csharp
   // In Unity Editor
   File → Build Settings → Android
   Player Settings → XR Plug-in Management → ARCore
   ```

2. **Setup AR Foundation**
   - Install AR Foundation package
   - Configure ARCore provider
   - Setup AR Camera and AR Session Origin

3. **Configure Android Manifest**
   ```xml
   <!-- Assets/Plugins/Android/AndroidManifest.xml -->
   <uses-permission android:name="android.permission.CAMERA" />
   <uses-permission android:name="android.permission.RECORD_AUDIO" />
   <uses-permission android:name="android.permission.INTERNET" />
   <uses-feature android:name="android.hardware.camera.ar" android:required="true" />
   ```

4. **Build and Test**
   ```bash
   # Build APK
   Unity -batchmode -quit -projectPath . -buildTarget Android -buildPath build/debug.apk

   # Install on device
   adb install build/debug.apk
   ```

### Debug Build

```bash
# Create debug build with development flags
./scripts/build-debug.sh
```

---

## Staging Deployment

### Firebase App Distribution

1. **Setup App Distribution**
   ```bash
   # Add testers
   firebase appdistribution:testers:add tester1@example.com tester2@example.com

   # Create tester group
   firebase appdistribution:group:create --group-alias "internal-testers" \
     --display-name "Internal Testers"
   ```

2. **Deploy to Staging**
   ```bash
   # Build staging APK
   ./scripts/build-staging.sh

   # Upload to App Distribution
   firebase appdistribution:distribute build/staging.apk \
     --app 1:123456789:android:abcdef \
     --groups "internal-testers" \
     --release-notes "Staging build - $(git log -1 --pretty=format:'%h %s')"
   ```

### Staging Environment Configuration

```bash
# staging.env
ENVIRONMENT=staging
DEBUG_MODE=true
ENABLE_LOGGING=true
FIREBASE_PROJECT_ID=arlinguasphere-staging
API_BASE_URL=https://staging-api.arlinguasphere.com
```

---

## Production Deployment

### Pre-Production Checklist

- [ ] All unit tests passing
- [ ] Integration tests completed
- [ ] Performance benchmarks met
- [ ] Security audit completed
- [ ] Accessibility testing done
- [ ] Multi-device testing completed
- [ ] Privacy policy updated
- [ ] Terms of service updated

### Google Play Store Deployment

1. **Prepare Release Build**
   ```bash
   # Set production environment
   export ENVIRONMENT=production
   export BUILD_TYPE=release

   # Build signed AAB
   ./scripts/build-production.sh
   ```

2. **Upload to Play Console**
   ```bash
   # Using Google Play CLI
   ./scripts/upload-to-play-store.sh build/production.aab
   ```

3. **Release Configuration**
   - **Release Track**: Internal → Alpha → Beta → Production
   - **Rollout Percentage**: Start with 5%, gradually increase
   - **Target Audience**: All countries
   - **Device Compatibility**: ARCore-compatible devices only

### App Store Optimization (ASO)

1. **Store Listing**
   - Title: "ARLinguaSphere - AR Language Learning"
   - Short Description: "Learn languages through immersive AR"
   - Full Description: Include features, benefits, and screenshots

2. **Screenshots**
   - AR labels in action (5 screenshots)
   - UI overview (2 screenshots)
   - Multi-language support demo (2 screenshots)
   - Quiz interface (1 screenshot)

3. **Keywords**
   - Primary: AR, language learning, augmented reality
   - Secondary: translation, education, multilingual
   - Long-tail: AR language tutor, immersive learning

---

## CI/CD Pipeline

### GitHub Actions Workflow

The deployment pipeline is automated through GitHub Actions:

```yaml
# .github/workflows/deploy.yml
name: Deploy ARLinguaSphere

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Run Tests
        run: ./scripts/run-tests.sh

  build:
    needs: test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Build APK
        run: ./scripts/build-ci.sh
      - name: Upload Artifacts
        uses: actions/upload-artifact@v3

  deploy-staging:
    needs: build
    if: github.ref == 'refs/heads/develop'
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to Firebase
        run: ./scripts/deploy-staging.sh

  deploy-production:
    needs: build
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    environment: production
    steps:
      - name: Deploy to Play Store
        run: ./scripts/deploy-production.sh
```

### Deployment Scripts

1. **build-ci.sh**
   ```bash
   #!/bin/bash
   set -e
   
   echo "Building ARLinguaSphere for CI..."
   
   # Set Unity license
   unity-license activate
   
   # Build project
   Unity -batchmode -quit -projectPath . \
     -buildTarget Android \
     -buildPath build/arlinguasphere.apk \
     -logFile build.log
   
   echo "Build completed successfully"
   ```

2. **deploy-staging.sh**
   ```bash
   #!/bin/bash
   set -e
   
   echo "Deploying to staging..."
   
   # Upload to Firebase App Distribution
   firebase appdistribution:distribute build/arlinguasphere.apk \
     --app $FIREBASE_APP_ID \
     --groups "internal-testers" \
     --release-notes-file release-notes.txt
   
   echo "Staging deployment completed"
   ```

---

## Monitoring & Analytics

### Firebase Analytics Setup

1. **Configure Analytics**
   ```csharp
   // In AnalyticsManager.cs
   FirebaseAnalytics.LogEvent("app_launch");
   FirebaseAnalytics.LogEvent("ar_session_started");
   FirebaseAnalytics.LogEvent("object_detected", "object_type", objectType);
   ```

2. **Custom Events**
   - `ar_label_placed`
   - `language_changed`
   - `quiz_completed`
   - `gesture_performed`
   - `voice_command_used`

### Performance Monitoring

1. **Firebase Performance**
   ```csharp
   // Track custom traces
   var trace = FirebasePerformance.Instance.NewTrace("ml_inference");
   trace.Start();
   // ... ML inference code ...
   trace.Stop();
   ```

2. **Key Metrics**
   - App startup time
   - AR session initialization time
   - ML inference latency
   - Network request duration
   - Memory usage patterns

### Crash Reporting

```csharp
// Firebase Crashlytics
FirebaseCrashlytics.Log("AR session started");
FirebaseCrashlytics.SetCustomKey("user_language", currentLanguage);
```

---

## Troubleshooting

### Common Issues

1. **Build Failures**
   ```bash
   # Clear Unity cache
   rm -rf Library/
   rm -rf Temp/
   
   # Reimport assets
   Unity -batchmode -quit -projectPath . -importPackage
   ```

2. **AR Session Issues**
   - Check ARCore compatibility
   - Verify camera permissions
   - Ensure proper lighting conditions
   - Update ARCore services on device

3. **ML Inference Problems**
   - Verify model file exists in StreamingAssets
   - Check TensorFlow Lite version compatibility
   - Monitor memory usage during inference
   - Validate input tensor shapes

4. **Network Connectivity**
   - Check Firebase configuration
   - Verify internet permissions
   - Test with different network conditions
   - Implement offline fallback

### Debug Tools

1. **Unity Profiler**
   - Monitor CPU usage
   - Track memory allocations
   - Analyze rendering performance

2. **Android Debug Bridge (ADB)**
   ```bash
   # View device logs
   adb logcat | grep Unity
   
   # Monitor memory usage
   adb shell dumpsys meminfo com.arlinguasphere.app
   
   # Check network activity
   adb shell netstat
   ```

3. **Firebase Debug View**
   ```bash
   # Enable debug mode
   adb shell setprop debug.firebase.analytics.app com.arlinguasphere.app
   ```

### Performance Optimization

1. **ML Inference**
   - Use GPU acceleration when available
   - Implement frame skipping
   - Optimize model size
   - Cache inference results

2. **AR Rendering**
   - Limit number of active labels
   - Use object pooling for labels
   - Implement LOD system
   - Optimize shader performance

3. **Memory Management**
   - Dispose textures properly
   - Use object pooling
   - Implement garbage collection optimization
   - Monitor memory leaks

---

## Security Considerations

### API Key Management

1. **Server-Side Keys**
   - Store sensitive keys on server
   - Use proxy endpoints for API calls
   - Implement rate limiting
   - Monitor for abuse

2. **Client-Side Security**
   - Obfuscate code
   - Implement certificate pinning
   - Validate server certificates
   - Use encrypted local storage

### Privacy Compliance

1. **GDPR Compliance**
   - Implement data export
   - Provide data deletion
   - Obtain user consent
   - Maintain privacy policy

2. **Data Protection**
   - Encrypt sensitive data
   - Minimize data collection
   - Implement data retention policies
   - Regular security audits

---

## Rollback Procedures

### Emergency Rollback

1. **Play Store Rollback**
   ```bash
   # Halt current rollout
   ./scripts/halt-rollout.sh
   
   # Rollback to previous version
   ./scripts/rollback-production.sh
   ```

2. **Firebase Rollback**
   ```bash
   # Revert database rules
   firebase firestore:rules:release --project previous-rules
   
   # Rollback cloud functions
   firebase functions:deploy --project previous-version
   ```

### Gradual Rollback

1. Reduce rollout percentage to 0%
2. Monitor crash reports and user feedback
3. Fix issues in new version
4. Redeploy with fixes

---

## Support & Maintenance

### Regular Maintenance Tasks

- **Weekly**: Review crash reports and performance metrics
- **Monthly**: Update dependencies and security patches
- **Quarterly**: Performance optimization and feature updates
- **Annually**: Major version updates and architecture reviews

### Support Channels

- **GitHub Issues**: Bug reports and feature requests
- **Discord**: Community support and discussions
- **Email**: support@arlinguasphere.com
- **Documentation**: Comprehensive guides and API docs

---

*This deployment guide is regularly updated. Please check for the latest version before deploying.*

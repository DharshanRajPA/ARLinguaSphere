# Compilation Error Prevention Guide

## Overview
This document provides comprehensive guidelines to prevent compilation errors in Unity projects, specifically for ARLinguaSphere.

## Common Compilation Error Categories

### 1. Editor-Only API Usage in Runtime Scripts

**❌ PROBLEMATIC:**
```csharp
// In a MonoBehaviour script (runtime)
using UnityEditor;
using UnityEditor.PackageManager;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        Client.Add("com.unity.xr.arfoundation"); // ❌ Editor-only API
        AssetDatabase.Refresh(); // ❌ Editor-only API
    }
}
```

**✅ CORRECT:**
```csharp
// Option 1: Use preprocessor directives
public class MyScript : MonoBehaviour
{
    void Start()
    {
        #if UNITY_EDITOR
            // Editor-only code here
            UnityEditor.AssetDatabase.Refresh();
        #else
            // Runtime code here
            Application.Quit();
        #endif
    }
}

// Option 2: Move to Editor folder
// File: Assets/Editor/MyEditorScript.cs
using UnityEditor;
public class MyEditorScript
{
    [MenuItem("Tools/Do Something")]
    static void DoSomething()
    {
        AssetDatabase.Refresh(); // ✅ OK in editor scripts
    }
}
```

### 2. Missing Package Dependencies

**❌ PROBLEMATIC:**
```csharp
using UnityEngine.XR.ARFoundation; // ❌ May not be available
using TMPro; // ❌ May not be available
```

**✅ CORRECT:**
```csharp
// Check package availability first
#if UNITY_XR_ARFOUNDATION
    using UnityEngine.XR.ARFoundation;
#endif

#if UNITY_TEXTMESHPRO
    using TMPro;
#endif

public class MyScript : MonoBehaviour
{
    void Start()
    {
        #if UNITY_XR_ARFOUNDATION
            // AR Foundation code here
        #endif
        
        #if UNITY_TEXTMESHPRO
            // TextMeshPro code here
        #endif
    }
}
```

### 3. Deprecated API Usage

**❌ DEPRECATED:**
```csharp
var obj = FindObjectOfType<MyComponent>(); // ❌ Deprecated
var objs = FindObjectsOfType<MyComponent>(); // ❌ Deprecated
```

**✅ CURRENT:**
```csharp
var obj = FindFirstObjectByType<MyComponent>(); // ✅ Current
var objs = FindObjectsByType<MyComponent>(FindObjectsSortMode.None); // ✅ Current
```

### 4. Incorrect Namespace Imports

**❌ PROBLEMATIC:**
```csharp
using UnityEditor.Build; // ❌ May not exist in all Unity versions
using UnityEditor.PackageManager; // ❌ May not be available
```

**✅ CORRECT:**
```csharp
// Only import what you actually use
using UnityEngine;
using System.Collections;

// For editor-only functionality, use preprocessor directives
#if UNITY_EDITOR
    using UnityEditor;
#endif
```

## Prevention Checklist

### Before Adding New Scripts:
- [ ] Is this a runtime script or editor script?
- [ ] Are all required packages installed?
- [ ] Are you using current APIs (not deprecated ones)?
- [ ] Are namespace imports correct and minimal?

### Before Using Editor APIs:
- [ ] Is the script in the Editor folder?
- [ ] Are editor APIs wrapped in `#if UNITY_EDITOR`?
- [ ] Is there a runtime fallback?

### Before Using Package-Specific APIs:
- [ ] Is the package installed in manifest.json?
- [ ] Are you using conditional compilation?
- [ ] Is there a fallback for missing packages?

## Package Dependencies

### Required Packages for ARLinguaSphere:
```json
{
  "dependencies": {
    "com.unity.xr.arfoundation": "6.2.0",
    "com.unity.xr.arcore": "6.2.0",
    "com.unity.xr.core-utils": "2.3.0",
    "com.unity.xr.management": "4.5.1",
    "com.unity.ugui": "2.0.0",
    "com.unity.textmeshpro": "3.2.0",
    "com.unity.inputsystem": "1.8.2",
    "com.unity.feature.ar": "1.0.2"
  }
}
```

### Conditional Compilation Symbols:
- `UNITY_XR_ARFOUNDATION` - AR Foundation package
- `UNITY_TEXTMESHPRO` - TextMeshPro package
- `UNITY_UGUI` - Unity UI package
- `UNITY_EDITOR` - Unity Editor (not in builds)

## Testing and Validation

### Use the CompilationErrorPrevention Script:
1. Add `CompilationErrorPrevention` component to a GameObject
2. Right-click and select "Run Compilation Health Check"
3. Review any warnings or errors
4. Fix issues before building

### Manual Checks:
1. Build the project regularly during development
2. Check Console for compilation errors
3. Verify all scripts compile in both Editor and Runtime
4. Test on target platforms

## Common Patterns to Avoid

### ❌ DON'T:
```csharp
// Mix editor and runtime code
public class BadScript : MonoBehaviour
{
    void Start()
    {
        #if UNITY_EDITOR
            AssetDatabase.Refresh();
        #endif
        // Runtime code mixed with editor code
    }
}

// Use deprecated APIs
var obj = FindObjectOfType<MyComponent>();

// Assume packages are always available
using UnityEngine.XR.ARFoundation; // Without checking
```

### ✅ DO:
```csharp
// Separate concerns
public class RuntimeScript : MonoBehaviour
{
    void Start()
    {
        // Only runtime code here
    }
}

// Use current APIs
var obj = FindFirstObjectByType<MyComponent>();

// Check package availability
#if UNITY_XR_ARFOUNDATION
    using UnityEngine.XR.ARFoundation;
#endif
```

## Emergency Fixes

If you encounter compilation errors:

1. **Check Console** - Look for specific error messages
2. **Identify the Script** - Find which script has the error
3. **Check Imports** - Verify all using statements are correct
4. **Check APIs** - Ensure you're using current, available APIs
5. **Test Build** - Verify the fix works in a build

## Tools and Resources

- Unity Console - Shows compilation errors
- Package Manager - Manages package dependencies
- CompilationErrorPrevention Script - Automated health checks
- Unity Documentation - API reference and examples

Remember: Prevention is better than fixing! Follow these guidelines to avoid compilation errors in the future.

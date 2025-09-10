using UnityEngine;
using System;
using System.Collections.Generic;

namespace ARLinguaSphere.Gesture
{
    /// <summary>
    /// Manages gesture recognition and input handling
    /// </summary>
    public class GestureManager : MonoBehaviour
    {
        [Header("Gesture Settings")]
        public float gestureTimeout = 2f;
        public float pinchThreshold = 0.1f;
        public float swipeThreshold = 50f;
        public float swipeTimeThreshold = 0.5f;
        
        [Header("Input Settings")]
        public bool enableTouchGestures = true;
        public bool enableHandGestures = true;
        
        private bool isInitialized = false;
        private Vector2 lastTouchPosition;
        private float lastTouchTime;
        private bool isPinching = false;
        private float pinchStartDistance;
        
        // Hand tracking
        private IMediaPipeHands hands;
        private float lastHandGestureTime;
        public float handGestureCooldown = 0.8f;
        
        // Events
        public event Action<GestureType, Vector2> OnGestureDetected;
        public event Action<GestureType, float> OnGestureHeld;
        
        public void Initialize()
        {
            Debug.Log("GestureManager: Initializing gesture systems...");
            
            // Initialize MediaPipe Hands or other gesture recognition systems
            InitializeHandGestureRecognition();
            
            isInitialized = true;
            Debug.Log("GestureManager: Gesture systems initialized!");
        }
        
        private void InitializeHandGestureRecognition()
        {
            if (!enableHandGestures) return;
            
            var handsComponent = GameObject.FindFirstObjectByType<MediaPipeHands>();
            if (handsComponent == null)
            {
                var go = new GameObject("MediaPipeHands");
                handsComponent = go.AddComponent<MediaPipeHands>();
            }
            hands = handsComponent;
            hands.Initialize(1, true);
            hands.OnHandLandmarks += OnHandLandmarks;
            
            Debug.Log("GestureManager: Hand gesture recognition initialized");
        }
        
        private void Update()
        {
            if (!isInitialized) return;
            
            HandleTouchInput();
            // Hand gestures are event-driven via OnHandLandmarks
        }
        
        private void HandleTouchInput()
        {
            if (!enableTouchGestures) return;
            
            // Handle single touch gestures
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchBegan(touch.position);
                        break;
                    case TouchPhase.Moved:
                        OnTouchMoved(touch.position);
                        break;
                    case TouchPhase.Ended:
                        OnTouchEnded(touch.position);
                        break;
                }
            }
            // Handle multi-touch gestures
            else if (Input.touchCount == 2)
            {
                HandlePinchGesture();
            }
        }
        
        private void OnTouchBegan(Vector2 position)
        {
            lastTouchPosition = position;
            lastTouchTime = Time.time;
        }
        
        private void OnTouchMoved(Vector2 position)
        {
            Vector2 deltaPosition = position - lastTouchPosition;
            float deltaTime = Time.time - lastTouchTime;
            
            // Detect swipe gestures
            if (deltaPosition.magnitude > swipeThreshold && deltaTime < swipeTimeThreshold)
            {
                if (Mathf.Abs(deltaPosition.x) > Mathf.Abs(deltaPosition.y))
                {
                    // Horizontal swipe
                    GestureType swipeType = deltaPosition.x > 0 ? GestureType.SwipeRight : GestureType.SwipeLeft;
                    OnGestureDetected?.Invoke(swipeType, position);
                }
                else
                {
                    // Vertical swipe
                    GestureType swipeType = deltaPosition.y > 0 ? GestureType.SwipeUp : GestureType.SwipeDown;
                    OnGestureDetected?.Invoke(swipeType, position);
                }
            }
            
            lastTouchPosition = position;
            lastTouchTime = Time.time;
        }
        
        private void OnTouchEnded(Vector2 position)
        {
            float deltaTime = Time.time - lastTouchTime;
            Vector2 deltaPosition = position - lastTouchPosition;
            
            // Detect tap gestures
            if (deltaTime < gestureTimeout && deltaPosition.magnitude < 10f)
            {
                OnGestureDetected?.Invoke(GestureType.Tap, position);
            }
        }
        
        private void HandlePinchGesture()
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);
            
            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                pinchStartDistance = currentDistance;
                isPinching = true;
            }
            else if (isPinching && (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved))
            {
                float pinchDelta = currentDistance - pinchStartDistance;
                
                if (Mathf.Abs(pinchDelta) > pinchThreshold)
                {
                    GestureType pinchType = pinchDelta > 0 ? GestureType.PinchOut : GestureType.PinchIn;
                    Vector2 centerPosition = (touch1.position + touch2.position) / 2f;
                    OnGestureDetected?.Invoke(pinchType, centerPosition);
                    
                    pinchStartDistance = currentDistance;
                }
            }
            else if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
            {
                isPinching = false;
            }
        }
        
        private void OnHandLandmarks(HandLandmarks landmarks)
        {
            if (!enableHandGestures) return;
            if (Time.time - lastHandGestureTime < handGestureCooldown) return;
            if (landmarks == null || landmarks.keypoints == null || landmarks.keypoints.Length == 0) return;
            
            var gesture = ClassifyHandGesture(landmarks);
            if (gesture.HasValue)
            {
                lastHandGestureTime = Time.time;
                OnGestureDetected?.Invoke(gesture.Value, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
            }
        }
        
        private GestureType? ClassifyHandGesture(HandLandmarks landmarks)
        {
            // NOTE: This is a naive heuristic placeholder. Replace with a trained classifier.
            // Use confidence as a proxy to toggle between gestures for demo.
            if (landmarks.confidence > 0.85f)
            {
                return GestureType.ThumbsUp;
            }
            
            // If lower confidence, treat as open palm
            if (landmarks.confidence > 0.6f)
            {
                return GestureType.OpenPalm;
            }
            
            return null;
        }
        
        public void SetGestureEnabled(GestureType gestureType, bool enabled)
        {
            // TODO: Implement per-gesture enable/disable
            Debug.Log($"GestureManager: {gestureType} gesture {(enabled ? "enabled" : "disabled")}");
        }
        
        public void SetGestureSensitivity(GestureType gestureType, float sensitivity)
        {
            // TODO: Implement gesture sensitivity adjustment
            Debug.Log($"GestureManager: {gestureType} sensitivity set to {sensitivity}");
        }
    }
    
    /// <summary>
    /// Types of gestures that can be detected
    /// </summary>
    public enum GestureType
    {
        Tap,
        PinchIn,
        PinchOut,
        SwipeLeft,
        SwipeRight,
        SwipeUp,
        SwipeDown,
        ThumbsUp,
        OpenPalm,
        TwoFingerRotate
    }
}

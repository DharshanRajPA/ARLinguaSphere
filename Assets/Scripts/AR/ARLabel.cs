using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ARLinguaSphere.AR
{
    /// <summary>
    /// AR label component that displays 3D text in AR space
    /// </summary>
    public class ARLabel : MonoBehaviour
    {
        [Header("Label Settings")]
        public string labelText = "Object";
        public string originalLanguage = "en";
        public string translatedText = "";
        public float labelScale = 1f;
        public float maxDistance = 5f;
        public float minDistance = 0.5f;
        
        [Header("UI Components")]
        public TextMeshProUGUI textComponent;
        public Image backgroundImage;
        public Canvas labelCanvas;
        
        [Header("Animation Settings")]
        public bool enableFadeIn = true;
        public float fadeInDuration = 0.5f;
        public bool enableScaleAnimation = true;
        public float scaleAnimationDuration = 0.3f;
        
        [Header("Look At Camera")]
        public bool lookAtCamera = true;
        public bool lockYAxis = true;
        
        private Camera arCamera;
        private Vector3 originalScale;
        private bool isInitialized = false;
        private bool isVisible = true;
        
        // Events
        public event Action<ARLabel> OnLabelClicked;
        public event Action<ARLabel> OnLabelDestroyed;
        
        public void Initialize(string text, string language = "en")
        {
            labelText = text;
            originalLanguage = language;
            translatedText = text;
            
            // Get AR camera reference
            arCamera = Camera.main;
            if (arCamera == null)
            {
                arCamera = FindObjectOfType<Camera>();
            }
            
            // Store original scale
            originalScale = transform.localScale;
            
            // Setup UI components
            SetupUIComponents();
            
            // Start animations
            if (enableFadeIn)
            {
                StartFadeInAnimation();
            }
            
            if (enableScaleAnimation)
            {
                StartScaleAnimation();
            }
            
            isInitialized = true;
        }
        
        private void SetupUIComponents()
        {
            // Create canvas if not assigned
            if (labelCanvas == null)
            {
                labelCanvas = GetComponentInChildren<Canvas>();
                if (labelCanvas == null)
                {
                    var canvasObj = new GameObject("LabelCanvas");
                    canvasObj.transform.SetParent(transform);
                    canvasObj.transform.localPosition = Vector3.zero;
                    canvasObj.transform.localRotation = Quaternion.identity;
                    
                    labelCanvas = canvasObj.AddComponent<Canvas>();
                    labelCanvas.renderMode = RenderMode.WorldSpace;
                    labelCanvas.worldCamera = arCamera;
                }
            }
            
            // Create text component if not assigned
            if (textComponent == null)
            {
                var textObj = new GameObject("LabelText");
                textObj.transform.SetParent(labelCanvas.transform);
                textObj.transform.localPosition = Vector3.zero;
                textObj.transform.localRotation = Quaternion.identity;
                
                textComponent = textObj.AddComponent<TextMeshProUGUI>();
                textComponent.text = translatedText;
                textComponent.fontSize = 24;
                textComponent.color = Color.white;
                textComponent.alignment = TextAlignmentOptions.Center;
            }
            
            // Create background if not assigned
            if (backgroundImage == null)
            {
                var bgObj = new GameObject("Background");
                bgObj.transform.SetParent(labelCanvas.transform);
                bgObj.transform.localPosition = Vector3.zero;
                bgObj.transform.localRotation = Quaternion.identity;
                bgObj.transform.SetAsFirstSibling(); // Put behind text
                
                backgroundImage = bgObj.AddComponent<Image>();
                backgroundImage.color = new Color(0, 0, 0, 0.7f);
                
                // Set background size
                var rectTransform = backgroundImage.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(200, 50);
            }
            
            // Update text
            UpdateText();
        }
        
        private void Update()
        {
            if (!isInitialized) return;
            
            // Look at camera
            if (lookAtCamera && arCamera != null)
            {
                Vector3 lookDirection = arCamera.transform.position - transform.position;
                
                if (lockYAxis)
                {
                    lookDirection.y = 0;
                }
                
                if (lookDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDirection);
                }
            }
            
            // Distance-based scaling
            if (arCamera != null)
            {
                float distance = Vector3.Distance(transform.position, arCamera.transform.position);
                float scaleFactor = Mathf.Clamp(distance / maxDistance, 0.1f, 1f);
                transform.localScale = originalScale * scaleFactor * labelScale;
            }
            
            // Visibility culling
            if (arCamera != null)
            {
                float distance = Vector3.Distance(transform.position, arCamera.transform.position);
                bool shouldBeVisible = distance >= minDistance && distance <= maxDistance;
                
                if (shouldBeVisible != isVisible)
                {
                    SetVisible(shouldBeVisible);
                }
            }
        }
        
        public void UpdateText(string newText = null, string language = null)
        {
            if (!string.IsNullOrEmpty(newText))
            {
                translatedText = newText;
            }
            
            if (!string.IsNullOrEmpty(language))
            {
                originalLanguage = language;
            }
            
            if (textComponent != null)
            {
                textComponent.text = translatedText;
            }
        }
        
        public void SetVisible(bool visible)
        {
            isVisible = visible;
            gameObject.SetActive(visible);
        }
        
        public void SetScale(float scale)
        {
            labelScale = scale;
            transform.localScale = originalScale * scale;
        }
        
        public void SetColor(Color color)
        {
            if (textComponent != null)
            {
                textComponent.color = color;
            }
        }
        
        public void SetBackgroundColor(Color color)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = color;
            }
        }
        
        private void StartFadeInAnimation()
        {
            if (textComponent != null)
            {
                var color = textComponent.color;
                color.a = 0f;
                textComponent.color = color;
                
                StartCoroutine(FadeInCoroutine());
            }
        }
        
        private System.Collections.IEnumerator FadeInCoroutine()
        {
            float elapsed = 0f;
            var startColor = textComponent.color;
            var targetColor = startColor;
            targetColor.a = 1f;
            
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeInDuration;
                textComponent.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }
            
            textComponent.color = targetColor;
        }
        
        private void StartScaleAnimation()
        {
            transform.localScale = Vector3.zero;
            StartCoroutine(ScaleAnimationCoroutine());
        }
        
        private System.Collections.IEnumerator ScaleAnimationCoroutine()
        {
            float elapsed = 0f;
            Vector3 startScale = Vector3.zero;
            Vector3 targetScale = originalScale * labelScale;
            
            while (elapsed < scaleAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / scaleAnimationDuration;
                t = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease out
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            
            transform.localScale = targetScale;
        }
        
        public void OnPointerClick()
        {
            OnLabelClicked?.Invoke(this);
        }
        
        public void DestroyLabel()
        {
            OnLabelDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
        
        private void OnDestroy()
        {
            OnLabelDestroyed?.Invoke(this);
        }
        
        public string GetLabelText() => translatedText;
        public string GetOriginalText() => labelText;
        public string GetLanguage() => originalLanguage;
        public bool IsVisible() => isVisible;
    }
}

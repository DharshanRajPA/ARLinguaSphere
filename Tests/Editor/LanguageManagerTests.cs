using NUnit.Framework;
using UnityEngine;
using ARLinguaSphere.Core;

namespace Tests.Editor
{
    public class LanguageManagerTests
    {
        private LanguageManager languageManager;
        private GameObject languageManagerObject;
        
        [SetUp]
        public void Setup()
        {
            languageManagerObject = new GameObject("LanguageManager");
            languageManager = languageManagerObject.AddComponent<LanguageManager>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (languageManagerObject != null)
            {
                Object.DestroyImmediate(languageManagerObject);
            }
        }
        
        [Test]
        public void LanguageManager_InitializesCorrectly()
        {
            // Act
            languageManager.Initialize();
            
            // Assert
            Assert.IsNotNull(languageManager);
            Assert.IsTrue(languageManager.currentLanguage == "en");
            Assert.IsTrue(languageManager.fallbackLanguage == "en");
        }
        
        [Test]
        public void LanguageManager_SetCurrentLanguage_UpdatesLanguage()
        {
            // Arrange
            languageManager.Initialize();
            string newLanguage = "es";
            
            // Act
            languageManager.SetCurrentLanguage(newLanguage);
            
            // Assert
            Assert.AreEqual(newLanguage, languageManager.currentLanguage);
        }
        
        [Test]
        public void LanguageManager_GetTranslation_ReturnsKeyWhenNoTranslation()
        {
            // Arrange
            languageManager.Initialize();
            string testKey = "nonexistent_key";
            
            // Act
            string result = languageManager.GetTranslation(testKey);
            
            // Assert
            Assert.AreEqual(testKey, result);
        }
        
        [Test]
        public void LanguageManager_GetTranslation_ReturnsKeyWhenOfflineDisabled()
        {
            // Arrange
            languageManager.Initialize();
            languageManager.enableOnlineTranslation = false;
            string testKey = "test_key";
            
            // Act
            string result = languageManager.GetTranslation(testKey);
            
            // Assert
            Assert.AreEqual(testKey, result);
        }
        
        [Test]
        public void LanguageManager_AddCustomTranslation_AddsTranslation()
        {
            // Arrange
            languageManager.Initialize();
            string key = "custom_key";
            string language = "en";
            string translation = "custom translation";
            
            // Act
            languageManager.AddCustomTranslation(key, language, translation);
            
            // Assert
            string result = languageManager.GetTranslation(key, language);
            Assert.AreEqual(translation, result);
        }
        
        [Test]
        public void LanguageManager_IsLanguageSupported_ReturnsFalseForUnsupportedLanguage()
        {
            // Arrange
            languageManager.Initialize();
            
            // Act
            bool isSupported = languageManager.IsLanguageSupported("unsupported_lang");
            
            // Assert
            Assert.IsFalse(isSupported);
        }
    }
}

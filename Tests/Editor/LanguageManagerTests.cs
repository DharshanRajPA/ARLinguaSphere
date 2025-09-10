using NUnit.Framework;
using UnityEngine;
using ARLinguaSphere.Core;
using System.Collections.Generic;

namespace ARLinguaSphere.Tests
{
    /// <summary>
    /// Unit tests for LanguageManager
    /// </summary>
    public class LanguageManagerTests
    {
        private GameObject testObject;
        private LanguageManager languageManager;
        
        [SetUp]
        public void Setup()
        {
            testObject = new GameObject("TestLanguageManager");
            languageManager = testObject.AddComponent<LanguageManager>();
        }
        
        [TearDown]
        public void Teardown()
        {
            if (testObject != null)
            {
                Object.DestroyImmediate(testObject);
            }
        }
        
        [Test]
        public void LanguageManager_Initialize_SetsDefaultLanguage()
        {
            // Act
            languageManager.Initialize();
            
            // Assert
            Assert.AreEqual("en", languageManager.currentLanguage);
            Assert.AreEqual("en", languageManager.fallbackLanguage);
        }
        
        [Test]
        public void LanguageManager_SetCurrentLanguage_ChangesLanguage()
        {
            // Arrange
            languageManager.Initialize();
            string newLanguage = "es";
            bool languageChangedCalled = false;
            string previousLang = "";
            string newLang = "";
            
            languageManager.OnLanguageChanged += (prev, curr) => 
            {
                languageChangedCalled = true;
                previousLang = prev;
                newLang = curr;
            };
            
            // Act
            languageManager.SetCurrentLanguage(newLanguage);
            
            // Assert
            Assert.AreEqual(newLanguage, languageManager.currentLanguage);
            Assert.IsTrue(languageChangedCalled);
            Assert.AreEqual("en", previousLang);
            Assert.AreEqual(newLanguage, newLang);
        }
        
        [Test]
        public void LanguageManager_SetCurrentLanguage_SameLanguage_NoEvent()
        {
            // Arrange
            languageManager.Initialize();
            bool languageChangedCalled = false;
            
            languageManager.OnLanguageChanged += (prev, curr) => languageChangedCalled = true;
            
            // Act
            languageManager.SetCurrentLanguage("en"); // Same as current
            
            // Assert
            Assert.IsFalse(languageChangedCalled);
        }
        
        [Test]
        public void LanguageManager_GetTranslation_ReturnsCorrectTranslation()
        {
            // Arrange
            languageManager.Initialize();
            
            // Add test translation
            languageManager.AddCustomTranslation("test_key", "en", "English Text");
            languageManager.AddCustomTranslation("test_key", "es", "Texto en Español");
            
            // Act & Assert
            string englishTranslation = languageManager.GetTranslation("test_key", "en");
            string spanishTranslation = languageManager.GetTranslation("test_key", "es");
            
            Assert.AreEqual("English Text", englishTranslation);
            Assert.AreEqual("Texto en Español", spanishTranslation);
        }
        
        [Test]
        public void LanguageManager_GetTranslation_UnknownKey_ReturnsKey()
        {
            // Arrange
            languageManager.Initialize();
            string unknownKey = "unknown_key";
            
            // Act
            string result = languageManager.GetTranslation(unknownKey);
            
            // Assert
            Assert.AreEqual(unknownKey, result);
        }
        
        [Test]
        public void LanguageManager_GetTranslation_NoLanguageSpecified_UsesCurrentLanguage()
        {
            // Arrange
            languageManager.Initialize();
            languageManager.SetCurrentLanguage("es");
            languageManager.AddCustomTranslation("test", "es", "Prueba");
            
            // Act
            string result = languageManager.GetTranslation("test");
            
            // Assert
            Assert.AreEqual("Prueba", result);
        }
        
        [Test]
        public void LanguageManager_IsLanguageSupported_ReturnsCorrectResult()
        {
            // Arrange
            languageManager.Initialize();
            languageManager.AddCustomTranslation("test", "en", "Test");
            languageManager.AddCustomTranslation("test", "es", "Prueba");
            
            // Act & Assert
            Assert.IsTrue(languageManager.IsLanguageSupported("en"));
            Assert.IsTrue(languageManager.IsLanguageSupported("es"));
            Assert.IsFalse(languageManager.IsLanguageSupported("fr"));
        }
        
        [Test]
        public void LanguageManager_GetAvailableLanguages_ReturnsCorrectList()
        {
            // Arrange
            languageManager.Initialize();
            languageManager.AddCustomTranslation("test", "en", "Test");
            languageManager.AddCustomTranslation("test", "es", "Prueba");
            languageManager.AddCustomTranslation("test", "fr", "Test");
            
            // Act
            var availableLanguages = languageManager.GetAvailableLanguages();
            
            // Assert
            Assert.Contains("en", availableLanguages);
            Assert.Contains("es", availableLanguages);
            Assert.Contains("fr", availableLanguages);
            Assert.AreEqual(3, availableLanguages.Count);
        }
        
        [Test]
        public void LanguageManager_AddCustomTranslation_AddsTranslation()
        {
            // Arrange
            languageManager.Initialize();
            
            // Act
            languageManager.AddCustomTranslation("custom_key", "en", "Custom Text");
            
            // Assert
            string result = languageManager.GetTranslation("custom_key", "en");
            Assert.AreEqual("Custom Text", result);
        }
        
        [Test]
        public void LanguageManager_TranslationCompleted_EventTriggered()
        {
            // Arrange
            languageManager.Initialize();
            languageManager.AddCustomTranslation("test", "en", "Test Text");
            
            bool eventTriggered = false;
            string eventKey = "";
            string eventLanguage = "";
            string eventTranslation = "";
            
            languageManager.OnTranslationCompleted += (key, lang, translation) =>
            {
                eventTriggered = true;
                eventKey = key;
                eventLanguage = lang;
                eventTranslation = translation;
            };
            
            // Act
            languageManager.GetTranslation("test", "en");
            
            // Assert
            Assert.IsTrue(eventTriggered);
            Assert.AreEqual("test", eventKey);
            Assert.AreEqual("en", eventLanguage);
            Assert.AreEqual("Test Text", eventTranslation);
        }
    }
}
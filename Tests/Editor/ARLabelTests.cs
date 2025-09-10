using NUnit.Framework;
using UnityEngine;
using ARLinguaSphere.AR;

namespace Tests.Editor
{
    public class ARLabelTests
    {
        private ARLabel arLabel;
        private GameObject arLabelObject;
        
        [SetUp]
        public void Setup()
        {
            arLabelObject = new GameObject("ARLabel");
            arLabel = arLabelObject.AddComponent<ARLabel>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (arLabelObject != null)
            {
                Object.DestroyImmediate(arLabelObject);
            }
        }
        
        [Test]
        public void ARLabel_InitializesCorrectly()
        {
            // Arrange
            string testText = "test object";
            string testLanguage = "en";
            
            // Act
            arLabel.Initialize(testText, testLanguage);
            
            // Assert
            Assert.AreEqual(testText, arLabel.labelText);
            Assert.AreEqual(testLanguage, arLabel.originalLanguage);
            Assert.AreEqual(testText, arLabel.translatedText);
        }
        
        [Test]
        public void ARLabel_UpdateText_UpdatesTranslatedText()
        {
            // Arrange
            arLabel.Initialize("original", "en");
            string newText = "new text";
            
            // Act
            arLabel.UpdateText(newText);
            
            // Assert
            Assert.AreEqual(newText, arLabel.translatedText);
        }
        
        [Test]
        public void ARLabel_UpdateText_WithLanguage_UpdatesBoth()
        {
            // Arrange
            arLabel.Initialize("original", "en");
            string newText = "nuevo texto";
            string newLanguage = "es";
            
            // Act
            arLabel.UpdateText(newText, newLanguage);
            
            // Assert
            Assert.AreEqual(newText, arLabel.translatedText);
            Assert.AreEqual(newLanguage, arLabel.originalLanguage);
        }
        
        [Test]
        public void ARLabel_SetScale_UpdatesScale()
        {
            // Arrange
            arLabel.Initialize("test", "en");
            float newScale = 2f;
            
            // Act
            arLabel.SetScale(newScale);
            
            // Assert
            Assert.AreEqual(newScale, arLabel.labelScale);
        }
        
        [Test]
        public void ARLabel_SetVisible_UpdatesVisibility()
        {
            // Arrange
            arLabel.Initialize("test", "en");
            
            // Act
            arLabel.SetVisible(false);
            
            // Assert
            Assert.IsFalse(arLabel.IsVisible());
        }
        
        [Test]
        public void ARLabel_GetLabelText_ReturnsTranslatedText()
        {
            // Arrange
            string testText = "test object";
            arLabel.Initialize(testText, "en");
            
            // Act
            string result = arLabel.GetLabelText();
            
            // Assert
            Assert.AreEqual(testText, result);
        }
        
        [Test]
        public void ARLabel_GetOriginalText_ReturnsOriginalText()
        {
            // Arrange
            string originalText = "original";
            arLabel.Initialize(originalText, "en");
            arLabel.UpdateText("translated");
            
            // Act
            string result = arLabel.GetOriginalText();
            
            // Assert
            Assert.AreEqual(originalText, result);
        }
        
        [Test]
        public void ARLabel_GetLanguage_ReturnsLanguage()
        {
            // Arrange
            string language = "es";
            arLabel.Initialize("test", language);
            
            // Act
            string result = arLabel.GetLanguage();
            
            // Assert
            Assert.AreEqual(language, result);
        }
    }
}

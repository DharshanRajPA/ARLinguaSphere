using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ARLinguaSphere.AR;
using System.Collections;

namespace ARLinguaSphere.Tests
{
    /// <summary>
    /// Unit tests for ARLabel component
    /// </summary>
    public class ARLabelTests
    {
        private GameObject testObject;
        private ARLabel arLabel;
        
        [SetUp]
        public void Setup()
        {
            testObject = new GameObject("TestARLabel");
            arLabel = testObject.AddComponent<ARLabel>();
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
        public void ARLabel_Initialize_SetsPropertiesCorrectly()
        {
            // Arrange
            string testText = "Test Label";
            string testLanguage = "en";
            
            // Act
            arLabel.Initialize(testText, testLanguage);
            
            // Assert
            Assert.AreEqual(testText, arLabel.GetLabelText());
            Assert.AreEqual(testText, arLabel.GetOriginalText());
            Assert.AreEqual(testLanguage, arLabel.GetLanguage());
        }
        
        [Test]
        public void ARLabel_UpdateText_ChangesDisplayedText()
        {
            // Arrange
            arLabel.Initialize("Original", "en");
            string newText = "Updated Text";
            string newLanguage = "es";
            
            // Act
            arLabel.UpdateText(newText, newLanguage);
            
            // Assert
            Assert.AreEqual(newText, arLabel.GetLabelText());
            Assert.AreEqual(newLanguage, arLabel.GetLanguage());
        }
        
        [Test]
        public void ARLabel_SetVisible_ChangesVisibility()
        {
            // Arrange
            arLabel.Initialize("Test", "en");
            
            // Act & Assert
            arLabel.SetVisible(false);
            Assert.IsFalse(arLabel.IsVisible());
            Assert.IsFalse(testObject.activeInHierarchy);
            
            arLabel.SetVisible(true);
            Assert.IsTrue(arLabel.IsVisible());
            Assert.IsTrue(testObject.activeInHierarchy);
        }
        
        [Test]
        public void ARLabel_SetScale_ChangesTransformScale()
        {
            // Arrange
            arLabel.Initialize("Test", "en");
            float testScale = 2.0f;
            Vector3 originalScale = testObject.transform.localScale;
            
            // Act
            arLabel.SetScale(testScale);
            
            // Assert
            Vector3 expectedScale = originalScale * testScale;
            Assert.AreEqual(expectedScale, testObject.transform.localScale);
        }
        
        [Test]
        public void ARLabel_EventsTriggeredCorrectly()
        {
            // Arrange
            bool labelClickedCalled = false;
            bool labelDestroyedCalled = false;
            
            arLabel.Initialize("Test", "en");
            arLabel.OnLabelClicked += (label) => labelClickedCalled = true;
            arLabel.OnLabelDestroyed += (label) => labelDestroyedCalled = true;
            
            // Act
            arLabel.OnPointerClick();
            arLabel.DestroyLabel();
            
            // Assert
            Assert.IsTrue(labelClickedCalled);
            Assert.IsTrue(labelDestroyedCalled);
        }
        
        [UnityTest]
        public IEnumerator ARLabel_AnimationCompletes()
        {
            // Arrange
            arLabel.enableFadeIn = true;
            arLabel.enableScaleAnimation = true;
            arLabel.fadeInDuration = 0.1f;
            arLabel.scaleAnimationDuration = 0.1f;
            
            // Act
            arLabel.Initialize("Test", "en");
            
            // Wait for animations to complete
            yield return new WaitForSeconds(0.2f);
            
            // Assert - animations should have completed without errors
            Assert.IsTrue(arLabel.IsVisible());
        }
    }
}
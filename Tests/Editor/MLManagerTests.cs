using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ARLinguaSphere.ML;
using System.Collections.Generic;

namespace Tests.Editor
{
    public class MLManagerTests
    {
        private MLManager mlManager;
        private GameObject mlManagerObject;
        
        [SetUp]
        public void Setup()
        {
            mlManagerObject = new GameObject("MLManager");
            mlManager = mlManagerObject.AddComponent<MLManager>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (mlManagerObject != null)
            {
                Object.DestroyImmediate(mlManagerObject);
            }
        }
        
        [Test]
        public void MLManager_InitializesCorrectly()
        {
            // Act
            mlManager.Initialize();
            
            // Assert
            Assert.IsNotNull(mlManager);
            Assert.IsTrue(mlManager.confidenceThreshold == 0.5f);
            Assert.IsTrue(mlManager.maxDetections == 10);
        }
        
        [Test]
        public void MLManager_SetConfidenceThreshold_UpdatesValue()
        {
            // Arrange
            mlManager.Initialize();
            float newThreshold = 0.7f;
            
            // Act
            mlManager.SetConfidenceThreshold(newThreshold);
            
            // Assert
            Assert.AreEqual(newThreshold, mlManager.confidenceThreshold);
        }
        
        [Test]
        public void MLManager_SetMaxDetections_UpdatesValue()
        {
            // Arrange
            mlManager.Initialize();
            int newMax = 20;
            
            // Act
            mlManager.SetMaxDetections(newMax);
            
            // Assert
            Assert.AreEqual(newMax, mlManager.maxDetections);
        }
        
        [Test]
        public void MLManager_ProcessFrame_DoesNotThrow()
        {
            // Arrange
            mlManager.Initialize();
            var testTexture = new Texture2D(100, 100);
            
            // Act & Assert
            Assert.DoesNotThrow(() => mlManager.ProcessFrame(testTexture));
            
            // Cleanup
            Object.DestroyImmediate(testTexture);
        }
        
        [Test]
        public void MLManager_IsProcessing_ReturnsFalseInitially()
        {
            // Arrange
            mlManager.Initialize();
            
            // Act & Assert
            Assert.IsFalse(mlManager.IsProcessing);
        }
        
        [Test]
        public void MLManager_QueuedFrames_ReturnsZeroInitially()
        {
            // Arrange
            mlManager.Initialize();
            
            // Act & Assert
            Assert.AreEqual(0, mlManager.QueuedFrames);
        }
    }
}

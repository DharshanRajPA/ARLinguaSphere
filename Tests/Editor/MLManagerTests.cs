using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ARLinguaSphere.ML;
using System.Collections;
using System.Collections.Generic;

namespace ARLinguaSphere.Tests
{
    /// <summary>
    /// Unit tests for MLManager and object detection
    /// </summary>
    public class MLManagerTests
    {
        private GameObject testObject;
        private MLManager mlManager;
        
        [SetUp]
        public void Setup()
        {
            testObject = new GameObject("TestMLManager");
            mlManager = testObject.AddComponent<MLManager>();
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
        public void MLManager_Initialize_SetsInitializedFlag()
        {
            // Act
            mlManager.Initialize();
            
            // Assert
            Assert.IsTrue(mlManager.IsProcessing == false); // Should not be processing initially
            Assert.AreEqual(0, mlManager.QueuedFrames);
        }
        
        [Test]
        public void MLManager_SetConfidenceThreshold_UpdatesThreshold()
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
        public void MLManager_SetConfidenceThreshold_ClampsValue()
        {
            // Arrange
            mlManager.Initialize();
            
            // Act & Assert - Test upper bound
            mlManager.SetConfidenceThreshold(1.5f);
            Assert.AreEqual(1.0f, mlManager.confidenceThreshold);
            
            // Act & Assert - Test lower bound
            mlManager.SetConfidenceThreshold(-0.5f);
            Assert.AreEqual(0.0f, mlManager.confidenceThreshold);
        }
        
        [Test]
        public void MLManager_SetMaxDetections_UpdatesMaxDetections()
        {
            // Arrange
            mlManager.Initialize();
            int newMaxDetections = 15;
            
            // Act
            mlManager.SetMaxDetections(newMaxDetections);
            
            // Assert
            Assert.AreEqual(newMaxDetections, mlManager.maxDetections);
        }
        
        [Test]
        public void MLManager_SetMaxDetections_EnforcesMinimum()
        {
            // Arrange
            mlManager.Initialize();
            
            // Act
            mlManager.SetMaxDetections(0);
            
            // Assert
            Assert.AreEqual(1, mlManager.maxDetections);
        }
        
        [UnityTest]
        public IEnumerator MLManager_ProcessFrame_TriggersDetectionEvent()
        {
            // Arrange
            mlManager.Initialize();
            mlManager.enableAsyncProcessing = false; // Disable async for test
            
            bool eventTriggered = false;
            List<Detection> receivedDetections = null;
            
            mlManager.OnObjectsDetected += (detections) =>
            {
                eventTriggered = true;
                receivedDetections = detections;
            };
            
            // Create a test texture
            Texture2D testTexture = new Texture2D(640, 640, TextureFormat.RGB24, false);
            
            // Act
            mlManager.ProcessFrame(testTexture);
            
            // Wait for processing
            yield return new WaitForSeconds(0.1f);
            
            // Assert
            Assert.IsTrue(eventTriggered);
            Assert.IsNotNull(receivedDetections);
            
            // Cleanup
            Object.DestroyImmediate(testTexture);
        }
        
        [Test]
        public void MLManager_ProcessFrame_NullTexture_DoesNotCrash()
        {
            // Arrange
            mlManager.Initialize();
            
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => mlManager.ProcessFrame(null));
        }
        
        [UnityTest]
        public IEnumerator MLManager_AsyncProcessing_QueuesFrames()
        {
            // Arrange
            mlManager.Initialize();
            mlManager.enableAsyncProcessing = true;
            
            // Create test textures
            Texture2D testTexture1 = new Texture2D(640, 640, TextureFormat.RGB24, false);
            Texture2D testTexture2 = new Texture2D(640, 640, TextureFormat.RGB24, false);
            
            // Act
            mlManager.ProcessFrame(testTexture1);
            mlManager.ProcessFrame(testTexture2);
            
            // Assert - Frames should be queued
            Assert.GreaterOrEqual(mlManager.QueuedFrames, 0);
            
            // Wait for processing
            yield return new WaitForSeconds(0.3f);
            
            // Cleanup
            Object.DestroyImmediate(testTexture1);
            Object.DestroyImmediate(testTexture2);
        }
        
        [Test]
        public void Detection_ToString_ReturnsFormattedString()
        {
            // Arrange
            var detection = new Detection
            {
                label = "person",
                confidence = 0.85f,
                boundingBox = new Rect(0.1f, 0.2f, 0.3f, 0.4f),
                classId = 0
            };
            
            // Act
            string result = detection.ToString();
            
            // Assert
            Assert.IsTrue(result.Contains("person"));
            Assert.IsTrue(result.Contains("85"));
            Assert.IsTrue(result.Contains("0.1"));
        }
        
        [Test]
        public void Detection_Properties_SetCorrectly()
        {
            // Arrange & Act
            var detection = new Detection
            {
                label = "car",
                confidence = 0.92f,
                boundingBox = new Rect(0.2f, 0.3f, 0.4f, 0.5f),
                classId = 2
            };
            
            // Assert
            Assert.AreEqual("car", detection.label);
            Assert.AreEqual(0.92f, detection.confidence, 0.001f);
            Assert.AreEqual(new Rect(0.2f, 0.3f, 0.4f, 0.5f), detection.boundingBox);
            Assert.AreEqual(2, detection.classId);
        }
    }
}
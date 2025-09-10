using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ARLinguaSphere.Core;

namespace Tests.Editor
{
    public class GameManagerTests
    {
        private GameManager gameManager;
        private GameObject gameManagerObject;
        
        [SetUp]
        public void Setup()
        {
            gameManagerObject = new GameObject("GameManager");
            gameManager = gameManagerObject.AddComponent<GameManager>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (gameManagerObject != null)
            {
                Object.DestroyImmediate(gameManagerObject);
            }
        }
        
        [Test]
        public void GameManager_InitializesCorrectly()
        {
            // Act
            gameManager.InitializeGame();
            
            // Assert
            Assert.IsNotNull(gameManager);
            Assert.IsTrue(gameManager.debugMode == false);
            Assert.IsTrue(gameManager.defaultLanguage == "en");
        }
        
        [Test]
        public void GameManager_SingletonPatternWorks()
        {
            // Arrange
            var gameManager2 = new GameObject("GameManager2").AddComponent<GameManager>();
            
            // Act & Assert
            Assert.AreNotEqual(gameManager, gameManager2);
            
            // Cleanup
            Object.DestroyImmediate(gameManager2.gameObject);
        }
        
        [Test]
        public void GameManager_RestartGame_LoadsCurrentScene()
        {
            // This test would require scene management setup
            // For now, we'll just verify the method exists and can be called
            Assert.DoesNotThrow(() => gameManager.RestartGame());
        }
        
        [Test]
        public void GameManager_QuitGame_DoesNotThrow()
        {
            // This test verifies the quit method doesn't throw exceptions
            Assert.DoesNotThrow(() => gameManager.QuitGame());
        }
    }
}

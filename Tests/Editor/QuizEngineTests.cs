using NUnit.Framework;
using UnityEngine;
using ARLinguaSphere.Analytics;
using System.Collections.Generic;

namespace ARLinguaSphere.Tests
{
    /// <summary>
    /// Unit tests for QuizEngine
    /// </summary>
    public class QuizEngineTests
    {
        private GameObject testObject;
        private GameObject analyticsObject;
        private QuizEngine quizEngine;
        private AnalyticsManager analyticsManager;
        
        [SetUp]
        public void Setup()
        {
            // Create test objects
            testObject = new GameObject("TestQuizEngine");
            analyticsObject = new GameObject("TestAnalyticsManager");
            
            // Add components
            quizEngine = testObject.AddComponent<QuizEngine>();
            analyticsManager = analyticsObject.AddComponent<AnalyticsManager>();
            
            // Initialize
            analyticsManager.Initialize();
            quizEngine.Initialize(analyticsManager);
        }
        
        [TearDown]
        public void Teardown()
        {
            if (testObject != null)
            {
                Object.DestroyImmediate(testObject);
            }
            if (analyticsObject != null)
            {
                Object.DestroyImmediate(analyticsObject);
            }
        }
        
        [Test]
        public void QuizEngine_Initialize_SetsAnalyticsManager()
        {
            // Assert
            Assert.AreEqual(analyticsManager, quizEngine.analyticsManager);
        }
        
        [Test]
        public void QuizEngine_GetNextQuizSet_ReturnsEmptyWhenNoAnalytics()
        {
            // Arrange
            var emptyQuizEngine = testObject.AddComponent<QuizEngine>();
            emptyQuizEngine.Initialize(null);
            
            // Act
            var quizSet = emptyQuizEngine.GetNextQuizSet();
            
            // Assert
            Assert.IsNotNull(quizSet);
            Assert.AreEqual(0, quizSet.Count);
        }
        
        [Test]
        public void QuizEngine_GetNextQuizSet_ReturnsWordsFromAnalytics()
        {
            // Arrange - Add some interaction data
            analyticsManager.LogInteraction("anchor1", "apple", InteractionType.LabelPlaced, true);
            analyticsManager.LogInteraction("anchor2", "car", InteractionType.LabelPlaced, false);
            analyticsManager.LogInteraction("anchor3", "dog", InteractionType.LabelPlaced, true);
            
            // Act
            var quizSet = quizEngine.GetNextQuizSet(2);
            
            // Assert
            Assert.IsNotNull(quizSet);
            Assert.LessOrEqual(quizSet.Count, 2);
        }
        
        [Test]
        public void QuizEngine_RecordAnswer_LogsInteraction()
        {
            // Arrange
            string testWord = "testword";
            bool wasCorrect = true;
            float responseTime = 2.5f;
            
            // Act
            quizEngine.RecordAnswer(testWord, wasCorrect, responseTime);
            
            // Assert - Verify the interaction was logged
            var wordStats = analyticsManager.GetWordStats(testWord);
            Assert.IsNotNull(wordStats);
            Assert.AreEqual(1, wordStats.totalInteractions);
            Assert.AreEqual(1, wordStats.successfulInteractions);
        }
        
        [Test]
        public void QuizEngine_RecordAnswer_IncorrectAnswer_LogsFailure()
        {
            // Arrange
            string testWord = "difficultword";
            bool wasCorrect = false;
            float responseTime = 5.0f;
            
            // Act
            quizEngine.RecordAnswer(testWord, wasCorrect, responseTime);
            
            // Assert
            var wordStats = analyticsManager.GetWordStats(testWord);
            Assert.IsNotNull(wordStats);
            Assert.AreEqual(1, wordStats.totalInteractions);
            Assert.AreEqual(0, wordStats.successfulInteractions);
        }
        
        [Test]
        public void QuizEngine_RecordAnswer_EmptyWord_DoesNotCrash()
        {
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => quizEngine.RecordAnswer("", true));
            Assert.DoesNotThrow(() => quizEngine.RecordAnswer(null, true));
        }
        
        [Test]
        public void QuizEngine_RecordAnswer_UpdatesResponseTime()
        {
            // Arrange
            string testWord = "timedword";
            float firstResponseTime = 3.0f;
            float secondResponseTime = 1.5f;
            
            // Act
            quizEngine.RecordAnswer(testWord, true, firstResponseTime);
            quizEngine.RecordAnswer(testWord, true, secondResponseTime);
            
            // Assert
            var wordStats = analyticsManager.GetWordStats(testWord);
            Assert.IsNotNull(wordStats);
            Assert.AreEqual(2, wordStats.totalInteractions);
            Assert.AreEqual(2, wordStats.successfulInteractions);
            
            // Average response time should be (3.0 + 1.5) / 2 = 2.25
            Assert.AreEqual(2.25f, wordStats.averageResponseTime, 0.01f);
        }
        
        [Test]
        public void QuizEngine_IntegrationTest_AdaptiveDifficulty()
        {
            // Arrange - Create a word with multiple failed attempts
            string difficultWord = "challengingword";
            
            // Act - Record multiple incorrect answers
            for (int i = 0; i < 6; i++)
            {
                quizEngine.RecordAnswer(difficultWord, false, 5.0f);
            }
            
            // Assert - Difficulty should have increased
            var wordStats = analyticsManager.GetWordStats(difficultWord);
            Assert.IsNotNull(wordStats);
            Assert.Greater(wordStats.difficultyLevel, 1.0f);
        }
        
        [Test]
        public void QuizEngine_GetNextQuizSet_PrioritizesDifficultWords()
        {
            // Arrange - Create words with different success rates
            
            // Easy word - high success rate
            for (int i = 0; i < 10; i++)
            {
                quizEngine.RecordAnswer("easyword", true, 1.0f);
            }
            
            // Difficult word - low success rate
            for (int i = 0; i < 10; i++)
            {
                quizEngine.RecordAnswer("hardword", false, 5.0f);
            }
            
            // Act
            var quizSet = quizEngine.GetNextQuizSet(5);
            
            // Assert - Hard word should be prioritized in quiz
            Assert.IsNotNull(quizSet);
            Assert.Contains("hardword", quizSet);
        }
    }
}
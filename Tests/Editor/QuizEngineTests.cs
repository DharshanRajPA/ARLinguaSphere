using NUnit.Framework;
using UnityEngine;
using ARLinguaSphere.Analytics;

namespace Tests.Editor
{
	public class QuizEngineTests
	{
		[Test]
		public void QuizEngine_InitializesAndReturnsList()
		{
			var analyticsGO = new GameObject("AnalyticsManager");
			var analytics = analyticsGO.AddComponent<AnalyticsManager>();
			analytics.Initialize();
			var quizGO = new GameObject("QuizEngine");
			var quiz = quizGO.AddComponent<QuizEngine>();
			quiz.Initialize(analytics);
			var list = quiz.GetNextQuizSet(3);
			Assert.IsNotNull(list);
		}
	}
}



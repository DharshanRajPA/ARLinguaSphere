using UnityEngine;
using System.Collections.Generic;

namespace ARLinguaSphere.Analytics
{
	/// <summary>
	/// Simple adaptive quiz engine backed by AnalyticsManager word stats
	/// </summary>
	public class QuizEngine : MonoBehaviour
	{
		public AnalyticsManager analyticsManager;
		
		public void Initialize(AnalyticsManager manager)
		{
			analyticsManager = manager;
		}
		
		public List<string> GetNextQuizSet(int count = 5)
		{
			if (analyticsManager == null)
			{
				return new List<string>();
			}
			return analyticsManager.GetWordsForQuiz(count);
		}
	}
}



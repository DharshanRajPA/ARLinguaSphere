using UnityEngine;

namespace ARLinguaSphere.Voice
{
	/// <summary>
	/// Receives callbacks from Android SpeechPlugin via UnitySendMessage and forwards to VoiceManager.
	/// </summary>
	public class SpeechCallbackReceiver : MonoBehaviour
	{
		public VoiceManager voiceManager;

		public void OnSpeechBegin(string _)
		{
			voiceManager?.OnSpeechStarted?.Invoke();
		}

		public void OnSpeechEnd(string _)
		{
			voiceManager?.OnSpeechEnded?.Invoke();
		}

		public void OnSpeechError(string errorCode)
		{
			voiceManager?.OnSpeechError?.Invoke(errorCode);
		}

		public void OnSpeechResult(string text)
		{
			voiceManager?.OnSpeechRecognized?.Invoke(text);
		}

		public void OnSpeechPartial(string text)
		{
			// Optional: handle partial results if needed in future
		}

		public void OnTTSStart(string _)
		{
			voiceManager?.OnTTSStarted?.Invoke();
		}

		public void OnTTSEnd(string _)
		{
			voiceManager?.OnTTSEnded?.Invoke();
		}
	}
}



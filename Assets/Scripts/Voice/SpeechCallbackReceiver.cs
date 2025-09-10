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
			voiceManager?.NotifySpeechStarted();
		}

		public void OnSpeechEnd(string _)
		{
			voiceManager?.NotifySpeechEnded();
		}

		public void OnSpeechError(string errorCode)
		{
			voiceManager?.NotifySpeechError(errorCode);
		}

		public void OnSpeechResult(string text)
		{
			voiceManager?.NotifySpeechRecognized(text);
		}

		public void OnSpeechPartial(string text)
		{
			// Optional: handle partial results if needed in future
		}

		public void OnTTSStart(string _)
		{
			voiceManager?.NotifyTTSStarted();
		}

		public void OnTTSEnd(string _)
		{
			voiceManager?.NotifyTTSEnded();
		}
	}
}



using UnityEngine;

namespace ARLinguaSphere.Voice
{
	/// <summary>
	/// Android Java bridge for SpeechRecognizer and TextToSpeech.
	/// In Editor and non-Android, methods no-op.
	/// </summary>
	public class AndroidSpeechBridge
	{
		private AndroidJavaObject unityActivity;
		private AndroidJavaObject speechPlugin;
		
		public AndroidSpeechBridge()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			speechPlugin = new AndroidJavaObject("com.arlinguasphere.voice.SpeechPlugin", unityActivity);
			#endif
		}
		
		public void StartListening(string locale)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			speechPlugin?.Call("startListening", locale);
			#endif
		}
		
		public void StopListening()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			speechPlugin?.Call("stopListening");
			#endif
		}
		
		public void Speak(string text, string locale, float rate, float pitch, float volume)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			speechPlugin?.Call("speak", text, locale, rate, pitch, volume);
			#endif
		}
	}
}



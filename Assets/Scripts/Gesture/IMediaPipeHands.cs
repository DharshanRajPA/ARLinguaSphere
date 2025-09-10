using UnityEngine;
using System;

namespace ARLinguaSphere.Gesture
{
	public interface IMediaPipeHands
	{
		bool IsInitialized { get; }
		void Initialize(int maxHands = 1, bool useGPU = true);
		void ProcessFrame(Texture2D frameTexture);
		event Action<HandLandmarks> OnHandLandmarks;
	}
	
	[Serializable]
	public class HandLandmarks
	{
		public Vector3[] keypoints; // 21 keypoints
		public float confidence;
		public bool isRight;
	}
}



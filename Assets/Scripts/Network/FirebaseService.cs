using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using ARLinguaSphere.Core.ThirdParty;

namespace ARLinguaSphere.Network
{
	/// <summary>
	/// Thin abstraction for Firebase Realtime Database/Firestore operations.
	/// This is a placeholder that you can back with a concrete SDK or REST calls.
	/// </summary>
	public class FirebaseService : MonoBehaviour
	{
		[Header("Firebase Settings")]
		public string databaseUrl = "https://arlinguasphere.firebaseio.com";
		public bool useEmulator = false;
		public string emulatorHost = "127.0.0.1";
		public int emulatorPort = 9000;
		public float pollIntervalSeconds = 0.5f;
		
		public bool IsInitialized { get; private set; }
		private readonly Dictionary<string, HashSet<string>> roomSeenAnchors = new Dictionary<string, HashSet<string>>();
		private readonly Dictionary<string, Coroutine> roomPollCoroutines = new Dictionary<string, Coroutine>();
		private string baseUrl;
		
		public void Initialize()
		{
			baseUrl = BuildBaseUrl();
			IsInitialized = true;
			Debug.Log($"FirebaseService: Initialized (REST) baseUrl={baseUrl}");
		}
		
		public void SetRoomAnchor(string roomId, AnchorData anchorData, Action<bool> onComplete = null)
		{
			if (!IsInitialized) { onComplete?.Invoke(false); return; }
			StartCoroutine(PutJson($"{baseUrl}/rooms/{roomId}/anchors/{anchorData.id}.json", anchorData, onComplete));
		}
		
		public void ListenRoomAnchors(string roomId, Action<AnchorData> onAnchor)
		{
			if (!IsInitialized) return;
			if (!roomSeenAnchors.ContainsKey(roomId)) roomSeenAnchors[roomId] = new HashSet<string>();
			if (roomPollCoroutines.ContainsKey(roomId)) return;
			roomPollCoroutines[roomId] = StartCoroutine(PollAnchors(roomId, onAnchor));
		}
		
		public void RemoveRoomAnchor(string roomId, string anchorId, Action<bool> onComplete = null)
		{
			if (!IsInitialized) { onComplete?.Invoke(false); return; }
			StartCoroutine(Delete($"{baseUrl}/rooms/{roomId}/anchors/{anchorId}.json", onComplete));
		}

		private string BuildBaseUrl()
		{
			if (useEmulator)
			{
				return $"http://{emulatorHost}:{emulatorPort}";
			}
			return databaseUrl.TrimEnd('/');
		}

		private IEnumerator PutJson(string url, AnchorData data, Action<bool> onComplete)
		{
			string json = SerializeAnchor(data);
			var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT);
			byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
			request.uploadHandler = new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			bool ok = request.result == UnityWebRequest.Result.Success;
			if (!ok) Debug.LogWarning($"FirebaseService: PUT failed {request.responseCode} {request.error}");
			onComplete?.Invoke(ok);
		}

		private IEnumerator Delete(string url, Action<bool> onComplete)
		{
			var request = UnityWebRequest.Delete(url);
			yield return request.SendWebRequest();
			bool ok = request.result == UnityWebRequest.Result.Success;
			if (!ok) Debug.LogWarning($"FirebaseService: DELETE failed {request.responseCode} {request.error}");
			onComplete?.Invoke(ok);
		}

		private IEnumerator PollAnchors(string roomId, Action<AnchorData> onAnchor)
		{
			while (true)
			{
				string url = $"{baseUrl}/rooms/{roomId}/anchors.json";
				var request = UnityWebRequest.Get(url);
				yield return request.SendWebRequest();
				if (request.result == UnityWebRequest.Result.Success)
				{
					var json = request.downloadHandler.text;
					var parsed = MiniJSON.Deserialize(json) as Dictionary<string, object>;
					if (parsed != null)
					{
						var seen = roomSeenAnchors[roomId];
						foreach (var kv in parsed)
						{
							string id = kv.Key;
							if (!seen.Contains(id))
							{
								var anchor = DeserializeAnchor(id, kv.Value as Dictionary<string, object>);
								if (anchor != null)
								{
									seen.Add(id);
									onAnchor?.Invoke(anchor);
								}
							}
						}
					}
				}
				yield return new WaitForSeconds(pollIntervalSeconds);
			}
		}

		private string SerializeAnchor(AnchorData a)
		{
			// Minimal manual JSON to avoid extra deps
			return "{" +
				$"\"id\":\"{a.id}\"," +
				$"\"labelKey\":\"{a.labelKey}\"," +
				$"\"creatorId\":\"{a.creatorId}\"," +
				$"\"timestamp\":{a.timestamp}," +
				$"\"position\":{{\"x\":{a.position.x},\"y\":{a.position.y},\"z\":{a.position.z}}}," +
				$"\"rotation\":{{\"x\":{a.rotation.x},\"y\":{a.rotation.y},\"z\":{a.rotation.z},\"w\":{a.rotation.w}}}" +
			"}";
		}

		private AnchorData DeserializeAnchor(string id, Dictionary<string, object> map)
		{
			if (map == null) return null;
			try
			{
				var a = new AnchorData();
				a.id = id;
				a.labelKey = map.ContainsKey("labelKey") ? map["labelKey"].ToString() : null;
				a.creatorId = map.ContainsKey("creatorId") ? map["creatorId"].ToString() : null;
				if (map.ContainsKey("timestamp")) a.timestamp = Convert.ToInt64(map["timestamp"]);
				if (map.ContainsKey("position"))
				{
					var p = map["position"] as Dictionary<string, object>;
					a.position = new Vector3(
						Convert.ToSingle(p["x"]),
						Convert.ToSingle(p["y"]),
						Convert.ToSingle(p["z"]) );
				}
				if (map.ContainsKey("rotation"))
				{
					var r = map["rotation"] as Dictionary<string, object>;
					a.rotation = new Quaternion(
						Convert.ToSingle(r["x"]),
						Convert.ToSingle(r["y"]),
						Convert.ToSingle(r["z"]),
						Convert.ToSingle(r["w"]) );
				}
				return a;
			}
			catch (Exception e)
			{
				Debug.LogWarning($"FirebaseService: Failed to parse anchor: {e.Message}");
				return null;
			}
		}
	}
}



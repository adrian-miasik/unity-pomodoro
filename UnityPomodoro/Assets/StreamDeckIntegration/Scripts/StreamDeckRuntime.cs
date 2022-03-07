using UnityEngine;

namespace F10.StreamDeckIntegration {
	/// <summary>
	/// Keeps <see cref="StreamDeckSocket"/> updated when the integration is running on a build / not in the editor.
	/// </summary>
	[DisallowMultipleComponent]
	public class StreamDeckRuntime : MonoBehaviour {
		[SerializeField]
		[Tooltip("Keep the object loaded at all times")]
		// ReSharper disable once IdentifierTypo
		private bool _dontDestroyOnLoad = true;

		private void Awake() {
			if (_dontDestroyOnLoad) {
				DontDestroyOnLoad(gameObject);
			}

			if (Application.isEditor) return;

			StreamDeckSocket.Connect();
		}

		private void Update() {
			if (Application.isEditor) return;

			StreamDeckSocket.OnUpdate();
		}

		private void OnDestroy() {
			if (Application.isEditor) return;

			StreamDeckSocket.Disconnect();
		}
	}
}
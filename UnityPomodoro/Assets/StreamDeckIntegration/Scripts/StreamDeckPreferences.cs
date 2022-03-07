using UnityEngine;

namespace F10.StreamDeckIntegration {
	internal class StreamDeckPreferences : ScriptableObject {

		[SerializeField]
		private Log.LogLevel _logging = Log.DefaultLogLevel;

		private void OnValidate() {
			ApplySettings();
		}

		internal void ApplySettings() {
			Log.Level = _logging;
		}

	}
}
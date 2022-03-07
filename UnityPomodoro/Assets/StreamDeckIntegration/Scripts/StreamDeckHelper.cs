#if UNITY_EDITOR
using UnityEditor;

namespace F10.StreamDeckIntegration {
	[InitializeOnLoad]
	internal static class StreamDeckHelper {

		static StreamDeckHelper() {
			EditorApplication.playModeStateChanged += PlayModeStateChanged;
			EditorApplication.pauseStateChanged += PauseModeStateChanged;
		}

		internal static void SwitchPlayMode() {
			if (EditorApplication.isPlaying) {
#if UNITY_2019_1_OR_NEWER
				EditorApplication.ExitPlaymode();
#else
				EditorApplication.isPlaying = false;
#endif
			} else {
#if UNITY_2019_1_OR_NEWER
				EditorApplication.EnterPlaymode();
#else
				EditorApplication.isPlaying = true;
#endif
			}
		}

		internal static void SwitchPauseMode() {
			EditorApplication.isPaused = !EditorApplication.isPaused;
		}

		internal static void ExecuteMenu(string menuPath) {
			EditorApplication.ExecuteMenuItem(menuPath);
		}

		private static void PlayModeStateChanged(PlayModeStateChange playMode) {
			int state;

			switch (playMode) {
				case PlayModeStateChange.ExitingEditMode:
					state = 1;
					break;
				case PlayModeStateChange.ExitingPlayMode:
					state = 0;
					break;
				default:
					return;
			}

			var json = $"{{\"event\":\"playModeStateChanged\",\"payload\":{{\"state\":{state}}}}}";
			StreamDeckSocket.Send(json);
		}

		private static void PauseModeStateChanged(PauseState pauseState) {
			int state;

			switch (pauseState) {
				case PauseState.Paused:
					state = 1;
					break;
				case PauseState.Unpaused:
					state = 0;
					break;
				default:
					return;
			}

			var json = $"{{\"event\":\"pauseModeStateChanged\",\"payload\":{{\"state\":{state}}}}}";
			StreamDeckSocket.Send(json);
		}

	}
}
#endif
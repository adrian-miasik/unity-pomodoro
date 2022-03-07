namespace F10.StreamDeckIntegration {
	internal static class Log {

		private const string Tag = "<color=#265eff><b>StreamDeck</b></color>";

		internal enum LogLevel {

			Critical = 0,
			Essential = 1,
			All = 2

		}

		internal const LogLevel DefaultLogLevel = LogLevel.Essential;

		internal static LogLevel Level { private get; set; } = DefaultLogLevel;

		private static bool CanLog(LogLevel level) {
			return Level >= level;
		}

		internal static void Debug(string message) {
			if (!CanLog(LogLevel.All)) return;

			UnityEngine.Debug.Log(GetComposedMessage(message));
		}

		internal static void Info(string message) {
			if (!CanLog(LogLevel.Essential)) return;

			UnityEngine.Debug.Log(GetComposedMessage(message));
		}

		internal static void Warning(string message) {
			if (!CanLog(LogLevel.Essential)) return;

			UnityEngine.Debug.LogWarning(GetComposedMessage(message));
		}

		internal static void Error(string message) {
			if (!CanLog(LogLevel.Critical)) return;

			UnityEngine.Debug.LogError(GetComposedMessage(message));
		}

		private static string GetComposedMessage(string message) {
			return $"{Tag} - {message}";
		}

	}
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using F10.StreamDeckIntegration.Json;
using JetBrains.Annotations;

namespace F10.StreamDeckIntegration {
	internal static class StreamDeckReflection {

		[CanBeNull]
		private static string GetGroup(IReadOnlyDictionary<string, JsonData> data) {
			var isGroup = data["settings"].AsData["group"].AsBool;

			var groupId = isGroup ? data["settings"].AsData["group-id"].AsString : StreamDeck.MainGroup;
			if (StreamDeck.Groups.ContainsKey(groupId)) return groupId;

			Log.Warning($"No group ID \"{groupId}\" registered");
			return null;
		}

		[CanBeNull]
		private static StreamDeckButtonData GetButtonData(string groupId, string memberId) {
			var buttonData = StreamDeck.Groups[groupId].FirstOrDefault(x => x.Id == memberId);
			if (buttonData != null) return buttonData;

			// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
			if (groupId == StreamDeck.MainGroup) {
				Log.Warning($"No member ID \"{memberId}\" found");
			} else {
				Log.Warning($"No member ID \"{memberId}\" found in \"{groupId}\"");
			}

			return null;
		}

		internal static void InvokeMethod(IReadOnlyDictionary<string, JsonData> data) {
			var groupId = GetGroup(data);
			if (groupId == null) return;

			var memberId = data["settings"].AsData["id"].AsString;
			var buttonData = GetButtonData(groupId, memberId);
			if (buttonData == null) return;

			var info = buttonData.Member as MethodInfo;
			if (info == null) {
				Log.Error($"The ID \"{memberId}\" is not a method!");
				return;
			}

			// Check if method has parameters
			var parameters = info.GetParameters();
			var hasParam = data["settings"].AsData["param"].AsBool;

			if (parameters.Length <= 0 || !hasParam) {
				info.Invoke(buttonData.Target, null);
			} else {
				var paramValue = data["settings"].AsData["value"];
				var paramType = data["settings"].AsData["type"].AsString;

				info.Invoke(buttonData.Target, new[] {paramValue.AsCastedObject(paramType)});
			}
		}

		internal static void SetFieldOrProperty(IReadOnlyDictionary<string, JsonData> data) {
			var groupId = GetGroup(data);
			if (groupId == null) return;

			var memberId = data["settings"].AsData["id"].AsString;
			var buttonData = GetButtonData(groupId, memberId);
			if (buttonData == null) return;

			var infoProperty = buttonData.Member as PropertyInfo;
			var infoField = buttonData.Member as FieldInfo;
			if (infoProperty == null && infoField == null) {
				Log.Error($"The ID \"{memberId}\" is not a property nor a field!");
				return;
			}

			var value = data["settings"].AsData["value"];
			var type = data["settings"].AsData["type"].AsString;

			infoProperty?.SetValue(buttonData.Target, value.AsCastedObject(type));
			infoField?.SetValue(buttonData.Target, value.AsCastedObject(type));
		}

	}
}
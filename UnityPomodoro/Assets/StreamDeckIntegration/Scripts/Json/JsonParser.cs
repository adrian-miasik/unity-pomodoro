using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace F10.StreamDeckIntegration.Json {
	/// <summary>
	/// Simplified JSON parser to exclusively deal with StreamDeck JSON websocket communication messages.
	/// </summary>
	internal static class JsonParser {

		[CanBeNull]
		internal static Dictionary<string, JsonData> Parse(string json) {
			if (!json.StartsWith("{") || !json.EndsWith("}")) return null;

			var dict = new Dictionary<string, JsonData>();

			// Remove start and end {}
			json = json.Substring(1);
			json = json.Remove(json.Length - 1);

			// Remove ""
			json = json.Replace("\"", string.Empty);

			// Separate by , (taking care of nested {})
			var lastIndex = 0;
			var inNestedSection = false;
			for (int i = 0; i < json.Length; i++) {
				if (i >= json.Length - 1 || json[i] == ',' && !inNestedSection) {
					// Separate key-value :
					var section = json.Substring(lastIndex, (i + 1) - lastIndex);

					// Remove start and end ,
					if (section.StartsWith(",")) {
						section = section.Substring(1);
					}

					if (section.EndsWith(",")) {
						section = section.Remove(section.Length - 1);
					}

					var keyValue = section.Split(new[] {':'}, 2).Select(x => x.Trim()).ToArray();

					// Has nested json?
					var jsonData = keyValue[1].Contains("{")
						? new JsonData(Parse(keyValue[1]))
						: new JsonData(keyValue[1]);

					dict.Add(keyValue[0], jsonData);
					lastIndex = i;
					continue;
				}

				switch (json[i]) {
					case '{':
						inNestedSection = true;
						continue;
					case '}':
						inNestedSection = false;
						continue;
				}
			}

			return dict;
		}

	}
}
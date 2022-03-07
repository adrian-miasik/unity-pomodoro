using System;
using System.Collections.Generic;

namespace F10.StreamDeckIntegration.Json {
	/// <summary>
	/// Data schema used by the <see cref="JsonParser"/> to serialize StreamDeck messages.
	/// </summary>
	internal class JsonData {

		private readonly string _value;

		internal string AsString => _value;

		internal bool AsBool => _value.ToLowerInvariant() == "true";

		internal int AsInt => Convert.ToInt32(_value);

		internal float AsFloat => Convert.ToSingle(_value);

		private readonly Dictionary<string, JsonData> _nestedValue;

		internal Dictionary<string, JsonData> AsData => _nestedValue;

		internal JsonData(string value) {
			_value = value;
		}

		internal JsonData(Dictionary<string, JsonData> nestedValue) {
			_nestedValue = nestedValue;
		}

		internal object AsCastedObject(string typedType) {
			switch (typedType.ToLowerInvariant()) {
				case "int":
					return AsInt;
				case "float":
					return AsFloat;
				case "bool":
					return AsBool;
				case "string":
					return AsString;
			}

			throw new ArgumentOutOfRangeException(nameof(typedType), $"No type found or supported, named {typedType}");
		}

	}
}
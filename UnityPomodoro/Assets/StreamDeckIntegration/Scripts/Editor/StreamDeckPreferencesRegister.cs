using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace F10.StreamDeckIntegration.Editor {
	/// <summary>
	/// Unity settings provider for the Stream Deck Integration.
	/// </summary>
	internal static class StreamDeckPreferencesRegister {

		private const string PreferencesPath = "Assets/Editor";
		private const string PreferencesFile = "StreamDeckPreferences.asset";

		static StreamDeckPreferencesRegister() {
			var settings = GetPreferences();
			if (settings == null) return;

			settings.ApplySettings();
		}

		[SettingsProvider]
		public static SettingsProvider CreateProvider() {
			var provider = new SettingsProvider("Preferences/Stream Deck", SettingsScope.User) {
				label = "Stream Deck",
				guiHandler = searchContext => {
					var settings = new SerializedObject(GetOrCreatePreferences());
					settings.Update();

					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(settings.FindProperty("_logging"),
						new GUIContent("Logging level \u24D8", "Logging level. Essential is recommended."));

					settings.ApplyModifiedProperties();
				},

				keywords = new HashSet<string>(new[] {"StreamDeck", "Elgato", "Action", "F10"})
			};

			return provider;
		}

		[CanBeNull]
		private static StreamDeckPreferences GetPreferences() {
			var allAssets = AssetDatabase.FindAssets("t:StreamDeckPreferences");

			if (allAssets.Length <= 0) return null;

			var path = AssetDatabase.GUIDToAssetPath(allAssets[0]);
			return AssetDatabase.LoadAssetAtPath<StreamDeckPreferences>(path);
		}

		private static StreamDeckPreferences GetOrCreatePreferences() {
			var settings = GetPreferences();
			if (settings != null) return settings;

			settings = ScriptableObject.CreateInstance<StreamDeckPreferences>();

			if (!AssetDatabase.IsValidFolder(PreferencesPath)) {
				AssetDatabase.CreateFolder("Assets", "Editor");
			}

			AssetDatabase.CreateAsset(settings, $"{PreferencesPath}/{PreferencesFile}");
			AssetDatabase.SaveAssets();
			
			Log.Info("A new StreamDeckPreferences file has been created inside Assets/Editor\nYou can move this file anywhere in the project");

			return settings;
		}

	}
}
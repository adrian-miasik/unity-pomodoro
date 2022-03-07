using JetBrains.Annotations;
using UnityEngine;

namespace F10.StreamDeckIntegration {
	/// <summary>
	/// Handles changes to Stream Deck actions, like title and icon control.
	/// </summary>
	public static class StreamDeckSettings {

		/// <summary>
		/// Set a new title on the Stream Deck action linked to the passed member.
		/// <br/>
		/// If the target action has a custom title (manually set title on the Stream Deck software) changes won't be visible.
		/// </summary>
		/// <param name="title">Text to set as the action's title.</param>
		/// <param name="id">Member ID of the targeted action.</param>
		/// <param name="groupId">Optional.<br/>Group ID of the targeted action. Defaults to null for actions without groups.</param>
		/// <param name="immediate">Optional.<br/>Allows the setting to be sent immediately, instead of being added to the queue. Defaults to false.</param>
		public static void SetButtonTitle([NotNull] string title, [NotNull] string id, string groupId = null, bool immediate = false) {
			var buttonData = GetButtonData(id, groupId);
			if (buttonData == null) return;

			var json = $"{{\"event\":\"setTitle\",\"payload\":{{\"title\":\"{title}\",\"group-id\":\"{groupId ?? string.Empty}\",\"id\":\"{id}\"}}}}";
			StreamDeckSocket.Send(json, immediate);
		}

		/// <summary>
		/// Set a new image / icon on the Stream Deck action linked to the passed member.
		/// <br/>
		/// If the target action has a custom icon (manually set icon on the Stream Deck software) changes won't be visible.
		/// </summary>
		/// <param name="image">Texture2D to set as the action's image / action.</param>
		/// <param name="id">Member ID of the targeted action.</param>
		/// <param name="groupId">Optional.<br/>Group ID of the targeted action. Defaults to null for actions without groups.</param>
		/// <param name="immediate">Optional.<br/>Allows the setting to be sent immediately, instead of being added to the queue. Defaults to false.</param>
		public static void SetButtonImage([NotNull] Texture2D image, [NotNull] string id, string groupId = null, bool immediate = false) {
			var buttonData = GetButtonData(id, groupId);
			if (buttonData == null) return;

			var base64 = System.Convert.ToBase64String(image.EncodeToPNG());
			var encodedImage = $"data:image/png;base64,{base64}";

			var json = $"{{\"event\":\"setImage\",\"payload\":{{\"image\":\"{encodedImage}\",\"group-id\":\"{groupId ?? string.Empty}\",\"id\":\"{id}\"}}}}";
			StreamDeckSocket.Send(json, immediate);
		}

		[CanBeNull]
		private static StreamDeckButtonData GetButtonData(string id, string groupId) {
			var buttonData = StreamDeck.FindButtonById(groupId, id);

			if (buttonData != null) return buttonData;

			Log.Error($"There isn't any registered button with group ID {groupId} and / or {id}!");
			return null;
		}

	}
}
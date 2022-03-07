using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using F10.StreamDeckIntegration.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace F10.StreamDeckIntegration {
	/// <summary>
	/// Handles adding and removing members that can be executed by Stream Deck.
	/// </summary>
	[ExecuteAlways]
	public static class StreamDeck {
		internal const string MainGroup = "StreamDeck_Main";

		internal static readonly Dictionary<string, List<StreamDeckButtonData>> Groups = new Dictionary<string, List<StreamDeckButtonData>>();
		
		static StreamDeck() {
			if (!Groups.ContainsKey(MainGroup)) {
				Groups.Add(MainGroup, new List<StreamDeckButtonData>());
			}
		}

		/// <summary>
		/// Adds the given type as a static member to the list of available <see cref="StreamDeckButtonAttribute"/> and <see cref="StreamDeckGroupAttribute"/>.
		/// </summary>
		/// <param name="type">Type of the static member to add.</param>
		public static void AddStatic(Type type) {
			Internal_Add(type, null);
		}

		/// <summary>
		/// Adds the given object member instance to the list of available <see cref="StreamDeckButtonAttribute"/> and <see cref="StreamDeckGroupAttribute"/>.
		/// </summary>
		/// <param name="obj">Boxed member to add.</param>
		public static void Add(object obj) {
			var type = obj.GetType();
			Internal_Add(type, obj);
		}

		/// <summary>
		/// Removes the given object member instance to the list of available <see cref="StreamDeckButtonAttribute"/> and <see cref="StreamDeckGroupAttribute"/>.
		/// </summary>
		/// <param name="obj">Boxed member to remove.</param>
		public static void Remove(object obj) {
			var type = obj.GetType();

			if (Attribute.IsDefined(type, typeof(StreamDeckGroupAttribute), false)) {
				var attribute = type.GetCustomAttribute(typeof(StreamDeckGroupAttribute), false) as StreamDeckGroupAttribute;

				// ReSharper disable once PossibleNullReferenceException
				var id = attribute.Id ?? type.Name;
				Groups.Remove(id);
			}

			var members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			foreach (var member in members) {
				if (!Attribute.IsDefined(member, typeof(StreamDeckButtonAttribute), false)) continue;

				var attribute = member.GetCustomAttribute(typeof(StreamDeckButtonAttribute), false) as StreamDeckButtonAttribute;
				// ReSharper disable once PossibleNullReferenceException
				var id = attribute.Id ?? member.Name;
				var index = Groups[MainGroup].FindIndex(x => x.Id == id);

				if (index >= 0) {
					Groups[MainGroup].RemoveAt(index);
				}
			}
		}

		[CanBeNull]
		internal static StreamDeckButtonData FindButtonById([CanBeNull]string groupId, [NotNull]string memberId) {
			if (groupId == null || string.IsNullOrWhiteSpace(groupId)) {
				groupId = MainGroup;
			}
			
			if (!Groups.TryGetValue(groupId, out var group)) {
				Log.Debug($"Unable to find the group with ID {groupId}. The instance may not have been registered in time.");
				return null;
			}

			StreamDeckButtonData buttonData = group.FirstOrDefault(button => button.Id == memberId);
			if (buttonData != null) return buttonData;
			
			Log.Debug($"Unable to find the button with ID {memberId} inside the group with ID {groupId}. The instance may not have been registered in time.");
			return null;
		}
		
		private static void Internal_Add(Type type, object obj) {
			var members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

			// StreamDeckGroup
			if (Attribute.IsDefined(type, typeof(StreamDeckGroupAttribute), false)) {
				var attribute = type.GetCustomAttribute(typeof(StreamDeckGroupAttribute), false) as StreamDeckGroupAttribute;

				// ReSharper disable once PossibleNullReferenceException
				var id = attribute.Id ?? type.Name;

				if (Groups.ContainsKey(id)) {
					Log.Warning($"The ID \"{id}\" is already defined and added to active groups!\nEach StreamDeckGroup must have a unique ID. Custom ID's can be added as part of the attribute.\nThis group will be ignored.");
					return;
				}

				foreach (var member in members) {
					var buttonData = new StreamDeckButtonData(obj, member);

					if (!Groups.ContainsKey(id)) {
						Groups.Add(id, new List<StreamDeckButtonData> {buttonData});
					} else {
						Groups[id].Add(buttonData);
					}
				}

				return;
			}

			// StreamDeckButton
			foreach (var member in members) {
				if (!Attribute.IsDefined(member, typeof(StreamDeckButtonAttribute), false)) continue;

				var attribute = member.GetCustomAttribute(typeof(StreamDeckButtonAttribute), false) as StreamDeckButtonAttribute;
				// ReSharper disable once PossibleNullReferenceException
				var id = attribute.Id ?? member.Name;

				if (Groups[MainGroup].Any(x => x.Id == id)) {
					Log.Warning($"The ID \"{id}\" is already defined and added to active buttons!\nEach StreamDeckButton must have a unique ID. Custom ID's can be added as part of the attribute.\nThis button will be ignored.");
					return;
				}

				var buttonData = new StreamDeckButtonData(obj, member, id);
				Groups[MainGroup].Add(buttonData);
			}
		}
	}
}
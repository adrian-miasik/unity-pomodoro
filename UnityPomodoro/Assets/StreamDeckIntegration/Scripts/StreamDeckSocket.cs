using System;
using System.Collections.Generic;
using F10.StreamDeckIntegration.Json;
using JetBrains.Annotations;
using WebSocketSharp;

namespace F10.StreamDeckIntegration {
	/// <summary>
	/// Manages WebSocket connections and messages with the Stream Deck.
	/// </summary>
	public static class StreamDeckSocket {
		private const int WebSocketPort = 2245;

		private static readonly Queue<IReadOnlyDictionary<string, JsonData>> _messagesQueue = new Queue<IReadOnlyDictionary<string, JsonData>>();

		private static readonly Queue<string> _sendQueue = new Queue<string>();

		private static readonly object _queueLock = new object();

		private static WebSocket _socket = null;

		private static event Action UpdateEvents;

		/// <summary>
		/// Is the WebSocket connected and alive.
		/// </summary>
		[PublicAPI]
		public static bool IsConnected => _socket != null && _socket.IsAlive;
		
		/// <summary>
		/// Try to connect or re-connect to any available Stream Deck.
		/// </summary>
		[PublicAPI]
		public static void Connect() {
			if (IsConnected) {
				Log.Info("Disconnecting current link...");

				_socket.OnClose += (sender, args) => ConnectAsync();
				_socket.CloseAsync();
			} else {
				ConnectAsync();
			}
		}
		
		/// <summary>
		/// Disconnect from any connected Stream Deck.
		/// </summary>
		[PublicAPI]
		public static void Disconnect() {
			if (IsConnected) return;
			
			Log.Info("Disconnecting...");
			_socket.CloseAsync();
		}

		/// <summary>
		/// Handle one queued received and / or sent message.
		/// </summary>
		[PublicAPI]
		public static void OnUpdate() {
			UpdateEvents?.Invoke();
		}

		private static void ConnectAsync() {
			_socket = new WebSocket($"ws://127.0.0.1:{WebSocketPort}");

			_socket.OnOpen += OnOpen;
			_socket.OnMessage += OnMessage;
			_socket.OnClose += OnClose;

			_socket.ConnectAsync();
		}

		private static void OnOpen(object sender, EventArgs args) {
			UpdateEvents -= HandleMessages;
			UpdateEvents += HandleMessages;

			UpdateEvents -= HandleSending;
			UpdateEvents += HandleSending;

			Log.Info("Connected");
		}

		private static void OnMessage(object sender, MessageEventArgs e) {
			Log.Debug($"Received: {e.Data}");

			var data = JsonParser.Parse(e.Data);
			if (data == null) return;

			QueueMessage(data);
		}

		private static void OnClose(object sender, CloseEventArgs closeEventArgs) {
			UpdateEvents -= HandleSending;

			Log.Info($"Disconnected - {closeEventArgs.Reason}");
		}

		internal static void Send(string message, bool immediate = false) {
			if (!IsConnected) {
				Connect();
			}

			Log.Debug($"Queued message to send: {message}");

			if (immediate) {
				SendToPlugin(message);
			} else {
				_sendQueue.Enqueue(message);
			}
		}

		private static void QueueMessage(IReadOnlyDictionary<string, JsonData> data) {
			lock (_queueLock) {
				_messagesQueue.Enqueue(data);
			}
		}

		private static void HandleSending() {
			if (!IsConnected) return;
			if (_sendQueue.Count <= 0) return;

			SendToPlugin(_sendQueue.Dequeue());
		}

		private static void SendToPlugin(string message) {
			Log.Debug($"Sent: {message}");
			_socket.Send(message);
		}

		private static void HandleMessages() {
			lock (_queueLock) {
				if (_messagesQueue.Count <= 0) return;

				var data = _messagesQueue.Dequeue();

				Log.Debug($"Handling action: {data["action"].AsString} with state: {data["state"].AsInt}");

				var action = data["action"].AsString;

				switch (action) {
					case "invoke-method":
						StreamDeckReflection.InvokeMethod(data);
						break;
					case "set-field-property":
						StreamDeckReflection.SetFieldOrProperty(data);
						break;
					case "play-mode":
#if UNITY_EDITOR
						StreamDeckHelper.SwitchPlayMode();
#endif
						break;
					case "pause-mode":
#if UNITY_EDITOR
						StreamDeckHelper.SwitchPauseMode();
#endif
						break;
					case "execute-menu":
#if UNITY_EDITOR
						var path = data["settings"].AsData["path"].AsString;
						StreamDeckHelper.ExecuteMenu(path);
#endif
						break;
				}
			}
		}
	}
}
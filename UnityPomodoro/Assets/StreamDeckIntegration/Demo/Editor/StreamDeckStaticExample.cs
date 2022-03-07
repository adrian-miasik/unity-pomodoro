using F10.StreamDeckIntegration.Attributes;
using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
namespace F10.StreamDeckIntegration.Demo.Editor {
	/// <summary>
	/// Example class using <see cref="StreamDeckGroupAttribute"/> attributes and static support.
	/// </summary>
	[ExecuteAlways]
	[InitializeOnLoad]
	[StreamDeckGroup]
	public static class StreamDeckStaticExample {

		/*
		 * Action: Set Field / Property
		 * Member ID: _editorField
		 * Type: String
		 */
		private static string _editorField = null;

		/*
		 * Action: Set Field / Property
		 * Member ID: EditorProperty
		 * Type: Bool
		 */
		private static bool EditorProperty { get; set; }

		static StreamDeckStaticExample() {
			// Registers this class as a StreamDeck enabled class
			// This is a static class, for instanced classes use Add()
			StreamDeck.AddStatic(typeof(StreamDeckStaticExample));
		}

		/*
		 * Action: Invoke Method
		 * Member ID: ExampleStaticMethod
		 *
		 * No need for custom attribute, StreamDeckGroup will reference all fields, properties and methods 
		 */
		public static void ExampleStaticMethod() {
			Debug.Log("Example Static Method");
		}

		/*
		 * Action: Invoke Method
		 * Member ID: ExampleStaticFields
		 *
		 * No need for custom attribute, StreamDeckGroup will reference all fields, properties and methods 
		 */
		public static void ExampleStaticFields() {
			Debug.Log($"Field: {_editorField}, Property: {EditorProperty}");
		}

	}
}
using System;
using System.Globalization;
using F10.StreamDeckIntegration.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable RedundantDefaultMemberInitializer

namespace F10.StreamDeckIntegration.Demo {
	/// <summary>
	/// Example class using <see cref="StreamDeckButtonAttribute"/> attributes and runtime support.
	/// </summary>
	public class StreamDeckExample : MonoBehaviour {

		[Header("Check the source code for commented API!")]
		/*
	     * Action: Set Field / Property
	     * Member ID: _field
	     * Type: Int
	     */
		[StreamDeckButton]
		public int _field = 0;

		/*
		 * Action: Set Field / Property
		 * Member ID: Property
		 * Type: Float
		 */
		[StreamDeckButton]
		private float Property { get; set; }

		private void OnEnable() {
			// Registers this class as a StreamDeck enabled class
			StreamDeck.Add(this);
		}

		private void OnDisable() {
			// Removes this class as a StreamDeck enabled class
			StreamDeck.Remove(this);
		}

		/*
		 * Action: Invoke Method
		 * Member ID: ExampleMethod
		 *
		 * Public method, specific ID
		 */
		[StreamDeckButton("ExampleMethod")]
		public void ExampleMethod() {
			Debug.Log("Example Method");
		}

		/*
		 * Action: Invoke Method
	     * Member ID: ExampleMethodPrivate
	     *
	     * Private method, ID not defined
	     */
		[StreamDeckButton]
		private void ExampleMethodPrivate() {
			Debug.Log("Private Example Method");
		}

		/*
		 * Action: Invoke Method
		 * Member ID: CustomID
		 *
		 * Different ID then method name
		 */
		[StreamDeckButton("CustomID")]
		private void ExampleMethodCustomId() {
			Debug.Log("Example Method With Custom ID");
		}

		/*
	     * Action: Invoke Method
	     * Member ID: ExampleMethodWithParam
		 * Parameter: String
	     *
	     * Method with custom parameter
	     */
		[StreamDeckButton]
		private void ExampleMethodWithParam(string param) {
			Debug.Log($"Example Method With Param: {param}");
		}

		/*
		 * Action: Invoke Method
		 * Member ID: ExampleMethodLogFields
		 *
		 * Print the supported field and property to the console
		 */
		[StreamDeckButton]
		private void ExampleMethodLogFields() {
			Debug.Log($"Field: {_field}, Property: {Property}");
		}

		/*
		 * Action: Invoke Method
		 * Member ID: Visual
		 *
		 * Visually shows the main camera changing background color
		 * Perfect for testing a build without logging support
		 */
		[StreamDeckButton("Visual")]
		private void ExampleMethodVisual() {
			var mainCamera = Camera.main;

			if (mainCamera == null) {
				Debug.LogWarning("No camera found! Is a camera available in the scene?");
				return;
			}

			mainCamera.backgroundColor = mainCamera.backgroundColor == Color.black ? Color.white : Color.black;
		}

		/*
		 * Action: Invoke Method
		 * Member ID: ExampleMethodSetTitle
		 *
		 * Sets the title of the Stream Deck Action when clicked to a random numeric value
		 * IMPORTANT: The Stream Deck won't show any changes if a title is manually set on the software. Make sure the action has no title.
		 */
		[StreamDeckButton]
		private void ExampleMethodSetTitle() {
			var newTitle = DateTime.UtcNow.Millisecond.ToString(CultureInfo.InvariantCulture);
			StreamDeckSettings.SetButtonTitle(newTitle, "ExampleMethodSetTitle");
		}

		/*
		 * Action: Invoke Method
		 * Member ID: ExampleMethodSetImage
		 *
		 * Sets the image / icon of the Stream Deck Action when clicked to a random 4 color image.
		 * IMPORTANT: The Stream Deck won't show any changes if an image / icon is manually set on the software. Make sure the action has the default image / icon.
		 */
		[StreamDeckButton]
		private void ExampleMethodSetImage() {
			var randomColors = new Color[4];
			for (int i = 0; i < randomColors.Length; i++) {
				randomColors[i] = Random.ColorHSV(0, 1);
			}

			var randomImage = new Texture2D(2, 2);
			randomImage.SetPixels(randomColors);

			StreamDeckSettings.SetButtonImage(randomImage, "ExampleMethodSetImage");
		}

	}
}
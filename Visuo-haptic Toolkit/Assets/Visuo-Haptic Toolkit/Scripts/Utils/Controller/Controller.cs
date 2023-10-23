using UnityEngine;
using UnityEngine.InputSystem;

namespace BG.Visualisation {
	public class Controller : MonoBehaviour {

		public float speed = 0.5f;

		Camera cam;
		Vector3 previousMousePosition;
		bool wasFocused;

        void Awake() => cam = Camera.main;

        void Update() {
			// KEYBOARD
			// Get the horizontal and vertical axis.
			// By default they are mapped to the arrow keys.
			// The value is in the range -1 to 1
			float translationVertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float translationHorizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

			// Move translation along the object's z-axis
			transform.Translate(translationHorizontal, 0, translationVertical);


			// MOUSE
			Mouse mouse = Mouse.current;
			if (mouse.leftButton.isPressed && wasFocused) {
				Vector3 mousePosition = mouse.position.ReadValue();
				transform.Translate(cam.ScreenToWorldPoint(mousePosition) - cam.ScreenToWorldPoint(previousMousePosition));
			}
			previousMousePosition = mouse.position.ReadValue();
			wasFocused = Application.isFocused;
		}
	}
}

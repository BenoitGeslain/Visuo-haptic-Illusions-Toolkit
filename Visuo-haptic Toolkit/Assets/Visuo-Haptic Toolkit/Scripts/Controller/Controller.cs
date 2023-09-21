using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour {

	public float speed = 0.5f;

	Camera camera;
	Vector3 previousMousePosition;
	Transform clickedObject;

	void Awake() {
		camera = Camera.main;
	}

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
		// If left clic mouse button is pressed, apply the mouse displacement converted from camera space to world space to the transform
		if (mouse.leftButton.isPressed) {
			Vector3 mousePosition = mouse.position.ReadValue();

			transform.position += camera.ScreenToWorldPoint(mousePosition) - camera.ScreenToWorldPoint(previousMousePosition);
		}
		previousMousePosition = mouse.position.ReadValue();
    }
}
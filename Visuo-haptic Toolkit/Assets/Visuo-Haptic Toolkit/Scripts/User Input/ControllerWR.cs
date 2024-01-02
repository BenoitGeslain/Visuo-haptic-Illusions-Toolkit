// using UnityEngine;
// using UnityEngine.InputSystem;

// namespace VHToolkit.Visualisation {
// 	public class ControllerWR : MonoBehaviour {

// 		[SerializeField] private float speed = 0.5f, sensitivity = 1f;

// 		bool wasFocused;

//         void Update() {
// 			// KEYBOARD
// 			// Get the horizontal and vertical axis.
// 			// By default they are mapped to the arrow keys.
// 			float translationForward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
//             float translationSideways = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

// 			// Move translation along the object's z-axis
// 			this.transform.Translate(translationSideways, 0, translationForward);

// 			// MOUSE
// 			if(Input.GetMouseButton(0) && wasFocused) {
// 				transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * sensitivity, -Input.GetAxis("Mouse X") * sensitivity, 0));
// 				transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
// 			}
// 			wasFocused = Application.isFocused;
// 		}
// 	}
// }

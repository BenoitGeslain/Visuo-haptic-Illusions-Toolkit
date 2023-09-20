using UnityEngine;

public class Controller : MonoBehaviour {

	public float speed = 0.5f;

	void Update() {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translationVertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float translationHorizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        // Move translation along the object's z-axis
        transform.Translate(translationHorizontal, 0, translationVertical);
    }
}
using UnityEngine;

public class OriginCollider : MonoBehaviour {

    [SerializeField] private CubesRedirection script;
    private void OnTriggerEnter(Collider other) {
        script.TouchedOrigin();
    }
}
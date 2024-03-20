using UnityEngine;

public class TargetCollider : MonoBehaviour {

    [SerializeField] private CubesRedirection script;
    public bool enableTrigger;
    
    private void OnTriggerEnter(Collider other) {
        if (enableTrigger)
            script.TouchedTarget();
        Debug.Log("entered trigger");
    }
}
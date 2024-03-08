using UnityEngine;
using VHToolkit.Redirection.WorldRedirection;

public class CorridorRepresentation : MonoBehaviour {
	[SerializeField] private Transform physicalCorridor, virtualCorridor;

	private WorldRedirection script;

	private void Start() => script = GetComponent<WorldRedirection>();

	private void Update() {
		virtualCorridor.SetPositionAndRotation(physicalCorridor.position, physicalCorridor.rotation);
		virtualCorridor.position = physicalCorridor.position + script.scene.GetHeadToHeadVector();
		virtualCorridor.RotateAround(script.scene.physicalHead.position, Vector3.up, -script.scene.HeadToHeadRedirection.eulerAngles.y);
	}
}
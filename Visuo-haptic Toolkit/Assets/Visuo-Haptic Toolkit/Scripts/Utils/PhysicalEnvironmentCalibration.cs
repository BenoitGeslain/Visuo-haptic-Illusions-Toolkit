using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using VHToolkit;
using VHToolkit.Redirection;

public class PhysicalEnvironmentCalibration : MonoBehaviour {

    [SerializeField] private float eps = 0.01f;
    [SerializeField] private Transform user;
    [SerializeField] private List<Vector3> bounds;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        List<Vector3> obs = new();

        var dir = (bounds.First() - bounds.Last()).normalized;
        int n = (int)(Vector3.Distance(bounds.Last(), bounds.First()) / eps);
        for (int j = 0; j < n; j++) {
            obs.Add(bounds.Last() + (j + 0.5f) * eps * dir);
        }
        for (int i = 1; i < bounds.Count; i++) {
            dir = (bounds[i] - bounds[i - 1]).normalized;
            n = (int) (Vector3.Distance(bounds[i - 1], bounds[i]) / eps);
            for (int j = 0; j < n; j++) {
                obs.Add(bounds[i - 1] + (j + 0.5f) * eps * dir);
            }
        }
        Debug.Log(obs.Count);
        obs.ForEach(o => Debug.Log(o));

        Debug.DrawRay(user.position, ComputeGradient(user, obs).normalized);
    }

    private void OnDrawGizmos() {
        for (int i = 1; i < bounds.Count; i++) {
            Gizmos.DrawLine(bounds[i - 1], bounds[i]);
        }
        Gizmos.DrawLine(bounds.Last(), bounds.First());
    }

    private Vector3 ComputeGradient(Transform user, List<Vector3> dividedObstacles) => Vector3.ProjectOnPlane(MathTools.Gradient3(
                                        MathTools.RepulsivePotential3D(dividedObstacles),
                                        MathTools.ProjectToHorizontalPlaneV3(user.position)
                                    ), Vector3.up);
}

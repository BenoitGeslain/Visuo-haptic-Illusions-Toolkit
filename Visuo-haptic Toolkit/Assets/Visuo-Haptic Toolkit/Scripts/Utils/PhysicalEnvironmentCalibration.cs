using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using VHToolkit;

public class PhysicalEnvironmentCalibration : MonoBehaviour {

    [SerializeField] private float eps = 0.01f;
    [SerializeField] private Transform user;
    [SerializeField] private List<Vector3> bounds;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {


        var dir = (bounds.First() - bounds.Last()).normalized;
        int n = (int)(Vector3.Distance(bounds.Last(), bounds.First()) / eps);
        List<Vector3> obs = new();
        foreach (var(a, b) in bounds.Zip(bounds.Skip(1).Append(bounds.First())))
        {
            dir = (b - a).normalized;
            n = (int) (Vector3.Distance(a, b) / eps);
            obs.AddRange(Enumerable.Range(0, n - 1).Select(j => a + (j + 0.5f) * eps * dir));
        }
        Debug.Log(obs.Count);
        obs.ForEach(o => Debug.Log(o));

        Debug.DrawRay(user.position, ComputeGradient(user, obs).normalized);
    }

    private void OnDrawGizmos() {
        foreach (var (a, b) in bounds.Zip(bounds.Skip(1).Append(bounds.First()))) {
            Gizmos.DrawLine(a, b);
        }
    }

    private Vector3 ComputeGradient(Transform user, List<Vector3> dividedObstacles) => Vector3.ProjectOnPlane(MathTools.Gradient3(
                                        MathTools.RepulsivePotential3D(dividedObstacles),
                                        MathTools.ProjectToHorizontalPlaneV3(user.position)
                                    ), Vector3.up);
}

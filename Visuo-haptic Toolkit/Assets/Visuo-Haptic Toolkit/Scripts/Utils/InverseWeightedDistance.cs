using UnityEngine;

namespace VHToolkit {
    public static class InverseWeightedDistance {
        public static Vector3 Interpolate(Vector3[] x, Vector3[] target, float p, Vector3 position) {
            Debug.Assert(x.Length == target.Length);
            Debug.Assert(x.Length >= 3);
            Vector3 numerator = Vector3.zero;
            float denumerator = 0f;
            foreach (var (origin, targ) in x.Zip(target)) {
                var distance = (origin - position).sqrMagnitude;
                if (Mathf.Approximately(distance, 0)) {
                    return targ;
                }
                else {
                    var weight = Mathf.Pow(distance, -p / 2);
                    numerator += weight * targ;
                    denumerator += weight;
                }
            }
            return numerator / denumerator;
        }
    }
}

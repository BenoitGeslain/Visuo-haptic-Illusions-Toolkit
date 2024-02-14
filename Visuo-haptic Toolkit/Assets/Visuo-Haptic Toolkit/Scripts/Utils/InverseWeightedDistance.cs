using System.Linq;
using UnityEngine;
using VHToolkit;

namespace VHToolkit
{
    public static class InverseWeightedDistance
    {
   public static Vector3 Interpolate(Vector3[] x, Vector3[] target, float p, Vector3 position) {
            var weights = new float[x.Length];
            int idx = 0;
            foreach (var (origin, targ) in x.Zip(target)) {
                var d = Vector3.Distance(origin, position);
                if (Mathf.Approximately(d, 0)) {
                    return targ;
                }
                else {
                    weights[idx++] =  Mathf.Pow(d, -p);
                }
            }
            return target.Zip(weights, (t, w) => w * t).Aggregate(Vector3.zero, (a, b) => a + b) / weights.Sum();
        }
    }
}

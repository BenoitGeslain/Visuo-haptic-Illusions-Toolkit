using UnityEngine;

namespace VHToolkit {
	public static class InverseWeightedDistance {
		public static Vector3 Interpolate(Vector3[] x, Vector3[] target, float p, Vector3 position) {
			/* Inverse distance weighting computes a weighted average of the displacement over the 
			given refereence points. Each reference point at distance d from the current position receives
			a weight proportional to (1/d)^p, where p is a parameter which tends to penalize more distant
			points.
			*/
			Debug.Assert(x.Length == target.Length);
			Debug.Assert(x.Length >= 3);
			Vector3 numerator = Vector3.zero;
			float denumerator = 0f;
			foreach (var (origin, targ) in x.Zip(target)) {
				var sqrDistance = (origin - position).sqrMagnitude;
				if (Mathf.Approximately(sqrDistance, 0)) {
					return targ;
				}
				else {
					var weight = Mathf.Pow(sqrDistance, -p / 2);
					numerator += weight * targ;
					denumerator += weight;
				}
			}
			return numerator / denumerator;
		}
	}
}

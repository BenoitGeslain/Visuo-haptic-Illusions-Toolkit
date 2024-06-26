using System.Linq;
using UnityEngine;
using VHToolkit;
using System;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;


namespace VHToolkit {
	public static class ThinPlateSpline {
		// The notation follows [Eberly 96, Thin-Plate Splines] at https://www.geometrictools.com/Documentation/ThinPlateSplines.pdf.
		static float GreenFunction2D(float distance) => (distance == 0f) ? 0f : distance * distance * MathF.Log(distance);


		/// <summary>
		// Returns f:R^2 -> R satisfying f(s) = t for each pair (s, t) in x.Zip(y).
		// Among all such functions, f minimizes the energy  (∑ₖ |f(x[k]) - y[k]|²) + λ ∬ ∑ᵢⱼ (∂ᵢ∂ⱼf)².
		/// </summary>

		public static Func<Vector2, float> SolveSmoothed(Vector2[] x, float[] y, float lambda) {
			Debug.Assert(x.Length == y.Length);
			Debug.Assert(lambda >= 0);
			var Mat = Matrix<float>.Build;

			var yAsVec = Vector<float>.Build.DenseOfArray(y);
			var N = Mat.DenseOfRowArrays(x.Select(xx => new[] { 1, xx.x, xx.y }));
			// Compute (M + lambda * Id)^-1 only once
			var Minv = (Mat.Dense(x.Length, x.Length, (i, j) => GreenFunction2D(Vector2.Distance(x[i], x[j]))) + lambda * Mat.DenseIdentity(x.Length)).Inverse();
			var temp = N.Transpose() * Minv; // Avoid computing this twice
			var b = (temp * N).Inverse() * temp * yAsVec;
			var w = Minv * (yAsVec - N * b);

			return point => x.Select(xx => Vector2.Distance(xx, point)).ScalarProduct(w) + b[0] + b[1] * point.x + b[2] * point.y;
		}

		/// <summary>
		// Returns f:R^2 -> R satisfying f(s) = t for each pair (s, t) in x.Zip(y).
		// Among all such functions, f minimizes the energy ∬ ∑ᵢⱼ (∂ᵢ∂ⱼf)².
		/// </summary>
		public static Func<Vector2, float> Solve(Vector2[] x, float[] y) => SolveSmoothed(x, y, 0f);
		public static Func<Vector3, float> SolveSmoothed(Vector3[] x, float[] y, float lambda) {
			Debug.Assert(x.Length == y.Length);
			Debug.Assert(lambda >= 0);
			// Compared with the 2D case, the 3D Green function is the distance itself, thence inlined
			var Mat = Matrix<double>.Build;

			var yAsVec = Vector<double>.Build.DenseOfArray(Array.ConvertAll(y, x => (double)x));
			var N = Mat.DenseOfRowArrays(x.Select(xx => new[] { 1d, xx.x, xx.y, xx.z }));
			Debug.Log($"Matrix N is {N.ToMatrixString()}");
			var M = Mat.Dense(x.Length, x.Length, (i, j) => Vector3.Distance(x[i], x[j])) + lambda * Mat.DenseIdentity(x.Length);
			var Minv = M.Inverse();
			var temp = N.Transpose() * Minv; // Avoid computing this twice
			var b = (N.Transpose() * (Minv * N)).Solve(temp * yAsVec);
			//			var b = (N.Transpose() * (Minv * N)).Inverse() * temp * yAsVec;
			var bAsFloat = b.Select(x => (float)x).ToArray();
			var w = Minv * (yAsVec - N * b);
			var wAsFloat = w.Select(x => (float)x);
			return point => x.Select(xx => Vector3.Distance(xx, point)).ScalarProduct(wAsFloat) + bAsFloat[0] + bAsFloat[1] * point.x + bAsFloat[2] * point.y + bAsFloat[3] * point.z;
		}

		public static Func<Vector3, float> Solve(Vector3[] x, float[] y) => SolveSmoothed(x, y, 0f);

		private static float ScalarProduct(this IEnumerable<float> first, IEnumerable<float> second) =>
			first.Zip(second, (a, b) => a * b).Sum();

		// See e.g. Rohit Saboo's PhD thesis, eqs (3.2) - (3.3)
		// Warning: no diffeomorphism guaranty
		// Returns f:R^3 -> R^3 satisfying f(s) = t for each pair (s, t) in x.Zip(y).
		// Each coordinate component fₖ, 0 ≤ k ≤ 2, minimizes the energy ∭ ∑ᵢⱼ (∂ᵢ∂ⱼfₖ)².
		public static Func<Vector3, Vector3> SabooSmoothedDisplacementField(Vector3[] x, Vector3[] y, float lambda, bool rescale = false) {
			Debug.Assert(x.Length == y.Length);
			Debug.Assert(lambda >= 0);
			Vector3 minima, maxima, diff;
			if (rescale) {
				var bounds = GeometryUtility.CalculateBounds(x.Concat(y).ToArray(), Matrix4x4.identity);
				Debug.Log($"Bounds are {bounds.min} to {bounds.max}");
				minima = bounds.min;
				maxima = bounds.max;
				diff = maxima - minima;
				var scale = new Vector3(1 / diff.x, 1 / diff.y, 1 / diff.z);
				x = Array.ConvertAll(x, xx => Vector3.Scale(xx - minima, scale));
				y = Array.ConvertAll(y, xx => Vector3.Scale(xx - minima, scale));
				var componentsRescaled = Enumerable.Range(0, 3).Select(i => SolveSmoothed(x, Array.ConvertAll(y, yy => yy[i]), lambda)).ToArray();
				return point => minima + Vector3.Scale(diff, new(
					componentsRescaled[0](point),
					componentsRescaled[1](point),
					componentsRescaled[2](point)));
			}
			var components = Enumerable.Range(0, 3).Select(i => SolveSmoothed(x, Array.ConvertAll(y, yy => yy[i]), lambda)).ToArray();
			return point => new(components[0](point), components[1](point), components[2](point));
		}

		public static Func<Vector3, Vector3> SabooDisplacementField(Vector3[] x, Vector3[] y) => SabooSmoothedDisplacementField(x, y, 0f);
	}
}

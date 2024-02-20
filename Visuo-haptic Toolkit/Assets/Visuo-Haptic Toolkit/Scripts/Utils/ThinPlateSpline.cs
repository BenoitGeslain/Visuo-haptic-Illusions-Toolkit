using System.Linq;
using UnityEngine;
using VHToolkit;
using System;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;


namespace VHToolkit {
    public static class ThinPlateSpline {
        // The notation follows [Eberly 96, Thin-Plate Splines] at https://www.geometrictools.com/Documentation/ThinPlateSplines.pdf.
        static float GreenFunction2D(float distance) => distance * distance * MathF.Log(distance);

        /// <summary>
        // Returns f:R^2 -> R satisfying f(s) = t for each pair (s, t) in x.Zip(y).
        // Among all such functions, f minimizes the energy ∬ ∑ᵢⱼ (∂ᵢ∂ⱼf)².
        /// </summary>
        public static Func<Vector2, float> Solve(Vector2[] x, float[] y)
        {
            Debug.Assert(x.Length == y.Length);
            var Mat = Matrix<float>.Build;

            var yAsVec = Vector<float>.Build.DenseOfArray(y);
            var N = Mat.DenseOfRowArrays(x.Select(xx => new[] { 1, xx.x, xx.y }));
            // Compute (M + lambda * Id)^-1 only once
            var Minv = Mat.Dense(x.Length, x.Length, (i, j) => GreenFunction2D(Vector2.Distance(x[i], x[j]))).Inverse();
            var temp = N.Transpose() * Minv; // Avoid computing this twice
            var b = (temp * N).Inverse() * temp * yAsVec;
            var w = Minv * (yAsVec - N * b);

            return point => x.Select(xx => Vector2.Distance(xx, point)).ScalarProduct(w) + b[0] + b[1] * point.x + b[2] * point.y;
        }

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

        public static Func<Vector3, float> Solve(Vector3[] x, float[] y) {
            Debug.Assert(x.Length == y.Length);
            // Compared with the 2D case, the 3D Green function is the distance itself, thence inlined
            var Mat = Matrix<float>.Build;

            var yAsVec = Vector<float>.Build.DenseOfArray(y);
            var N = Mat.DenseOfRowArrays(x.Select(xx => new[] { 1, xx.x, xx.y, xx.z }));
            var Minv = Mat.Dense(x.Length, x.Length, (i, j) => Vector3.Distance(x[i], x[j])).Inverse();
            var temp = N.Transpose() * Minv; // Avoid computing this twice
            var b = (temp * N).Inverse() * temp * yAsVec;
            var w = Minv * (yAsVec - N * b);
            return point => x.Select(xx => Vector3.Distance(xx, point)).ScalarProduct(w) + b[0] + b[1] * point.x + b[2] * point.y + b[3] * point.z;
        }

        public static Func<Vector3, float> SolveSmoothed(Vector3[] x, float[] y, float lambda) {
            Debug.Assert(x.Length == y.Length);
            Debug.Assert(lambda >= 0);

            var Mat = Matrix<float>.Build;

            var yAsVec = Vector<float>.Build.DenseOfArray(y);
            var N = Mat.DenseOfRowArrays(x.Select(xx => new[] { 1, xx.x, xx.y, xx.z }));
            var Minv = (Mat.Dense(x.Length, x.Length, (i, j) => Vector3.Distance(x[i], x[j])) + lambda * Mat.DenseIdentity(x.Length)).Inverse();
            var temp = N.Transpose() * Minv; // Avoid computing this twice
            var b = (temp * N).Inverse() * temp * yAsVec;
            var w = Minv * (yAsVec - N * b);
            return point => x.Select(xx => Vector3.Distance(xx, point)).ScalarProduct(w) + b[0] + b[1] * point.x + b[2] * point.y + b[3] * point.z;
        }

        private static float ScalarProduct(this IEnumerable<float> first, IEnumerable<float> second) =>
            first.Zip(second, (a, b) => a * b).Sum();

        // See e.g. Rohit Saboo's PhD thesis, eqs (3.2) - (3.3)
        // Warning: no diffeomorphism guaranty
        // Returns f:R^3 -> R^3 satisfying f(s) = t for each pair (s, t) in x.Zip(y).
        // Each coordinate component fₖ, 0 ≤ k ≤ 2, minimizes the energy ∭ ∑ᵢⱼ (∂ᵢ∂ⱼfₖ)².
        public static Func<Vector3, Vector3> SabooDisplacementField(Vector3[] x, Vector3[] y) {
            Debug.Assert(x.Length == y.Length);
            var components = Enumerable.Range(0, 3).Select(i => Solve(x, Array.ConvertAll(y, yy => yy[i]))).ToArray();

            return point => new(components[0](point), components[1](point), components[2](point));
        }

        public static Func<Vector3, Vector3> SabooSmoothedDisplacementField(Vector3[] x, Vector3[] y, float lambda) {
            Debug.Assert(x.Length == y.Length);
            Debug.Assert(lambda >= 0);
            var components = Enumerable.Range(0, 3).Select(i => SolveSmoothed(x, Array.ConvertAll(y, yy => yy[i]), lambda)).ToArray();

            return point => new(components[0](point), components[1](point), components[2](point));
        }
    }
}

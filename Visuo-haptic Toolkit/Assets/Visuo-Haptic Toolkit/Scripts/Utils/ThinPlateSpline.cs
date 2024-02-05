using System.Linq;
using UnityEngine;
using VHToolkit;
using System;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;


namespace VHToolkit
{
    public static class ThinPlateSpline
    {
        public static Func<Vector2, float> Solve(Vector2[] x, float[] y) => SolveSmoothed(x, y, 0f);

        // The notation follows [Eberly 96, Thin-Plate Splines] at https://www.geometrictools.com/Documentation/ThinPlateSplines.pdf.
        public static Func<Vector2, float> SolveSmoothed(Vector2[] x, float[] y, float lambda) {
            Debug.Assert(x.Length == y.Length);
            Debug.Assert(lambda >= 0);
            static float GreenFunction2D(Vector2 a, Vector2 b) {
                var d = Vector2.Distance(a, b);
                return d * d * Mathf.Log(d);
            }
            var Mat = Matrix<float>.Build;

            var yAsVec = Vector<float>.Build.DenseOfArray(y);
            var N = Mat.DenseOfRowArrays(x.Select(xx => new[] { 1, xx.x, xx.y }));
            // Compute (M + lambda * Id)^-1 only once
            var Minv = (Mat.Dense(x.Length, x.Length, (i, j) => GreenFunction2D(x[i], x[j])) + lambda * Mat.DenseIdentity(x.Length)).Inverse();
            var temp = N.Transpose() * Minv; // Avoid computing this twice
            var b = (temp * N).Inverse() * temp * yAsVec;
            var w = Minv * (yAsVec - N * b);

            return (point) => x.Select(xx => GreenFunction2D(xx, point)).ScalarProduct(w) + b[0] + b[1] * point.x + b[2] * point.y;
        }

        public static Func<Vector3, float> Solve(Vector3[] x, float[] y) => SolveSmoothed(x, y, 0f);

        public static Func<Vector3, float> SolveSmoothed(Vector3[] x, float[] y, float lambda) {
            Debug.Assert(x.Length == y.Length);
            Debug.Assert(lambda >= 0);
            static float GreenFunction3D(Vector3 a, Vector3 b) => Vector3.Distance(a, b);
            var Mat = Matrix<float>.Build;

            var yAsVec = Vector<float>.Build.DenseOfArray(y);
            var N = Mat.DenseOfRowArrays(x.Select(xx => new[] { 1, xx.x, xx.y, xx.z }));
            var Minv = (Mat.Dense(x.Length, x.Length, (i, j) => GreenFunction3D(x[i], x[j])) + lambda * Mat.DenseIdentity(x.Length)).Inverse();
            var temp = N.Transpose() * Minv; // Avoid computing this twice
            var b = (temp * N).Inverse() * temp * yAsVec;
            var w = Minv * (yAsVec - N * b);

            return (point) => x.Select(xx => GreenFunction3D(xx, point)).ScalarProduct(w) + b[0] + b[1] * point.x + b[2] * point.y + b[3] * point.z;
        }

        private static float ScalarProduct(this IEnumerable<float> first, IEnumerable<float> second) =>
            first.Zip(second, (a, b) => a * b).Sum();
    }
}

using System.Linq;
using UnityEngine;
using VHToolkit;
using System;
using MathNet.Numerics.LinearAlgebra;

namespace VHToolkit
{
    public static class ThinPlateSpline
    {
        public static Func<Vector2, float> Solve(Vector2[] x, float[] y) => SolveSmoothed(x, y, 0f);

        // The notation follows [Eberly 96, Thin-Plate Splines].
        public static Func<Vector2, float> SolveSmoothed(Vector2[] x, float[] y, float lambda)
        {
            static float GreenFunction2D(Vector2 a, Vector2 b)
            {
                var d = Vector2.Distance(a, b);
                return d * d * Mathf.Log(d);
            }
            var Mat = Matrix<float>.Build;

            var yVec = Vector<float>.Build.DenseOfArray(y);
            var N = Mat.DenseOfRowArrays(x.Select(xx => new[] { 1, xx.x, xx.y }));
            var Minv = (Mat.Dense(x.Length, x.Length, (i, j) => GreenFunction2D(x[i], x[j])) + lambda * Mat.DenseIdentity(x.Length)).Inverse();
            var b = (N.Transpose() * Minv * N).Inverse() * N.Transpose() * Minv * yVec;
            var w = Minv * (yVec - N * b);

            return (point) => x.Select(xx => GreenFunction2D(xx, point)).Zip(w, (ww, g) => ww * g).Sum() + b[0] + b[1] * point.x + b[2] * point.y;
        }
    }
}

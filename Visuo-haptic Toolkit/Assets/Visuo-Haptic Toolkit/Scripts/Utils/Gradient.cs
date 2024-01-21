using UnityEngine;
using System;
using UnityEditor.PackageManager;
using System.Collections.Generic;
using System.Linq;

namespace VHToolkit
{
    static class MathTools
    {
        /// <summary>
        /// Compute the gradient of its input <c>function</c> at position <c>x</c> by the central differences method.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static Vector3 Gradient3(Func<Vector3, float> function, Vector3 x)
        {
            var eps = Vector3.kEpsilon;
            return new Vector3(
                function(x + eps * Vector3.right) - function(x + eps * Vector3.left),
                function(x + eps * Vector3.up) - function(x + eps * Vector3.down),
                function(x + eps * Vector3.forward) - function(x + eps * Vector3.back)
            ) / (2 * eps);

        }

        /// <summary>
        /// Compute the gradient of its input <c>function</c> at position <c>x</c> by the central differences method.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static Vector2 Gradient2(Func<Vector2, float> function, Vector2 x)
        {
            var eps = Vector2.kEpsilon;
            return new Vector2(
                function(x + eps * Vector2.right) - function(x + eps * Vector2.left),
                function(x + eps * Vector2.up) - function(x + eps * Vector2.down)
            ) / (2 * eps);
        }

        static Vector2 ProjectToHorizontalPlane(this Vector3 v) => new(v.x, v.z);

        static Func<Vector2, float> RepulsivePotential(List<Collider2D> obstacles) =>
            (x) => obstacles.Sum(o => 1 / Vector2.Distance(x, o.ClosestPoint(x)));
    }
}


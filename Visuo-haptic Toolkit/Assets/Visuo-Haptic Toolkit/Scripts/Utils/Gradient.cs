using UnityEngine;
using System;

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
        static Vector3 Gradient(Func<Vector3, float> function, Vector3 x)
        {
            var eps = Vector3.kEpsilon;
            return new Vector3(
                function(x + eps * Vector3.right) - function(x + eps * Vector3.left),
                function(x + eps * Vector3.up) - function(x + eps * Vector3.down),
                function(x + eps * Vector3.forward) - function(x + eps * Vector3.back)
            ) / (2 * eps);

        }

    }
}


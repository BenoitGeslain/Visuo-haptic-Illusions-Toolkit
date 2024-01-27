using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D.IK;
using UnityEngine.UIElements;

namespace VHToolkit {
    static class MathTools {
        /// <summary>
        /// Compute the gradient of its input <c>function</c> at position <c>x</c> by the central differences method.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static Vector3 Gradient3(Func<Vector3, float> function, Vector3 x) {
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
        static Vector2 Gradient2(Func<Vector2, float> function, Vector2 x) {
            var eps = Vector2.kEpsilon;
            return new Vector2(
                function(x + eps * Vector2.right) - function(x + eps * Vector2.left),
                function(x + eps * Vector2.up) - function(x + eps * Vector2.down)
            ) / (2 * eps);
        }

        static Func<Vector2, float> RepulsivePotential(List<Collider2D> obstacles) =>
            (x) => obstacles.Sum(o => 1 / Vector2.Distance(x, o.ClosestPoint(x)));

        // The potential is given as || x - goal || / 2. Will become unstable near goal.
        static Vector2 GradientOfAttractivePotential(Vector2 goal, Vector2 x) => (x - goal).normalized / 2;


        public readonly struct PositionAndRotation2D {
            public readonly Vector2 position;
            public readonly Vector2 forward;

            public PositionAndRotation2D(Vector2 position = new(), Vector2 forward = new()) {
                this.position = position;
                this.forward = forward;
            }
        }

        // TODO careful, this isn't the same choice as Vector2.Vector2 / Vector2.Vector3. (Those project to a vertical plane.)
        static public Vector2 ProjectToHorizontalPlane(this Vector3 v) => new(v.x, v.z);
        static public Vector3 LiftFromHorizontalPlane(this Vector2 v) => new(v.x, 0, v.y);

        // Compute the forward kinematics chain from link-to-link length and angle information.
        static public List<PositionAndRotation2D> ForwardKinematics(PositionAndRotation2D origin, List<float> lengths, List<float> angles) {
            Debug.Assert(lengths.Count == angles.Count);
            List<PositionAndRotation2D> result = new() { origin };
            var position = origin.position;
            var direction = origin.forward.LiftFromHorizontalPlane();
            foreach (var (length, angle) in lengths.Zip(angles)) {
                Debug.Assert(length > 0);
                Debug.Assert(Mathf.Abs(angle) <= 180f);
                position += length * result.Last().forward;
                direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                result.Add(new(position, direction.ProjectToHorizontalPlane()));
            }
            return result;
        }

        static public List<Vector2> InverseKinematics(Vector2 origin, Vector2 end, List<float> lengths) {
            // TODO figure out sensible solver limit and tolerance
            bool wasSolved = false;
            int solverLimit = 1;
            float tolerance = Vector2.kEpsilon;
            Vector2[] positions = new Vector2[lengths.Count - 1];
            while (!wasSolved) {
                wasSolved = FABRIK2D.Solve(end, solverLimit, tolerance, lengths.ToArray(), ref positions);
                solverLimit *= 2;
                tolerance *= 2;
            }
            return positions.Select(v => origin + v).ToList();
        }
    }
}

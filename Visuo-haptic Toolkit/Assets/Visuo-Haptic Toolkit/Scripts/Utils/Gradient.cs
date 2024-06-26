using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace VHToolkit {
	public static class MathTools {
		/// <summary>
		/// Compute the gradient of its input <c>function</c> at position <c>x</c> by the central differences method.
		/// </summary>
		/// <param name="function"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static Vector3 Gradient3(Func<Vector3, float> function, Vector3 x) {
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
		public static Vector2 Gradient2(Func<Vector2, float> function, Vector2 x) {
			var eps = Vector2.kEpsilon;
			return new Vector2(
				function(x + eps * Vector2.right) - function(x + eps * Vector2.left),
				function(x + eps * Vector2.up) - function(x + eps * Vector2.down)
			) / (2 * eps);
		}

		public static Func<Vector3, float> RepulsivePotential3D(List<Collider> obstacles, float exponent = 1f) =>
			(x) => obstacles.Sum(o => 1 / Mathf.Pow(Vector3.Distance(x, o.ClosestPoint(x)), exponent));

		public static Func<Vector3, float> RepulsivePotential3D(List<Vector3> obstacles, float exponent = 1f) =>
			(x) => obstacles.Sum(o => 1 / Mathf.Pow(Vector3.Distance(x, o), exponent));


		public static Vector2 Gradient2v2(Vector2 x, List<Collider2D> obstaclescolliders) {
			float RepulsivePotential(Vector2 x) => obstaclescolliders.Sum(o => 1 / Vector2.Distance(x, o.ClosestPoint(x)));
			var eps = Vector2.kEpsilon;
			return new Vector2(
				RepulsivePotential(x + eps * Vector2.right) - RepulsivePotential(x + eps * Vector2.left),
				RepulsivePotential(x + eps * Vector2.up) - RepulsivePotential(x + eps * Vector2.down)
			) / (2 * eps);
		}

		// The potential is given as || x - goal || / 2.
		public static Vector2 GradientOfAttractivePotential(Vector2 goal, Vector2 x) => (x - goal).normalized / 2;
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
		static public Vector3 ProjectToHorizontalPlaneV3(this Vector3 v) => new(v.x, 0, v.z);
	}
}

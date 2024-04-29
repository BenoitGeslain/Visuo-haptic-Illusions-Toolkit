using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VHToolkit;
using static VHToolkit.MathTools;

// Adapted from UnityEngine.U2D.IK
public static class CCD2D {
	/// <summary>
	/// Solve the 2D inverse kinematics problem for a 2D kinematic chain consisting of rigid links connected by revolute joints.
	/// The algorithm used is cyclic coordinate descent (CCD).
	/// </summary>
	/// <param name="targetPosition">The target position for the "end" of the kinematic chain.</param>
	/// <param name="forward">A nonzero vector orthogonal to the 2D plane in which the problem is embedded.</param>
	/// <param name="maxSteps">A limit number of iterations.</param>
	/// <param name="tolerance">Numerical tolerance for the position errors.</param>
	/// <param name="velocity">A parameter that controls convergence speed.</param>
	/// <param name="positions">An array containing the positions of the joints, bookended by those of the chain extremities ("start effector", "end effector"). Its values evolve over iteration steps.</param>
	/// <returns><c>true</c> if the solver converges.</returns>
	public static bool Solve(Vector3 targetPosition, Vector3 forward, int maxSteps, float tolerance, float velocity, ref Vector3[] positions) {
		int step = 0;
		float sqrTolerance = tolerance * tolerance;
		bool hasConverged;
		while (!(hasConverged = (targetPosition - positions[^1]).sqrMagnitude <= sqrTolerance) && step++ < maxSteps) {
			DoIteration(targetPosition, forward, velocity, ref positions);
		}
		return hasConverged;
	}

	private static void DoIteration(Vector3 targetPosition, Vector3 forward, float velocity, ref Vector3[] positions) {
		for (int num = positions.Length - 2; num >= 0; num--) {
			float bearingToTarget = Vector3.SignedAngle(positions[^1] - positions[num], targetPosition - positions[num], forward);
			float rotationalCorrection = Mathf.Lerp(0f, bearingToTarget, velocity);
			var rotation = Quaternion.AngleAxis(rotationalCorrection, forward);
			for (int num2 = positions.Length - 2; num2 > num; num2--) {
				positions[num2] = positions[num] + rotation * (positions[num2] - positions[num]);
			}
		}
	}
}

// Adapted from UnityEngine.U2D.IK
public static class MyFABRIK2D {
	/// <summary>
	/// Solve the 2D inverse kinematics problem for a 2D kinematic chain consisting of rigid links connected by revolute joints.
	/// The algorithm used is FABRIK.
	/// </summary>
	/// <param name="targetPosition">The target position for the "end" of the kinematic chain.</param>
	/// <param name="maxSteps">A limit number of iterations.</param>
	/// <param name="tolerance">Numerical tolerance for the position errors.</param>
	/// <param name="lengths">Rigid link lengths.</param>
	/// <param name="positions">An array containing the positions of the joints, bookended by those of the chain extremities. Its values evolve over iteration steps.</param>
	/// <returns><c>true</c> if the solver converges.</returns>
	public static bool SolveWithFloatingOrigin(Vector2 targetPosition, int maxSteps, float tolerance, float[] lengths, ref Vector2[] positions) {
		int step = 0;
		float sqrTolerance = tolerance * tolerance;
		bool hasConverged;
		while (!(hasConverged = (targetPosition - positions[^1]).sqrMagnitude <= sqrTolerance) && step++ < maxSteps) {
			Forward(targetPosition, lengths, ref positions);
			Backward(positions[0], lengths, ref positions);
		}
		return hasConverged;
	}

	public static bool SolveWithFixedOrigin(Vector2 originPosition, Vector2 targetPosition, int maxSteps, float tolerance, float[] lengths, ref Vector2[] positions) {
		Debug.Assert(
			lengths.Sum() >= Vector2.Distance(originPosition, targetPosition),
			"Link lengths insufficient to cover desired distance."
		);
		int step = 0;
		float sqrTolerance = tolerance * tolerance;
		bool hasConverged;
		while (!(hasConverged = (targetPosition - positions[^1]).sqrMagnitude <= sqrTolerance) && step++ < maxSteps) {
			Forward(targetPosition, lengths, ref positions);
			Backward(originPosition, lengths, ref positions);
		}
		return hasConverged;
	}

	private static void Forward(Vector2 targetPosition, IList<float> lengths, ref Vector2[] positions) {
		positions[^1] = targetPosition;
		for (int num2 = positions.Length - 2; num2 >= 0; num2--) {
			float num3 = lengths[num2] / (positions[num2 + 1] - positions[num2]).magnitude;
			positions[num2] = Vector2.LerpUnclamped(positions[num2 + 1], positions[num2], num3);
		}
	}

	private static void Backward(Vector2 originPosition, IList<float> lengths, ref Vector2[] positions) {
		positions[0] = originPosition;
		int num = positions.Length - 1;
		for (int i = 0; i < num; i++) {
			float num2 = lengths[i] / (positions[i + 1] - positions[i]).magnitude;
			positions[i + 1] = Vector2.LerpUnclamped(positions[i], positions[i + 1], num2);
		}
	}

	// Own functions:
	static public IList<Vector2> InverseKinematics(Vector2 origin, Vector2 end, IList<float> lengths) {
		// TODO figure out sensible solver limit and tolerance
		bool wasSolved = false;
		int solverLimit = 1;
		float tolerance = Vector2.kEpsilon;
		Vector2[] positions = new Vector2[lengths.Count - 1];
		while (!wasSolved) {
			wasSolved = SolveWithFixedOrigin(origin, end, solverLimit, tolerance, lengths.ToArray(), ref positions);
			solverLimit *= 2;
			tolerance *= 2;
		}
		return positions.ToList();
	}

	// Compute the forward kinematics chain from link lengths and joint angles.
	static public IList<PositionAndRotation2D> ForwardKinematics(PositionAndRotation2D origin, IList<float> lengths, IList<float> angles) {
		Debug.Assert(lengths.Count == angles.Count,
			"The number of revolute joints must be the number of links minus one."
		);
		List<PositionAndRotation2D> result = new() { origin };
		var position = origin.position;
		var direction = origin.forward.LiftFromHorizontalPlane();
		foreach (var (length, angle) in lengths.Zip(angles)) {
			Debug.Assert(length > 0, "Link lengths must be positive.");
			Debug.Assert(Mathf.Abs(angle) <= 180f);
			position += length * result.Last().forward;
			direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
			result.Add(new(position, direction.ProjectToHorizontalPlane()));
		}
		return result;
	}
}

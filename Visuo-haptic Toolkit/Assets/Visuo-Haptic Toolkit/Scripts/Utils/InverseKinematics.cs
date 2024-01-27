using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VHToolkit;
using static VHToolkit.MathTools;


// Adapted from UnityEngine.U2D.IK
public static class MyFABRIK2D
{
    public static bool Solve(Vector2 targetPosition, int solverLimit, float tolerance, float[] lengths, ref Vector2[] positions)
    {
        int step = 0;
        float sqrTolerance = tolerance * tolerance;
        while ((targetPosition - positions[^1]).sqrMagnitude > sqrTolerance && step < solverLimit)
        {
            Forward(targetPosition, lengths, ref positions);
            Backward(positions[0], lengths, ref positions);
            ++step;
        }
        return step != 0;
    }

    
    private static void Forward(Vector2 targetPosition, IList<float> lengths, ref Vector2[] positions)
    {
        int num = positions.Length - 1;
        positions[num] = targetPosition;
        for (int num2 = num - 1; num2 >= 0; num2--)
        {
            float num3 = lengths[num2] / (positions[num2 + 1] - positions[num2]).magnitude;
            positions[num2] = (1f - num3) * positions[num2 + 1] + num3 * positions[num2];
        }
    }

    private static void Backward(Vector2 originPosition, IList<float> lengths, ref Vector2[] positions)
    {
        positions[0] = originPosition;
        int num = positions.Length - 1;
        for (int i = 0; i < num; i++)
        {
            float num2 = lengths[i] / (positions[i + 1] - positions[i]).magnitude;
            positions[i + 1] = (1f - num2) * positions[i] + num2 * positions[i + 1];
        }
    }

    // Own functions:
    static public List<Vector2> InverseKinematics(Vector2 origin, Vector2 end, List<float> lengths) {
        // TODO figure out sensible solver limit and tolerance
        bool wasSolved = false;
        int solverLimit = 1;
        float tolerance = Vector2.kEpsilon;
        Vector2[] positions = new Vector2[lengths.Count - 1];
        while (!wasSolved) {
            wasSolved = Solve(end, solverLimit, tolerance, lengths.ToArray(), ref positions);
            solverLimit *= 2;
            tolerance *= 2;
        }
        return positions.Select(v => origin + v).ToList();
    }
    
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
}

using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// TODO fix namespace issues

namespace Assets.Visuo_Haptic_Toolkit.Scripts.Logging.UserTrajectorySimulation {


    public class MinJerkTrajectory {
        /// <summary>
        /// Minimum Jerk Trajectory (L2 norm) between the input points from t=0 to t=1 with boundary conditions speed = acceleration = 0.
        /// </summary>
        /// <param name="pos0"></param>
        /// <param name="pos1"></param>
        /// <returns></returns>
        static System.Func<float, Vector3> minJerkTrajectory(Vector3 pos0, Vector3 pos1) {
            float[] coeffsByIncreasingPower = { 0, 0, 0, 10, -15, 6 };
            return t => pos0 + (pos1 - pos0) * (float)coeffsByIncreasingPower.Select((a, i) => a * Mathf.Pow(t, i)).Sum();
        }
    }
}
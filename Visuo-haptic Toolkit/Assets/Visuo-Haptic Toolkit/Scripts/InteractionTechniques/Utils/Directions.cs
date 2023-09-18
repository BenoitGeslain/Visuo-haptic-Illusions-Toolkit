using UnityEngine;

namespace BG.Redirection {
	[System.Serializable]
	public class Vector2Translation {
		public float forward;
		public float backward;
	}

	[System.Serializable]
	public class Vector2Rotation {
		public float same;
		public float opposite;
	}
}
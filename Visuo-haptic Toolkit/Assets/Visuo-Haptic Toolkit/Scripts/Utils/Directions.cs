using UnityEngine;

namespace BG.Redirection {
	[System.Serializable]
	public class Vector2Translation {
		public float forward;
		public float backward;
	}

	[System.Serializable]
	public class Vector2Rotation {
		public float left;
		public float right;
	}

	[System.Serializable]
	public class ZDirection {
		public float forward;
		public float backward;
	}
}
using UnityEngine;

namespace BG.Redirection {
	[System.Serializable]
	public class Vector2Horizontal {
		public float left;
		public float right;
	}
	[System.Serializable]
	public class Vector2Vertical {
		public float up;
		public float down;
	}
	[System.Serializable]
	public class Vector2Gain {
		public float faster;
		public float slower;
	}

	[System.Serializable]
	public class Vector2Rotation {
		public float same;
		public float opposite;
	}
}
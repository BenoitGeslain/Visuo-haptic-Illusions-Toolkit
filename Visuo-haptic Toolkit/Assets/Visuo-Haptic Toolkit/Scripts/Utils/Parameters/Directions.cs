namespace VHToolkit.Redirection
{
    /// <summary>
    /// Wrapper for a pair of <c>float</c> associated with left and right.
    /// </summary>
    [System.Serializable]
	public class Vector2Horizontal {
		public float left;
		public float right;
	}

	/// <summary>
	/// Wrapper for a pair of <c>float</c> associated with up and down.
	/// </summary>
	[System.Serializable]
	public class Vector2Vertical {
		public float up;
		public float down;
	}

	/// <summary>
	/// Wrapper for a pair of <c>float</c> associated with forward and backward.
	/// </summary>
	[System.Serializable]
	public class Vector2Gain {
		public float forward;
		public float backward;
	}

	/// <summary>
	/// Wrapper for a pair of <c>float</c> associated with same and opposite.
	/// </summary>
	[System.Serializable]
	public class Vector2Rotation {
		public float same;
		public float opposite;
	}
}
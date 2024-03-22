using UnityEngine;

namespace VHToolkit.Redirection.Interpolation3D {
	public class ThreeDimensionalInterpolation : Interaction {
		[SerializeField] private ThreeDTechnique _technique;
		public ThreeDTechnique Technique { get => _technique; set => _technique = value; }
		private ThreeDTechnique previousTechnique;
		public InterpolationTechnique techniqueInstance;

		/// <summary>
		/// Updates the techniqueInstance according to the enumeration technique chosen.
		/// </summary>
		private void UpdateTechnique() {
			techniqueInstance = _technique switch {
				ThreeDTechnique.None => new NoInterpolation(),
				ThreeDTechnique.Kohli2010RedirectedTouching => new Kohli2010RedirectedTouching(),
				_ => null
			};

			if (techniqueInstance is null)
				Debug.LogError("Error Unknown Redirection technique.");
		}

		/// <summary>
		/// Start function called once when the game is starting. This function calls updateTechnique() to instantiate the technique class and
		/// initializes the previous head positions.
		/// </summary>
		private void Start() {
			UpdateTechnique();
			previousTechnique = Technique;
		}

		/// <summary>
		/// Update function called once per frame. This function
		/// calls updateTechnique() to instantiate the technique class,
		/// calls Redirect(...) from the InterpolationTechnique class to apply the redirection,
		/// applies redirection to the physical hand.
		/// </summary>
		private void LateUpdate() {
			if (previousTechnique != Technique || techniqueInstance == null) {
				UpdateTechnique();
				previousTechnique = Technique;
			}

			(redirect ? techniqueInstance : new NoInterpolation())?.Redirect(scene);
		}
	}
}
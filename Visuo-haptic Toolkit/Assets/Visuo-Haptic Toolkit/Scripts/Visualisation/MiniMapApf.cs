using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VHToolkit.Redirection;

namespace VHToolkit.Visualisation {
	public class ApfVisualisation : MonoBehaviour {
		public bool DrawRayCast;

		public bool DisplayGradient;

		private List<Collider2D> obstacleColliders;
		GameObject PhysicalUser2d;
		GameObject[] gradients;
		GameObject PtitGradient;
		readonly Sprite Arrow = Resources.Load<Sprite>("gradient/fleche");
		readonly Sprite Warning = Resources.Load<Sprite>("gradient/warning");
		int totalSteps;

		public void OnEnable() => obstacleColliders = GameObject.FindGameObjectsWithTag("Obstacle").Select(o => o.GetComponent<Collider2D>()).ToList();

		public void Start() => PhysicalUser2d = GameObject.Find("2duser");

		public void Update() {
			if (DrawRayCast) RaycasttoObstaclesDraw();
			if (DisplayGradient) GradientsDraw();
		}
		void RaycasttoObstaclesDraw() {
			foreach (Collider2D obscol in obstacleColliders) {
				Vector2 closestpt = obscol.ClosestPoint(PhysicalUser2d.transform.position);
				Debug.DrawLine(PhysicalUser2d.transform.position, closestpt, Color.red, .01f);
			}
		}

		void GradientsDraw() {

			if (gradients == null) {
				PtitGradient = Resources.Load<GameObject>("gradient/flechego");
				var bounds = GameObject.Find("Map").GetComponent<MeshCollider>().bounds;
				Vector2 map_size = bounds.size;
				Vector2 map_center = bounds.center;
				int steps = 1;

				totalSteps = (int)(System.Math.Floor(map_size.x / steps) * System.Math.Floor(map_size.y / steps));
				gradients = new GameObject[totalSteps];

				int i = 0;

				for (int x = (int)(map_center.x - map_size.x / 2) + steps; x < map_center.x + map_size.x / 2; x += steps) {
					for (int y = (int)(map_center.y - map_size.y / 2) + steps; y < map_center.y + map_size.y / 2; y += steps) {

						Vector2 Gradobject = ApfRedirection.ComputeGradient(new Vector2(x, y));


						var grad = Instantiate(PtitGradient);
						gradients[i] = grad;
						grad.transform.position = new Vector3(x, y, 2);

						float angleInDegrees = Mathf.Atan2(Gradobject.y, Gradobject.x) * Mathf.Rad2Deg;

						if (!float.IsNaN(Gradobject.x) && !float.IsNaN(Gradobject.y)) {
							grad.transform.rotation = Quaternion.Euler(0, 0, angleInDegrees);
							grad.GetComponent<Renderer>().material.color = new Color(1f * Gradobject.magnitude, 1f * Gradobject.magnitude, 1f);
						}
						else {
							grad.GetComponent<SpriteRenderer>().sprite = Warning;
						}
						i++;
					}
				}
			}

			else if (totalSteps > 0 && gradients.Length == totalSteps) {
				foreach (GameObject gradientgameobj in gradients) {
					if (gradientgameobj != null) {

						Vector2 gradObject = ApfRedirection.ComputeGradient(gradientgameobj.transform.position);

						if (!float.IsNaN(gradObject.x) && !float.IsNaN(gradObject.y)) {
							float angleInDegrees = Mathf.Atan2(gradObject.y, gradObject.x) * Mathf.Rad2Deg;
							gradientgameobj.transform.rotation = Quaternion.Euler(0, 0, angleInDegrees);
							gradientgameobj.GetComponent<Renderer>().material.color = new Color(1f * gradObject.magnitude, 1f * gradObject.magnitude, 1f);
							gradientgameobj.GetComponent<SpriteRenderer>().sprite = Arrow;
						}
						else {
							gradientgameobj.GetComponent<SpriteRenderer>().sprite = Warning;
						}
					}
				}
			}
		}
	}
}
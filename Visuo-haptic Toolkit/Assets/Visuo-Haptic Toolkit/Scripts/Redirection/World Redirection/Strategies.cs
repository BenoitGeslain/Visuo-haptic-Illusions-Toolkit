using UnityEngine;
using System.Collections.Generic;
using System;

namespace VHToolkit.Redirection {
	public class WorldRedirectionStrategy {

		public virtual Vector3 SteerTo(Scene scene) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}

	public class NoSteering: WorldRedirectionStrategy {

        public override Vector3 SteerTo(Scene scene) => scene.physicalHead.forward;
    }

	public class SteerToCenter: WorldRedirectionStrategy {

        public override Vector3 SteerTo(Scene scene) => scene.targets[0].position - scene.physicalHead.position;
    }

	public class SteerToOrbit: WorldRedirectionStrategy {

		public override Vector3 SteerTo(Scene scene) {
			float distanceToTarget = scene.GetHeadToTargetDistance();
			float angleToTargets = (distanceToTarget < scene.radius ? Mathf.PI / 3 : Mathf.Asin(scene.radius / distanceToTarget)) * Mathf.Rad2Deg;
			var v = Vector3.ProjectOnPlane(scene.targets[0].position - scene.physicalHead.position, Vector3.up);
            Vector3 leftTarget = Quaternion.Euler(0f, angleToTargets, 0f) * v;
			Vector3 rightTarget = Quaternion.Euler(0f, -angleToTargets, 0f) * v;
            return Vector3.Angle(leftTarget, scene.physicalHead.forward) < Vector3.Angle(scene.physicalHead.forward, rightTarget)
                ? leftTarget
                : rightTarget;
        }
    }

	public class SteerToMultipleTargets: WorldRedirectionStrategy {
		/// <summary>
		/// Select the target that has the smallest bearing with the orientation of the user's head.
		/// Return a vector pointing from the physical head in the direction of the selected target.
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public override Vector3 SteerTo(Scene scene) {
            // Equivalent code in later .Net versions:
            // Func<Transform, float> bearing = (Transform t) => Vector3.Angle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), Vector3.ProjectOnPlane(t.position - scene.physicalHead.position, Vector3.up));
            // Transform target = scene.targets.Min(bearing);

            float smallestBearing = 360f;
			Transform target = null;
			foreach (Transform t in scene.targets) {
				float a = Vector3.Angle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), Vector3.ProjectOnPlane(t.position - scene.physicalHead.position, Vector3.up));
				// a *= (scene.physicalHead.position - t.position).sqrMagnitude;
				if (a < smallestBearing) {
					smallestBearing = a;
					target = t;
				}
			}
			scene.selectedTarget = target;
			return target.position - scene.physicalHead.position;
		}
	}

	class SteerInDirection: WorldRedirectionStrategy {
        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public override Vector3 SteerTo(Scene scene) {
			Debug.DrawRay(scene.physicalHead.position, Vector3.Reflect(scene.targets[0].position, scene.physicalHead.right));
            return Vector3.Reflect(scene.targets[0].position, scene.physicalHead.right);
        }
    }

    public class APFRedirection2 : WorldRedirectionStrategy
    {
        GameObject Moncube;
        Func<Vector2, float> RepulsiveFUnc;
        Vector2 LastPositionObjet;
        GameObject PtitGradient;
        GameObject[] ListeGradients;
        Sprite Fleche = Resources.Load<Sprite>("gradient/fleche");
        Sprite Warning = Resources.Load<Sprite>("gradient/warning");
        int totalpas;
        List<Collider2D> Obstaclescolliders;

        public override Vector3 SteerTo(Scene scene)
        {
            Obstaclescolliders = scene.GetAllObstaclesCollider();

            Position2d(scene);
            Vector2 vec = Gradientcompute(scene, Moncube.transform.position);

            GradientsDraw(scene);
            RaycasttoObstaclesDraw(scene);

            return new Vector3(vec.x, 0f, vec.y);
        }

        void RaycasttoObstaclesDraw(Scene scene)
        {
            foreach (Collider2D obscol in Obstaclescolliders)
            {
                Vector2 Closestpt = obscol.ClosestPoint(Moncube.transform.position);
                Debug.DrawLine(Moncube.transform.position, Closestpt, Color.red, .01f);
            }

        }

        void Position2d(Scene scene)
        {
            if (!Moncube)
            {
                Moncube = GameObject.Find("2duser");
            }
            Moncube.transform.position = (MathTools.ProjectToHorizontalPlane(scene.physicalHead.transform.position));
        }

        Vector2 Gradientcompute(Scene scene, Vector2 PositionObjet)
        {
            RepulsiveFUnc = MathTools.RepulsivePotential(scene.GetAllObstaclesCollider());
            Vector2 CalculGradient = Vector2.one;

            if (PositionObjet != LastPositionObjet)
            {
                CalculGradient = MathTools.Gradient2(RepulsiveFUnc, PositionObjet);

                LastPositionObjet = PositionObjet;
            }

            return CalculGradient;
        }

        void GradientsDraw(Scene scene)
        {


            if (ListeGradients == null)
            {
                PtitGradient = Resources.Load<GameObject>("gradient/flechego");

                Vector2 map_size = GameObject.Find("Map").GetComponent<MeshCollider>().bounds.size;
                Vector2 map_center = GameObject.Find("Map").GetComponent<MeshCollider>().bounds.center;
                int pas = 1;

                totalpas = (int)Math.Floor(map_size.x / pas) * (int)Math.Floor(map_size.y / pas);
                ListeGradients = new GameObject[totalpas];



                int i = 0;

                for (int x = (int)(map_center.x - map_size.x / 2) + pas; x < map_center.x + map_size.x / 2; x += pas)
                {
                    for (int y = (int)(map_center.y - map_size.y / 2) + pas; y < map_center.y + map_size.y / 2; y += pas)
                    {

                        Vector2 Gradobject = Gradientcompute(scene, new Vector2(x, y));


                        ListeGradients[i] = UnityEngine.Object.Instantiate(PtitGradient);
                        ListeGradients[i].transform.position = new Vector3(x, y, 2);

                        float angleRadian = Mathf.Atan2(Gradobject.y, Gradobject.x);
                        float angleEnDegres = angleRadian * Mathf.Rad2Deg;

                        if (!float.IsNaN(Gradobject.x) && !float.IsNaN(Gradobject.y))
                        {

                            Quaternion nouvelleRotation = Quaternion.Euler(0, 0, angleEnDegres);

                            ListeGradients[i].transform.rotation = nouvelleRotation;
                            ListeGradients[i].GetComponent<Renderer>().material.color = new Color(1f * Gradobject.magnitude, 1f * Gradobject.magnitude, 1f);

                        }

                        else
                        {
                            ListeGradients[i].GetComponent<SpriteRenderer>().sprite = Warning;

                        }
                        i++;
                    }
                }
            }

            else if (totalpas > 0 && ListeGradients.Length == totalpas)
            {
                foreach (GameObject gradientgameobj in ListeGradients)
                {
                    if (gradientgameobj != null)
                    {


                        float x = gradientgameobj.transform.position.x;
                        float y = gradientgameobj.transform.position.y;

                        Vector2 Gradobject = Gradientcompute(scene, new Vector2(x, y));

                        float angleRadian = Mathf.Atan2(Gradobject.y, Gradobject.x);
                        float angleEnDegres = angleRadian * Mathf.Rad2Deg;

                        if (!float.IsNaN(Gradobject.x) && !float.IsNaN(Gradobject.y))
                        {

                            Quaternion nouvelleRotation = Quaternion.Euler(0, 0, angleEnDegres);

                            gradientgameobj.transform.rotation = nouvelleRotation;
                            gradientgameobj.GetComponent<Renderer>().material.color = new Color(1f * Gradobject.magnitude, 1f * Gradobject.magnitude, 1f);
                            gradientgameobj.GetComponent<SpriteRenderer>().sprite = Fleche;
                        }

                        else
                        {
                            gradientgameobj.GetComponent<SpriteRenderer>().sprite = Warning;

                        }

                    }


                }
            }




        }
    }

    public class ApfRedirection : WorldRedirectionStrategy
	{
		GameObject PhysicalUser2d;

        [SerializeField] public GameObject User2d;

		List<Collider2D> Obstaclescolliders;
		Vector3 gradient = Vector2.zero;
        public override Vector3 SteerTo(Scene scene)
		{
			Operateur(scene);
			return gradient;
            
        }

		void Operateur (Scene scene)

		{
			Obstaclescolliders = GetAllObstaclesCollider();
			Position2d(scene);
			gradient = Gradientcompute (PhysicalUser2d.transform.position);

		}

		public static List<Collider2D> GetAllObstaclesCollider()
        {
            GameObject[] Obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
			List<Collider2D> ObstaclesColliders = new();

            for (int i = 0; i < Obstacles.Length; i++)
            {
                ObstaclesColliders.Add(Obstacles[i].GetComponent<Collider2D>());
            }

			return ObstaclesColliders;
        }



        void Position2d (Scene scene)
		{
            if (!PhysicalUser2d)
            {
                PhysicalUser2d = GameObject.Find("2duser");
            }
            PhysicalUser2d.transform.position = MathTools.ProjectToHorizontalPlane(scene.physicalHead.transform.position);
        }

		public static Vector2 Gradientcompute (Vector2 PositionObjet)
		{
            Func <Vector2,float> RepulsiveFUnc ()
			{
				return MathTools.RepulsivePotential(GetAllObstaclesCollider());
			}


            var CalculGradient = MathTools.Gradient2(RepulsiveFUnc(), PositionObjet);


			return CalculGradient;
        }

		
	}
	
}

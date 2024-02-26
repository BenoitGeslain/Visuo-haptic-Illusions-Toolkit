using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VHToolkit.Redirection;

namespace VHToolkit.Visualisation
{
    public class ApfVisualisation : MonoBehaviour

    {
        [SerializeField]
        public bool TraitObjet;

        [SerializeField]
        public bool AffichageGradient;

        private List<Collider2D> Obstaclescolliders;
        GameObject PhysicalUser2d;
        GameObject[] ListeGradients;
        GameObject PtitGradient;
        Sprite Fleche = Resources.Load<Sprite>("gradient/fleche");
        Sprite Warning = Resources.Load<Sprite>("gradient/warning");
		int totalpas;

        public void OnEnable ()
        {
            Obstaclescolliders = GameObject.FindGameObjectsWithTag("Obstacle").Select(o => o.GetComponent<Collider2D>()).ToList();
        }

        public void Start()
        {
            PhysicalUser2d = GameObject.Find("2duser");
        }

        public void Update()
        {
            if (TraitObjet) RaycasttoObstaclesDraw();
            if (AffichageGradient) GradientsDraw();
        }
        void RaycasttoObstaclesDraw()
        {
            foreach (Collider2D obscol in Obstaclescolliders)
            {
                Vector2 Closestpt = obscol.ClosestPoint(PhysicalUser2d.transform.position);
                Debug.DrawLine(PhysicalUser2d.transform.position, Closestpt, Color.red, .01f);
            }

        }

        void GradientsDraw()
        {


            if (ListeGradients == null)
            {
                PtitGradient = Resources.Load<GameObject>("gradient/flechego");

                Vector2 map_size = GameObject.Find("Map").GetComponent<MeshCollider>().bounds.size;
                Vector2 map_center = GameObject.Find("Map").GetComponent<MeshCollider>().bounds.center;
                int pas = 1;

                totalpas = (int)System.Math.Floor(map_size.x / pas) * (int)System.Math.Floor(map_size.y / pas);
                ListeGradients = new GameObject[totalpas];



                int i = 0;

                for (int x = (int)(map_center.x - map_size.x / 2) + pas; x < map_center.x + map_size.x / 2; x += pas)
                {
                    for (int y = (int)(map_center.y - map_size.y / 2) + pas; y < map_center.y + map_size.y / 2; y += pas)
                    {

                        Vector2 Gradobject = ApfRedirection.ComputeGradient(new Vector2(x, y));


                        ListeGradients[i] = Instantiate(PtitGradient);
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

                        Vector2 Gradobject =ApfRedirection.ComputeGradient(gradientgameobj.transform.position);

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

}
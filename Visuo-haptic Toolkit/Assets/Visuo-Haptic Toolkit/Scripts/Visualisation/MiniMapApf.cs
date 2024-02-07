using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using VHToolkit.Redirection;

namespace VHToolkit.Visualisation
{
    public class ApfVisualisation : MonoBehaviour

    {
        void RaycasttoObstaclesDraw(Scene scene)
        {
            foreach (Collider2D obscol in Obstaclescolliders)
            {
                Vector2 Closestpt = obscol.ClosestPoint(Moncube.transform.position);
                Debug.DrawLine(Moncube.transform.position, Closestpt, Color.red, .01f);
            }

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

}
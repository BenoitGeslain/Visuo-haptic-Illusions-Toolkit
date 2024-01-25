using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VHToolkit.Redirection
//MathTools.Gradient2() //retourner gradient a partir d'une fonction et position x
//MathTools.ProjectToHorizontalPlane () // modifier Vecteur position x->y
//MathTools.RepulsivePotential() // calcul repultion a partir d'une liste de colliders
//MathTools.GradientOfAttractivePotential() // calcul attraction à partir de x et xgoal

{

    public class Afp_controller : MonoBehaviour
    {
        [SerializeField]
        [Tooltip ("mettre le tracking réel de la position du casque utilisateur")]
        GameObject Physical_headset;
        [SerializeField]
        [Tooltip("position cible idéal (centre de la pièce?)")]
        Vector2 xgoal;
        [SerializeField]
        [Tooltip("Orientation gradient")]
        GameObject gradientobject;

        GameObject[] Obstacles;
        List<Collider> ObstaclesColliders = new ();
        Vector2 UserPosition;
        Vector2 LastUserPosition;
        Func<Vector2, float> RepulsiveFUnc;




        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("script lancé");
            GetAllObstaclesCollider();
            // initialisation de la fonction repulsive dans start, les obstacles sont considérés immobiles
            RepulsiveFUnc = MathTools.RepulsivePotential3d(ObstaclesColliders);


        }

        // Update is called once per frame
        void Update()
        {
            GetUserPosition();
            
        }

        void GetAllObstaclesCollider()
        {
            Obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

            for (int i=0; i<Obstacles.Length;i++)
            {
                ObstaclesColliders.Add (Obstacles[i].GetComponent<Collider>());
            }
        }

        void GetUserPosition()
        {
            UserPosition = MathTools.ProjectToHorizontalPlane(Physical_headset.transform.position);
            if (UserPosition!=LastUserPosition)
            {
                UserPosition = MathTools.ProjectToHorizontalPlane(Physical_headset.transform.position);
                GradientCompute();
                LastUserPosition = UserPosition;
            }
        }

        void GradientCompute ()
        {
            Vector2 Gradient = MathTools.Gradient2(RepulsiveFUnc, UserPosition);
            Debug.Log(Gradient);

            gradientobject.transform.position = Physical_headset.transform.position;
            gradientobject.transform.eulerAngles = new Vector3 (Gradient.x,Gradient.y);
        }


    }

}

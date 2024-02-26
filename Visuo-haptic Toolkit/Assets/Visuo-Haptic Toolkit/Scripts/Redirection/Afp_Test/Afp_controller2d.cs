using Oculus.VoiceSDK.UX;
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

    public class Afp_controller2d : MonoBehaviour
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
        List<Collider2D> ObstaclesColliders = new ();
        Vector2 UserPosition;
        Vector2 LastUserPosition;
        Func<Vector2, float> RepulsiveFUnc;




        // Start is called before the first frame update
        void Start()
        {
            GetAllObstaclesCollider();
            // initialisation de la fonction repulsive dans start, les obstacles sont considérés immobiles
            RepulsiveFUnc = MathTools.RepulsivePotential2D(ObstaclesColliders);


        }

        // Update is called once per frame
        void Update()
        {
            GetGradient();
            MoveUserKeyboard();



        }

        void GetAllObstaclesCollider()
        {
            Obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

            for (int i=0; i<Obstacles.Length;i++)
            {
                ObstaclesColliders.Add (Obstacles[i].GetComponent<Collider2D>());
            }
        }

        void GetGradient()
        {
            UserPosition = Physical_headset.transform.position;
            
            if (UserPosition!=LastUserPosition)
            {
                GradientCompute();
                LastUserPosition = UserPosition;
            }
        }

        void GradientCompute ()
        {
            Vector2 Gradient = MathTools.Gradient2(RepulsiveFUnc, UserPosition);

            Debug.Log(Gradient);

            GradientRepresentation(Gradient);





        }

        // tentative de représenter l'orientation du gradient (pour Théo!)
        void GradientRepresentation (Vector2 Gradient)
        {
            gradientobject.transform.position = Physical_headset.transform.position + gradientobject.transform.right * 1.3f;

            Gradient.Normalize();

            Debug.Log(Gradient);

            // représenter l'orientation
            float angleRadian = Mathf.Atan2(Gradient.y, Gradient.x);
            float angleEnDegres = angleRadian * Mathf.Rad2Deg;

            Quaternion nouvelleRotation = Quaternion.Euler(0, 0, angleEnDegres);

            gradientobject.transform.rotation = nouvelleRotation;

            //représenter la taille

            Vector3 nouvelleTaille = new Vector3(Gradient.magnitude, gradientobject.transform.localScale.y, gradientobject.transform.localScale.z);

            // Appliquer la nouvelle taille au GameObject
            gradientobject.transform.localScale = nouvelleTaille;
        }

        void MoveUserKeyboard()
        {
            float rotationSpeed = 50f; // Vitesse de rotation (ajustez selon vos besoins)
            float horizontalInput = Input.GetAxis("Horizontal"); // Récupère l'entrée des touches gauche/droite
            Physical_headset.transform.Rotate(Vector3.up, -horizontalInput * rotationSpeed * Time.deltaTime);

            float moveSpeed = 5f; // Vitesse de déplacement (ajustez selon vos besoins)
            float verticalInput = Input.GetAxis("Vertical"); // Récupère l'entrée des touches haut/bas
            Physical_headset.transform.Translate(Vector3.forward * verticalInput * moveSpeed * Time.deltaTime);
        }

    }

}

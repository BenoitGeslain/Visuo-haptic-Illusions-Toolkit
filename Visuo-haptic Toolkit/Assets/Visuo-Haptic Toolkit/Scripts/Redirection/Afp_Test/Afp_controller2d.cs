using Oculus.VoiceSDK.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VHToolkit.Redirection
//MathTools.Gradient2() //retourner gradient a partir d'une fonction et position x
//MathTools.ProjectToHorizontalPlane () // modifier Vecteur position x->y
//MathTools.RepulsivePotential() // calcul repultion a partir d'une liste de colliders
//MathTools.GradientOfAttractivePotential() // calcul attraction � partir de x et xgoal

{

    public class Afp_controller2d : MonoBehaviour
    {
        [SerializeField]
        [Tooltip ("mettre le tracking r�el de la position du casque utilisateur")]
        GameObject Physical_headset;
        [SerializeField]
        [Tooltip("position cible id�al (centre de la pi�ce?)")]
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
            // initialisation de la fonction repulsive dans start, les obstacles sont consid�r�s immobiles
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

        // tentative de repr�senter l'orientation du gradient (pour Th�o!)
        void GradientRepresentation (Vector2 Gradient)
        {
            gradientobject.transform.position = Physical_headset.transform.position + gradientobject.transform.right * 1.3f;

            Gradient.Normalize();

            Debug.Log(Gradient);

            // repr�senter l'orientation
            float angleRadian = Mathf.Atan2(Gradient.y, Gradient.x);
            float angleEnDegres = angleRadian * Mathf.Rad2Deg;

            Quaternion nouvelleRotation = Quaternion.Euler(0, 0, angleEnDegres);

            gradientobject.transform.rotation = nouvelleRotation;

            //repr�senter la taille

            Vector3 nouvelleTaille = new Vector3(Gradient.magnitude, gradientobject.transform.localScale.y, gradientobject.transform.localScale.z);

            // Appliquer la nouvelle taille au GameObject
            gradientobject.transform.localScale = nouvelleTaille;
        }

        void MoveUserKeyboard()
        {
            float rotationSpeed = 50f; // Vitesse de rotation (ajustez selon vos besoins)
            float horizontalInput = Input.GetAxis("Horizontal"); // R�cup�re l'entr�e des touches gauche/droite
            Physical_headset.transform.Rotate(Vector3.up, -horizontalInput * rotationSpeed * Time.deltaTime);

            float moveSpeed = 5f; // Vitesse de d�placement (ajustez selon vos besoins)
            float verticalInput = Input.GetAxis("Vertical"); // R�cup�re l'entr�e des touches haut/bas
            Physical_headset.transform.Translate(Vector3.forward * verticalInput * moveSpeed * Time.deltaTime);
        }

    }

}

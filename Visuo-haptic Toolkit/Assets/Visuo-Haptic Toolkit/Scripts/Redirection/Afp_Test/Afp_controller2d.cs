using Oculus.VoiceSDK.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VHToolkit.Redirection
//MathTools.Gradient2() //retourner gradient a partir d'une fonction et position x
//MathTools.ProjectToHorizontalPlane () // modifier Vecteur position x->y
//MathTools.RepulsivePotential() // calcul repultion a partir d'une liste de colliders
//MathTools.GradientOfAttractivePotential() // calcul attraction ï¿½ partir de x et xgoal

{

    public class Afp_controller2d : MonoBehaviour
    {
        [SerializeField]
        [Tooltip ("mettre le tracking rï¿½el de la position du casque utilisateur")]
        GameObject Physical_headset;
        [SerializeField]
        [Tooltip("position cible idï¿½al (centre de la piï¿½ce?)")]
        Vector2 xgoal;
        [SerializeField]
        [Tooltip("Orientation gradient")]
        GameObject gradientobject;

        GameObject[] Obstacles;
        List<Collider> ObstaclesColliders = new ();
        Vector2 UserPosition;
        Vector2 LastUserPosition;
        Func<Vector3, float> RepulsiveFunc;

        [SerializeField] Shader GradientHeatmapShader;
        [SerializeField] GameObject GradientPrefab;
        [SerializeField] Shader shad;
        Renderer rend;
        readonly System.Random rand = new System.Random();
        const int STEPS = 60;
        float[] DensityTable;
        float maxX;
        float minX;
        float maxY;
        float minY;
        float width;
        float height;
        float stepX;
        float stepY;
        GameObject map;

        // Start is called before the first frame update
        void Start()
        {
            GetAllObstaclesCollider();
            // initialisation de la fonction repulsive dans start, les obstacles sont considï¿½rï¿½s immobiles
            RepulsiveFunc = MathTools.RepulsivePotential3D(ObstaclesColliders);

            DensityTable = new float[STEPS * STEPS];

            if (ObstaclesColliders.Count > 0) {

                maxX = ObstaclesColliders[0].bounds.max.x;
                minX = ObstaclesColliders[0].bounds.min.x;
                maxY = ObstaclesColliders[0].bounds.max.y;
                minY = ObstaclesColliders[0].bounds.min.y;

                foreach (Collider col in ObstaclesColliders.Skip(1)) {

                    float iMaxX = col.bounds.max.x;
                    float iMinX = col.bounds.min.x;
                    float iMaxY = col.bounds.max.y;
                    float iMinY = col.bounds.min.y;

                    if (iMaxX > maxX) maxX = iMaxX;
                    if (iMinX < minX) minX = iMinX;
                    if (iMaxY > maxY) maxY = iMaxY;
                    if (iMinY < minY) minY = iMinY;

                }

                width = maxX - minX;
                height = maxY - minY;

                stepX = width / STEPS;
                stepY = height / STEPS;

                Debug.Log($"INIT: X[{minX}:{maxX}] Y[{minY}:{maxY}]");
                Debug.Log($"INIT: width:{width} height:{height}");

                map = Instantiate(GradientPrefab);
                map.transform.position = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 6.8f);
                map.transform.localScale = new Vector3(width, height, 0);
                rend = map.GetComponent<Renderer>();

                UpdateShader();
            }
            
        }

        // Update is called once per frame
        void Update()
        {
            GetGradient();
            MoveUserKeyboard();
            InvokeRepeating("UpdateShader", 6f, 5f);
        }

        void UpdateShader()
        {
            for (int y = 0; y < STEPS; y++)
            {
                for (int x = 0; x < STEPS; x++)
                {

                    Vector2 Position = new Vector2(minX + (x * stepX) + (stepX / 2), minY + (y * stepY) + (stepY / 2));
                    Vector2 Gradient = MathTools.Gradient3(RepulsiveFunc, Position);

                    float Magnitude = Gradient.magnitude;
                    float Valeur = Math.Min(1, Math.Max(0, Magnitude));

                    DensityTable[x + (y * STEPS)] = Valeur;
                    //Debug.Log($"{stepX * x:0.0} - {stepY * y:0.0} : {Valeur}");
                }

            }

            float MaxDen = DensityTable.Max();

            rend.material.shader = shad;
            rend.material.SetFloat("_MaxDen", MaxDen);
            rend.material.SetFloatArray("_DensityTable", DensityTable);
        }

        void GetAllObstaclesCollider()
        {
            Obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

            for (int i=0; i<Obstacles.Length;i++)
            {
                ObstaclesColliders.Add (Obstacles[i].GetComponent<Collider>());
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
            Vector2 Gradient = MathTools.Gradient3(RepulsiveFunc, UserPosition);

            GradientRepresentation(Gradient);

        }

        // tentative de representation l'orientation du gradient (pour Theo!)
        void GradientRepresentation (Vector2 Gradient)
        {
            gradientobject.transform.position = Physical_headset.transform.position + gradientobject.transform.right * 1.3f;

            Gradient.Normalize();

            Debug.Log($"Mag: {Gradient.magnitude}");

            // representer l'orientation
            float angleRadian = Mathf.Atan2(Gradient.y, Gradient.x);
            float angleEnDegres = angleRadian * Mathf.Rad2Deg;

            Quaternion nouvelleRotation = Quaternion.Euler(0, 0, angleEnDegres);

            gradientobject.transform.rotation = nouvelleRotation;

            //representer la taille

            Vector3 nouvelleTaille = new Vector3(Gradient.magnitude, gradientobject.transform.localScale.y, gradientobject.transform.localScale.z);

            // Appliquer la nouvelle taille au GameObject
            gradientobject.transform.localScale = nouvelleTaille;
        }

        void MoveUserKeyboard()
        {
            float rotationSpeed = 50f; // Vitesse de rotation (ajustez selon vos besoins)
            float horizontalInput = Input.GetAxis("Horizontal"); // Recupere l'input gauche/droite
            Physical_headset.transform.Rotate(Vector3.up, -horizontalInput * rotationSpeed * Time.deltaTime);

            float moveSpeed = 5f; // Vitesse de deplacement (ajustez selon vos besoins)
            float verticalInput = Input.GetAxis("Vertical"); // Recupere l'input haut/bas
            Physical_headset.transform.Translate(Vector3.forward * verticalInput * moveSpeed * Time.deltaTime);
        }

    }

}
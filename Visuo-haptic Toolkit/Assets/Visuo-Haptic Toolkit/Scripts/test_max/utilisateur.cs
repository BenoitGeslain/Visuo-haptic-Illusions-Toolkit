using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using outils;
using System.Linq;
using UnityEngine.AI;


public class NewBehaviourScript : MonoBehaviour
{
    [Header ("Debug tools")]

    [SerializeField]
    [Tooltip("à cocher avant le lancement, affiche tous les objets taggés obstacles")]
    bool afficher_nom_et_position = false;
    [SerializeField]
    [Tooltip("Affiche la distance entre l'utilisateur et chaque obstacle en temps réel")]
    bool afficher_distance_objets = false;
    [SerializeField]
    bool position_utilisateur = false;
    [SerializeField]
    bool afficherp2r = false;



    Vector2 Position_utilisateur;
    Vector2 LastPosition_utilisateur;
    List<Vector2> Position_Obstacles;
    float[] Distance_Obstacles;
    GameObject[] Obstacles_gameObject;


    // récupération de la position des objets uniquement au start 
    // + tard : trouver le point le plus proche de chaque objet-user / optimisation du code pour le mettre dans le update
    void Start()
    {
        Position_Obstacles = Fonctions_utiles.ReturnObstaclePosition(out Obstacles_gameObject);

        if (afficher_nom_et_position)
        {
            for (int i = 0; i < Position_Obstacles.Count; i++)
            {
                Debug.Log($"obstacle {i} : {Obstacles_gameObject[i].name} à la position : {Position_Obstacles[i]} ");

            }
        }

        Distance_Obstacles = new float[Obstacles_gameObject.Length];
        
    }


    void Update()
    {
        Position_utilisateur = Fonctions_utiles.Get2dposition(this.gameObject);

        if (Position_utilisateur != LastPosition_utilisateur)
        {

            if (position_utilisateur) Debug.Log($"L'utilisateur est à la position {Position_utilisateur}");

            CalculDistanceObstacles();

            float p2r = Fonctions_utiles.P2rfunction2d(Position_utilisateur, new Vector2(0, 0), Distance_Obstacles);

            if (afficherp2r)Debug.Log("P2R :" + p2r);

            LastPosition_utilisateur = Position_utilisateur;
        }
    }



    void CalculDistanceObstacles()
    {
        
            for (int i = 0; i < Position_Obstacles.Count; i++)
            {
                Distance_Obstacles[i] = Fonctions_utiles.Distance(Position_utilisateur, Position_Obstacles[i]);

            }

            

            if (afficher_distance_objets)
            {
                for (int i = 0; i < Distance_Obstacles.Length; i++)
                {
                    Debug.Log($"Distance de l'objet {Obstacles_gameObject[i].name} à {Distance_Obstacles[i]} pixels");
                }
            }
        
    }
}

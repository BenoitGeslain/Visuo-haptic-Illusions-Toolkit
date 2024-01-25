using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

namespace outils
{
    public class Fonctions_utiles
    {
        // dans un env 3d, unity utilise les axes x,z au lieu de x,y
        public static Vector2 Get2dposition(GameObject gameobject)
        {
            return new Vector2(gameobject.transform.position.x, gameobject.transform.position.z);
        }

        public static float Distance(Vector2 posutilisateur,Vector2 posobstacle)
        {

            return Vector2.Distance(posutilisateur, posobstacle);
        }

        public static float P2rfunction2d(Vector2 x, Vector2 xgoal, float[] distanceobstacles)
        {
            float attractive = 1 / 2 * Mathf.Abs(Vector2.Distance(x, xgoal));

            float repulsive = 0;

            foreach (float distanceobstacle in distanceobstacles)
            {
                repulsive += 1 / (Mathf.Abs(distanceobstacle));
            }

            return attractive + repulsive;
        }

        public static List<Vector2> ReturnObstaclePosition(out GameObject[] Obstacles_gameObject)
        {
            Obstacles_gameObject = GameObject.FindGameObjectsWithTag("Obstacle");
            List<Vector2> PosObstacles = new();

            for (int i = 0; i < Obstacles_gameObject.Length; i++)
            {
                PosObstacles.Add(Fonctions_utiles.Get2dposition(Obstacles_gameObject[i]));

            }

            return PosObstacles;
        }
    }


}



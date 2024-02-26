using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using VHToolkit;

public class GradientVisuals : MonoBehaviour
{

    public GameObject heatmapQuadPrefab;
    public string obstacleTab;
    [Range(1,64)] public int subdivionsPerSide;
    public bool verbose;

    private Func<Vector3, float> repulsiveFunction;
    private float[] gradientDensityTable;
    private bool anyCollider;
    private float minX, maxX, minZ, maxZ, stepX, stepZ, width, depth;
    private Renderer rend;
    private System.Random rand = new System.Random();
    private GameObject quad;

    // Start is called before the first frame update
    void Start()
    {
        List<Collider> obstaclesCollider = new List<Collider>(GameObject.FindGameObjectsWithTag(obstacleTab).Select(o => o.GetComponent<Collider>()));
        anyCollider = obstaclesCollider.Any();

        if (anyCollider)
        {
            repulsiveFunction = MathTools.RepulsivePotential3D(obstaclesCollider);
            gradientDensityTable = new float[4096];

            Collider[] allColliders = FindObjectsOfType<Collider>().ToArray();

            maxX = allColliders[0].bounds.max.x;
            minX = allColliders[0].bounds.min.x;
            maxZ = allColliders[0].bounds.max.z;
            minZ = allColliders[0].bounds.min.z;

            foreach (Collider col in allColliders.Skip(1))
            {

                float iMaxX = col.bounds.max.x;
                float iMinX = col.bounds.min.x;
                float iMaxZ = col.bounds.max.z;
                float iMinZ = col.bounds.min.z;

                if (iMaxX > maxX) maxX = iMaxX;
                if (iMinX < minX) minX = iMinX;
                if (iMaxZ > maxZ) maxZ = iMaxZ;
                if (iMinZ < minZ) minZ = iMinZ;

            }

            width = maxX - minX;
            depth = maxZ - minZ;

            stepX = width / subdivionsPerSide;
            stepZ = depth / subdivionsPerSide;

            Log($"APF GRADIENT VISUALS INIT : X[{minX}:{maxX}] Z[{minZ}:{maxZ}]");
            Log($"APF GRADIENT VISUALS INIT : width:{width} height:{depth}");

            quad = Instantiate(heatmapQuadPrefab);
            quad.transform.position = new Vector3((minX + maxX) / 2, 0f, (minZ + maxZ) / 2);
            quad.transform.localScale = new Vector2(width, depth);
            rend = quad.GetComponent<Renderer>();

            UpdateShader();
            InvokeRepeating("UpdateShader", 5f, 3f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateShader()
    {
        stepX = width / subdivionsPerSide;
        stepZ = depth / subdivionsPerSide;

        for (int z = 0; z < subdivionsPerSide; z++)
        {
            for (int x = 0; x < subdivionsPerSide; x++)
            {

                Vector3 position = new Vector3(minX + (x * stepX) + (stepX / 2), 0, minZ + (z * stepZ) + (stepZ / 2));
                Vector2 gradient = MathTools.Gradient3(repulsiveFunction, position);

                float magnitude = gradient.magnitude;;
                //float Valeur = Math.Min(1, Math.Max(0, Magnitude));

                gradientDensityTable[x + (z * subdivionsPerSide)] = magnitude;
                Log($"{stepX * x:0.0} - {stepZ * z:0.0} : {magnitude}");
            }

        }

        float maxDen = gradientDensityTable.Max();

        rend.material.SetFloat("_UnitsPerSide", subdivionsPerSide);
        rend.material.SetFloat("_MaxDen", maxDen);
        rend.material.SetFloatArray("_DensityTable", gradientDensityTable);
    }

    private void Log(string msg)
    {
        if (verbose) UnityEngine.Debug.Log(msg);
    }
}

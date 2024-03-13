using Meta.WitAi;
using Oculus.Interaction.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using VHToolkit;
using VHToolkit.Redirection;

public class GradientVisuals : MonoBehaviour
{

    // PROPERTIES //

    // public general
    public string obstacleTab;
    public bool verbose;
    public int refreshRate;

    // public heatmap
    [Header("Heatmap")]
    [InspectorName("Enable Heatmap")] public bool heatmapEnabled;
    public GameObject heatmapQuadPrefab;
    [Range(1,64)] [Tooltip("The higher the finer")] public int heatmapMeshFineness;
    [Range(1,10)] public float clampValue = 1;


    // public vectors
    [Header("Vector Field")]
    [InspectorName("Enable Vectors")] public bool vectorsEnabled;
    [InspectorName("Sprite Flèche")] public Sprite vfFleche;
    [InspectorName("Sprite Warning")] public Sprite vfWarning;

    // private general
    private System.Random rand = new System.Random();
    private Func<Vector3, float> repulsiveFunction;
    private bool heatmapStateSave;
    private bool vectorsStateSave;
    private float minX, maxX, minZ, maxZ, stepX, stepZ, width, depth;
    private List<Collider> obstaclesCollider;

    // private heatmap
    private float[] hmDensityTable;
    private Renderer hmRend;
    private GameObject hmQuad;

    // private vector
    private GameObject vfPhysicalUser2d;
    private List<GameObject> vfVectors;
    private GameObject vfPtitGradient;
    private int vfTotalpas;



    // FUNCS & SUBS //

    // General

    void Start()
    {
        InvokeRepeating(nameof(CheckOnBools), 3f, 1f);
        hmDensityTable = new float[4096];
        vfVectors = new();

        // vf todo
        vfPhysicalUser2d = GameObject.Find("2duser");
        vfFleche = Resources.Load<Sprite>("gradient/fleche");
        vfWarning = Resources.Load<Sprite>("gradient/warning");
    }

    private void Log(string msg)
    {
        if (verbose) UnityEngine.Debug.Log(msg);
    }

    public void CheckOnBools()
    {
        bool hmChanged = heatmapEnabled != heatmapStateSave;
        bool vfChanged = vectorsEnabled != vectorsStateSave;

        if ((hmChanged && heatmapEnabled) || (vfChanged && vectorsEnabled)) InitVisualsTransform(hmChanged && heatmapEnabled, vfChanged && vectorsEnabled);
        if (hmChanged && !heatmapEnabled) CloseHeatmap();
        if (vfChanged && !vectorsEnabled) CloseVectorField();

        heatmapStateSave = heatmapEnabled;
        vectorsStateSave = vectorsEnabled;
    }

    private void InitVisualsTransform(bool initHeatmap = false, bool initVectors = false)
    {
        if (!initHeatmap && !initVectors) return;

        obstaclesCollider = new List<Collider>(GameObject.FindGameObjectsWithTag(obstacleTab).Select(o => o.GetComponent<Collider>()));

        if (obstaclesCollider.Any())
        {
            repulsiveFunction = MathTools.RepulsivePotential3D(obstaclesCollider);

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

            stepX = width / heatmapMeshFineness;
            stepZ = depth / heatmapMeshFineness;

            Log($"APF GRADIENT VISUALS INIT : X[{minX}:{maxX}] Z[{minZ}:{maxZ}]");
            Log($"APF GRADIENT VISUALS INIT : width:{width} height:{depth}");

            // heatmap
            if (initHeatmap)
            {
                hmQuad = Instantiate(heatmapQuadPrefab, this.transform);
                hmQuad.transform.position = new Vector3((minX + maxX) / 2, 0f, (minZ + maxZ) / 2);
                hmQuad.transform.localScale = new Vector2(width, depth);
                hmQuad.layer = LayerMask.NameToLayer("Visuals");

                hmRend = hmQuad.GetComponent<Renderer>();

                UpdateHeatmap();
                InvokeRepeating("UpdateHeatmap", 3f, 1f);
            } 
            else if (hmQuad != null)
            {
                hmQuad.transform.position = new Vector3((minX + maxX) / 2, hmQuad.transform.position.y, (minZ + maxZ) / 2);
                hmQuad.transform.localScale = new Vector2(width, depth);
            }


            // vectors

            int r = 12; // todo
            int i = 0;

            if (initVectors)
            {
                for (int z = 0; z < r; z++)
                {
                    for (int x = 0; x < r; x++)
                    {
                        GameObject vectorObj = new($"vector_{i}");

                        Vector3 position = new(minX + (x * (width/r)) + ((width / r) / 2), 0, minZ + (z * (depth / r)) + (depth / r));
                        Vector2 gradient = MathTools.Gradient3(repulsiveFunction, position);

                        vectorObj.transform.parent = this.transform;
                        vectorObj.transform.position = new Vector3(position.x, transform.position.y + 0.001f, position.z);
                        vectorObj.AddComponent<SpriteRenderer>();

                        if (!float.IsNaN(gradient.x) && !float.IsNaN(gradient.y))
                        {
                            float angleRadian = Mathf.Atan2(gradient.y, gradient.x);
                            float angleEnDegres = angleRadian * Mathf.Rad2Deg;
                            Quaternion rotation = Quaternion.Euler(-90, 0, angleEnDegres);

                            vectorObj.transform.rotation = rotation;
                            vectorObj.GetComponent<SpriteRenderer>().sprite = vfFleche;
                        }

                        else
                        {
                            vectorObj.GetComponent<SpriteRenderer>().sprite = vfWarning;
                        }

                        vfVectors.Add(vectorObj);
                        i++;
                    }
                }

                UpdateVectors();
                InvokeRepeating("UpdateVectors", 3f, 1f);

            }
        }
        else
        {
            heatmapEnabled = heatmapStateSave = false;
            Log("No colliders detected, can generate Heatmap.");
        }
    }


    // Heatmap

    public void UpdateHeatmap()
    {
        if (!heatmapEnabled) { return; }

        stepX = width / heatmapMeshFineness;
        stepZ = depth / heatmapMeshFineness;

        for (int z = 0; z < heatmapMeshFineness; z++)
        {
            for (int x = 0; x < heatmapMeshFineness; x++)
            {

                Vector3 position = new Vector3(minX + (x * stepX) + (stepX / 2), 0, minZ + (z * stepZ) + (stepZ / 2));
                Vector2 gradient = MathTools.Gradient3(repulsiveFunction, position);

                float magnitude = gradient.magnitude;
                float hmValue = magnitude;

                hmDensityTable[x + (z * heatmapMeshFineness)] = hmValue;
                Log($"{stepX * x:0.0} - {stepZ * z:0.0} : {hmValue}");
            }

        }

        float maxDen = clampValue;

        hmRend.material.SetFloat("_UnitsPerSide", heatmapMeshFineness);
        hmRend.material.SetFloat("_MaxDen", maxDen);
        hmRend.material.SetFloatArray("_DensityTable", hmDensityTable);
    }

    public void CloseHeatmap()
    {
        CancelInvoke(nameof(UpdateHeatmap));
        GameObject.Destroy(hmQuad);
        hmQuad = null;
        hmRend = null;
    }


    // Vectors

    public void UpdateVectors()
    {
        if (!vectorsEnabled) { return; }

        foreach (GameObject obj in vfVectors)
        {
            Vector2 gradient = MathTools.Gradient3(repulsiveFunction, obj.transform.position);

            if (!float.IsNaN(gradient.x) && !float.IsNaN(gradient.y))
            {
                float angleRadian = Mathf.Atan2(gradient.y, gradient.x);
                float angleEnDegres = angleRadian * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(-90, 0, angleEnDegres);

                obj.transform.rotation = rotation;
                obj.GetComponent<SpriteRenderer>().sprite = vfFleche;
            }

            else
            {
                obj.transform.rotation = Quaternion.Euler(-90, 0, 0);
                obj.GetComponent<SpriteRenderer>().sprite = vfWarning;
            }

        }

    }

    private void CloseVectorField()
    {
        CancelInvoke(nameof(UpdateHeatmap));
        vfVectors.Clear();
    }


}

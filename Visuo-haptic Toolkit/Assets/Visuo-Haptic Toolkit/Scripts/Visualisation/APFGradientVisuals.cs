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
    public bool traitObjet;

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
    private GameObject[] vfListeGradients;
    private GameObject vfPtitGradient;
    private int vfTotalpas;
    private Sprite vfFleche;
    private Sprite vfWarning;



    // FUNCS & SUBS //

    // General

    void Start()
    {
        InvokeRepeating("CheckOnBools", 3f, 1f);

        // vf todo
        vfPhysicalUser2d = GameObject.Find("2duser");
        vfFleche = Resources.Load<Sprite>("gradient/fleche");
        vfWarning = Resources.Load<Sprite>("gradient/warning");
}

    void Update()
    {
        // vf todo
        if (traitObjet) RaycasttoObstaclesDraw();
        if (vectorsEnabled) GradientsDraw();
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
            hmDensityTable = new float[4096];

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
            if (initVectors)
            {
                // TODO
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
        stepX = width / heatmapMeshFineness;
        stepZ = depth / heatmapMeshFineness;

        for (int z = 0; z < heatmapMeshFineness; z++)
        {
            for (int x = 0; x < heatmapMeshFineness; x++)
            {

                Vector3 position = new Vector3(minX + (x * stepX) + (stepX / 2), 0, minZ + (z * stepZ) + (stepZ / 2));
                Vector2 gradient = MathTools.Gradient3(repulsiveFunction, position);

                float magnitude = gradient.magnitude;;
                //float hmValue = Mathf.Clamp(magnitude, 0, clampValue);
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
        CancelInvoke("UpdateHeatmap");
        GameObject.Destroy(hmQuad);
        hmQuad = null;
        hmRend = null;
    }


    // Vectors
    // TODO

    void RaycasttoObstaclesDraw()
    {
        foreach (Collider obscol in obstaclesCollider)
        {
            Vector3 Closestpt = obscol.ClosestPoint(vfPhysicalUser2d.transform.position);
            UnityEngine.Debug.DrawLine(vfPhysicalUser2d.transform.position, Closestpt, Color.red, .01f);
        }

    }

    void GradientsDraw()
    {


        if (vfListeGradients == null)
        {
            vfPtitGradient = Resources.Load<GameObject>("gradient/flechego");

            Vector2 map_size = GameObject.Find("Map").GetComponent<MeshCollider>().bounds.size;
            Vector2 map_center = GameObject.Find("Map").GetComponent<MeshCollider>().bounds.center;
            int pas = 1;

            vfTotalpas = (int)System.Math.Floor(map_size.x / pas) * (int)System.Math.Floor(map_size.y / pas);
            vfListeGradients = new GameObject[vfTotalpas];



            int i = 0;

            for (int x = (int)(map_center.x - map_size.x / 2) + pas; x < map_center.x + map_size.x / 2; x += pas)
            {
                for (int y = (int)(map_center.y - map_size.y / 2) + pas; y < map_center.y + map_size.y / 2; y += pas)
                {

                    Vector2 Gradobject = ApfRedirection.ComputeGradient(new Vector2(x, y));


                    vfListeGradients[i] = Instantiate(vfPtitGradient);
                    vfListeGradients[i].transform.position = new Vector3(x, y, 2);

                    float angleRadian = Mathf.Atan2(Gradobject.y, Gradobject.x);
                    float angleEnDegres = angleRadian * Mathf.Rad2Deg;

                    if (!float.IsNaN(Gradobject.x) && !float.IsNaN(Gradobject.y))
                    {

                        Quaternion nouvelleRotation = Quaternion.Euler(0, 0, angleEnDegres);

                        vfListeGradients[i].transform.rotation = nouvelleRotation;
                        vfListeGradients[i].GetComponent<Renderer>().material.color = new Color(1f * Gradobject.magnitude, 1f * Gradobject.magnitude, 1f);

                    }

                    else
                    {
                        vfListeGradients[i].GetComponent<SpriteRenderer>().sprite = vfWarning;

                    }
                    i++;
                }
            }
        }

        else if (vfTotalpas > 0 && vfListeGradients.Length == vfTotalpas)
        {
            foreach (GameObject gradientgameobj in vfListeGradients)
            {
                if (gradientgameobj != null)
                {

                    Vector2 Gradobject = ApfRedirection.ComputeGradient(gradientgameobj.transform.position);

                    float angleRadian = Mathf.Atan2(Gradobject.y, Gradobject.x);
                    float angleEnDegres = angleRadian * Mathf.Rad2Deg;

                    if (!float.IsNaN(Gradobject.x) && !float.IsNaN(Gradobject.y))
                    {

                        Quaternion nouvelleRotation = Quaternion.Euler(0, 0, angleEnDegres);

                        gradientgameobj.transform.rotation = nouvelleRotation;
                        gradientgameobj.GetComponent<Renderer>().material.color = new Color(1f * Gradobject.magnitude, 1f * Gradobject.magnitude, 1f);
                        gradientgameobj.GetComponent<SpriteRenderer>().sprite = vfFleche;
                    }

                    else
                    {
                        gradientgameobj.GetComponent<SpriteRenderer>().sprite = vfWarning;

                    }

                }


            }
        }


    }

    private void CloseVectorField()
    {
        // TODO
    }


}

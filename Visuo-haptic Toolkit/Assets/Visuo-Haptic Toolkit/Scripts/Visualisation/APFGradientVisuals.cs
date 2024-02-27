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

    // public heatmap
    [Header("Heatmap")]
    [InspectorName("Enable Heatmap")] public bool heatmapEnabled;
    public GameObject heatmapQuadPrefab;
    [Range(1,64)] [Tooltip("The higher the finer")] public int HeatmapMeshFineness;

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
    private GameObject PhysicalUser2d;
    private GameObject[] ListeGradients;
    private GameObject PtitGradient;
    private int totalpas;
    private Sprite Fleche;
    private Sprite Warning;



    // FUNCS & SUBS //

    // General

    void Start()
    {
        InvokeRepeating("CheckOnBools", 3f, 1f);

        // vf todo
        PhysicalUser2d = GameObject.Find("2duser");
        Fleche = Resources.Load<Sprite>("gradient/fleche");
        Warning = Resources.Load<Sprite>("gradient/warning");
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

        if ((hmChanged && heatmapEnabled) || (vfChanged && vectorsEnabled)) InitVisualsTransform();
        if (hmChanged && !heatmapEnabled) CloseHeatmap();
        if (vfChanged && !vectorsEnabled) CloseVectorField();

        heatmapStateSave = heatmapEnabled;
        vectorsStateSave = vectorsEnabled;
    }

    private void InitVisualsTransform()
    {
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

            stepX = width / HeatmapMeshFineness;
            stepZ = depth / HeatmapMeshFineness;

            Log($"APF GRADIENT VISUALS INIT : X[{minX}:{maxX}] Z[{minZ}:{maxZ}]");
            Log($"APF GRADIENT VISUALS INIT : width:{width} height:{depth}");

            // heatmap
            hmQuad = Instantiate(heatmapQuadPrefab);
            hmQuad.transform.position = new Vector3((minX + maxX) / 2, 0f, (minZ + maxZ) / 2);
            hmQuad.transform.localScale = new Vector2(width, depth);

            hmRend = hmQuad.GetComponent<Renderer>();

            UpdateHeatmap();
            InvokeRepeating("UpdateHeatmap", 3f, 1f);
            
            // vectors todo

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
        stepX = width / HeatmapMeshFineness;
        stepZ = depth / HeatmapMeshFineness;

        for (int z = 0; z < HeatmapMeshFineness; z++)
        {
            for (int x = 0; x < HeatmapMeshFineness; x++)
            {

                Vector3 position = new Vector3(minX + (x * stepX) + (stepX / 2), 0, minZ + (z * stepZ) + (stepZ / 2));
                Vector2 gradient = MathTools.Gradient3(repulsiveFunction, position);

                float magnitude = gradient.magnitude;;
                //float Valeur = Math.Min(1, Math.Max(0, Magnitude));

                hmDensityTable[x + (z * HeatmapMeshFineness)] = magnitude;
                Log($"{stepX * x:0.0} - {stepZ * z:0.0} : {magnitude}");
            }

        }

        float maxDen = hmDensityTable.Max();

        hmRend.material.SetFloat("_UnitsPerSide", HeatmapMeshFineness);
        hmRend.material.SetFloat("_MaxDen", maxDen);
        hmRend.material.SetFloatArray("_DensityTable", hmDensityTable);
    }

    public void CloseHeatmap()
    {
        CancelInvoke("UpdateHeatmap");
        GameObject.Destroy(hmQuad);
        hmRend = null;
    }


    // Vectors todo

    void RaycasttoObstaclesDraw()
    {
        foreach (Collider obscol in obstaclesCollider)
        {
            Vector3 Closestpt = obscol.ClosestPoint(PhysicalUser2d.transform.position);
            UnityEngine.Debug.DrawLine(PhysicalUser2d.transform.position, Closestpt, Color.red, .01f);
        }

    }

    void GradientsDraw()
    {


        if (ListeGradients == null)
        {
            PtitGradient = Resources.Load<GameObject>("gradient/flechego");

            Vector2 map_size = GameObject.Find("Map").GetComponent<MeshCollider>().bounds.size;
            Vector2 map_center = GameObject.Find("Map").GetComponent<MeshCollider>().bounds.center;
            int pas = 1;

            totalpas = (int)System.Math.Floor(map_size.x / pas) * (int)System.Math.Floor(map_size.y / pas);
            ListeGradients = new GameObject[totalpas];



            int i = 0;

            for (int x = (int)(map_center.x - map_size.x / 2) + pas; x < map_center.x + map_size.x / 2; x += pas)
            {
                for (int y = (int)(map_center.y - map_size.y / 2) + pas; y < map_center.y + map_size.y / 2; y += pas)
                {

                    Vector2 Gradobject = ApfRedirection.ComputeGradient(new Vector2(x, y));


                    ListeGradients[i] = Instantiate(PtitGradient);
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

                    Vector2 Gradobject = ApfRedirection.ComputeGradient(gradientgameobj.transform.position);

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

    private void CloseVectorField()
    {
        // TODO
    }


}

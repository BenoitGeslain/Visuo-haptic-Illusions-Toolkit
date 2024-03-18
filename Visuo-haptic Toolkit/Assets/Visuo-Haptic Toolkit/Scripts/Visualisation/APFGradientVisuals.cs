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
    public string obstacleTag;
    public bool verbose;
    [Range(0.5f, 5f)]public float refreshRate;

    // public heatmap
    [Header("Heatmap")]
    [InspectorName("Enable Heatmap")] public bool heatmapEnabled;
    public GameObject heatmapQuadPrefab;
    [Range(1, 64)][Tooltip("The higher the finer")] public int heatmapMeshFineness;
    [Range(1, 10)] public float heatmapClampValue = 1;

    // public vectors
    [Header("Vector Field")]
    [InspectorName("Enable Vectors")] public bool vectorsEnabled;
    [Range(2, 15)] public int vectorsMeshFineness;
    public Sprite arrowSprite;
    public Sprite warningSprite;

    // private general
    private Func<Vector3, float> repulsiveFunc;
    private float minX, maxX, minZ, maxZ, width, depth;
    private List<GameObject> obstacles;

    // private heatmap
    private bool hmEnabledCurrentState;
    private float[] hmDensityTable = new float[4096];
    private Renderer hmRend;
    private GameObject hmQuad;
    private float hmStepX, hmStepZ;

    // private vector
    private bool vfEnabledCurrentState;
    private List<GameObject> vfVectors = new();



    // FUNCS & SUBS //

    #region "Main"

    void Start()
    {
        UpdateRepulsiveFunc();
        InvokeRepeating(nameof(Checks), 3f, 3f);
    }

    /// <summary>
    /// Prints message in console if "verbose" is checked.
    /// </summary>
    /// <param name="msg">Log message</param>
    private void Log(string msg)
    {
        if (verbose) UnityEngine.Debug.Log(msg);
    }

    /// <summary>
    /// Checks a number of things : scene updates, bool changes. If changes are to be made, does them.
    /// </summary>
    private void Checks()
    {
        bool hmToggled = heatmapEnabled != hmEnabledCurrentState;
        bool vfToggled = vectorsEnabled != vfEnabledCurrentState;

        // check for changes in the scene
        bool updateVisuals = CheckForSceneUpdate();
        if (updateVisuals) UpdateRepulsiveFunc();

        // if nothing to change, return
        if (!hmToggled && !vfToggled && !updateVisuals) return;

        if (hmToggled)
        {
            if (heatmapEnabled) { heatmapEnabled = InitHeatmap(); }
            else { CloseHeatmap(); }
        }
        else if (updateVisuals && hmEnabledCurrentState)
        {
            heatmapEnabled = InitHeatmap();
        }

        if (vfToggled)
        {
            if (vectorsEnabled) { vectorsEnabled = InitVectorsField(); }
            else { CloseVectorsField(); }
        }
        else if (updateVisuals && vfEnabledCurrentState)
        {
            vectorsEnabled = InitVectorsField();
        }

        // save state
        hmEnabledCurrentState = heatmapEnabled;
        vfEnabledCurrentState = vectorsEnabled;
    }

    /// <summary>
    /// Updates the repulsive function used to calculate gradient.
    /// </summary>
    private void UpdateRepulsiveFunc()
    {
        // cancel invokes while repulsive function update is ongoing
        CancelInvoke(nameof(UpdateHeatmap));
        CancelInvoke(nameof(UpdateVectorsField));

        // get gameobjects with obstacle tag
        obstacles = new(GameObject.FindGameObjectsWithTag(obstacleTag).Where(o => o.GetComponent<Collider>() != null));

        if (obstacles.Any())
        {
            repulsiveFunc = MathTools.RepulsivePotential3D(new List<Collider>(obstacles.Select(o => o.GetComponent<Collider>())));

            // get all colliders in scene
            List<Collider> allColliders = new(FindObjectsOfType<Collider>());

            maxX = allColliders.Max(o => o.bounds.max.x);
            minX = allColliders.Min(o => o.bounds.min.x);
            maxZ = allColliders.Max(o => o.bounds.max.z);
            minZ = allColliders.Min(o => o.bounds.min.z);
            width = maxX - minX;
            depth = maxZ - minZ;

            Log($"Visuals init : X[{minX}:{maxX}] Z[{minZ}:{maxZ}]");
            Log($"Visuals init : width:{width} height:{depth}");

        }
        else
        {
            repulsiveFunc = null;
            Log("No colliders detected, can't generate visuals.");
        }
    }

    /// <summary>
    /// Checks for changes with gameobjects tagged as obstacles, and also for changes is scene area size.
    /// </summary>
    /// <returns>True if any change found</returns>
    private bool CheckForSceneUpdate()
    {
        if (GameObject.FindGameObjectsWithTag(obstacleTag).Where(o => o.GetComponent<Collider>() != null).Count() != obstacles.Count) return true;

        foreach (var objCol in FindObjectsOfType<Collider>())
        {
            if (objCol.bounds.max.x > maxX) return true;
            if (objCol.bounds.min.x < minX) return true;
            if (objCol.bounds.max.z > maxZ) return true;
            if (objCol.bounds.min.z < minZ) return true;
        }

        return false;
    }

    #endregion



    #region "heatmap"

    /// <summary>
    /// Init heatmap quad with shahder, and invokes update func.
    /// </summary>
    /// <returns>True if init ended sucessfully</returns>
    private bool InitHeatmap()
    {
        if (repulsiveFunc == null)
        {
            Console.Write("APF visuals : cannot init heatmap, no repulsive function (is there any obstacle with collider ?).");
            CloseHeatmap();
            return false;
        }

        if (hmEnabledCurrentState)
        {
            CloseHeatmap();
        }

        hmStepX = width / heatmapMeshFineness;
        hmStepZ = depth / heatmapMeshFineness;

        hmQuad = Instantiate(heatmapQuadPrefab, this.transform);
        hmQuad.transform.position = new Vector3((minX + maxX) / 2, 0f, (minZ + maxZ) / 2);
        hmQuad.transform.localScale = new Vector2(width, depth);
        hmQuad.layer = LayerMask.NameToLayer("Visuals");

        hmRend = hmQuad.GetComponent<Renderer>();

        UpdateHeatmap();
        InvokeRepeating(nameof(UpdateHeatmap), 1f, refreshRate);

        return true;
    }

    /// <summary>
    /// Heatmap quad update method.
    /// </summary>
    private void UpdateHeatmap()
    {
        if (!heatmapEnabled) { return; }

        hmStepX = width / heatmapMeshFineness;
        hmStepZ = depth / heatmapMeshFineness;

        for (int z = 0; z < heatmapMeshFineness; z++)
        {
            for (int x = 0; x < heatmapMeshFineness; x++)
            {

                Vector3 position = new Vector3(minX + (x * hmStepX) + (hmStepX / 2), 0, minZ + (z * hmStepZ) + (hmStepZ / 2));
                Vector2 gradient = MathTools.Gradient3(repulsiveFunc, position);

                float magnitude = gradient.magnitude;
                float hmValue = magnitude;

                hmDensityTable[x + (z * heatmapMeshFineness)] = hmValue;
                Log($"{hmStepX * x:0.0} - {hmStepZ * z:0.0} : {hmValue}");
            }

        }

        hmRend.material.SetFloat("_UnitsPerSide", heatmapMeshFineness);
        hmRend.material.SetFloat("_MaxDen", heatmapClampValue);
        hmRend.material.SetFloatArray("_DensityTable", hmDensityTable);
    }

    /// <summary>
    /// Handles ending the heatmap.
    /// </summary>
    private void CloseHeatmap()
    {
        CancelInvoke(nameof(UpdateHeatmap));
        GameObject.Destroy(hmQuad);
        hmQuad = null;
        hmRend = null;
    }

    #endregion



    #region "Vectors field"

    /// <summary>
    /// Init vectors field, and invokes update func.
    /// </summary>
    /// <returns>True if init ended sucessfully</returns>
    private bool InitVectorsField()
    {
        if (repulsiveFunc == null)
        {
            Console.Write("APF visuals : cannot init vectors field, no repulsive function (is there any obstacle with collider ?).");
            CloseVectorsField();
            return false;
        }

        if (vfEnabledCurrentState)
        {
            CloseVectorsField();
        }

        int i = 0;
        for (int z = 0; z < vectorsMeshFineness; z++)
        {
            for (int x = 0; x < vectorsMeshFineness; x++)
            {
                GameObject vectorObj = new($"vector_{i}");

                Vector3 position = new(minX + (x * (width / vectorsMeshFineness)) + ((width / vectorsMeshFineness) / 2), 0, minZ + (z * (depth / vectorsMeshFineness)) + (depth / vectorsMeshFineness));
                Vector2 gradient = MathTools.Gradient3(repulsiveFunc, position);

                vectorObj.transform.parent = this.transform;
                vectorObj.transform.position = new Vector3(position.x, transform.position.y + 0.001f, position.z);
                vectorObj.AddComponent<SpriteRenderer>();

                if (!float.IsNaN(gradient.x) && !float.IsNaN(gradient.y))
                {
                    float angleRadian = Mathf.Atan2(gradient.y, gradient.x);
                    float angleEnDegres = angleRadian * Mathf.Rad2Deg;
                    Quaternion rotation = Quaternion.Euler(-90, 0, angleEnDegres);

                    vectorObj.transform.rotation = rotation;
                    vectorObj.GetComponent<SpriteRenderer>().sprite = arrowSprite;
                }

                else
                {
                    vectorObj.GetComponent<SpriteRenderer>().sprite = warningSprite;
                }

                vfVectors.Add(vectorObj);
                i++;
            }

        }

        UpdateVectorsField();
        InvokeRepeating(nameof(UpdateVectorsField), 1f, refreshRate);

        return true;
    }

    /// <summary>
    /// Vectors field update method.
    /// </summary>
    private void UpdateVectorsField()
    {
        if (!vectorsEnabled) { return; }

        foreach (GameObject vectorObj in vfVectors)
        {
            Vector2 gradient = MathTools.Gradient3(repulsiveFunc, vectorObj.transform.position);

            if (!float.IsNaN(gradient.x) && !float.IsNaN(gradient.y))
            {
                float angleRadian = Mathf.Atan2(gradient.y, gradient.x);
                float angleEnDegres = angleRadian * Mathf.Rad2Deg * 2; // TODO
                Quaternion rotation = Quaternion.Euler(-90, 0, angleEnDegres);

                vectorObj.transform.rotation = rotation;
                vectorObj.GetComponent<SpriteRenderer>().sprite = arrowSprite;
            }

            else
            {
                vectorObj.transform.rotation = Quaternion.Euler(-90, 0, 0);
                vectorObj.GetComponent<SpriteRenderer>().sprite = warningSprite;
            }

        }

    }

    /// <summary>
    /// Handles the ending of the vector field.
    /// </summary>
    private void CloseVectorsField()
    {
        CancelInvoke(nameof(UpdateVectorsField));

        for (int i = vfVectors.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(vfVectors[i]);
        }

        vfVectors.Clear();
    }

    #endregion
}

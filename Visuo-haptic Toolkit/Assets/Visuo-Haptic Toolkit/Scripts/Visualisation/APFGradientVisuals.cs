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
using static OVRPlugin;

public class GradientVisuals : MonoBehaviour
{

    // PROPERTIES //

    // public general
    public string obstacleTag;
    public bool verbose;
    [Range(0.5f, 5f)] public float refreshRate;

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
    [InspectorName("Arrow Sprite")] public Sprite vfArrow;
    [InspectorName("Sprite Warning")] public Sprite vfWarning;

    // private general
    private Func<Vector3, float> repulsiveFunction;
    private float  width, depth;
    private Vector3 min, max;
    private List<Collider> obstaclesCollider;

    // private heatmap
    private bool hmEnabledCurrentState;
    private float[] hmDensityTable;
    private Renderer hmRenderer;
    private GameObject hmQuad;
    private float hmStepX, hmStepZ;

    // private vector
    private bool vfEnabledCurrentState;
    private List<GameObject> vfVectors;



    // FUNCS & SUBS //

    #region "Main"

    void Start()
    {
        hmDensityTable = new float[4096];
        vfVectors = new();
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
        obstaclesCollider = new(GameObject.FindGameObjectsWithTag(obstacleTag).Where(o => o.GetComponent<Collider>() != null).Select(o => o.GetComponent<Collider>()));

        if (obstaclesCollider.Any())
        {
            static Func<Vector3, Vector3, Vector3> Pointwise(Func<float, float, float> f) =>
                (a, b) => new(f(a.x, b.x), f(a.y, b.y), f(a.z, b.z));

            repulsiveFunction = MathTools.RepulsivePotential3D(obstaclesCollider);

            var allColliderBounds = FindObjectsOfType<Collider>().Select(o => o.bounds.max).ToArray();
            max = allColliderBounds.Aggregate(Pointwise(Mathf.Max));
            min = allColliderBounds.Aggregate(Pointwise(Mathf.Min));
            min.y = max.y = 0f;

            width = max.x - min.x;
            depth = max.z - min.z;

            Log($"Visuals init : X[{min.x}:{max.x}] Z[{min.z}:{max.z}]");
            Log($"Visuals init : width:{width} height:{depth}");

        }
        else
        {
            repulsiveFunction = null;
            Log("No colliders detected, can't generate visuals.");
        }
    }

    /// <summary>
    /// Checks for changes with gameobjects tagged as obstacles, and also for changes is scene area size.
    /// </summary>
    /// <returns>True if any change found</returns>
    private bool CheckForSceneUpdate()
    {
        if (GameObject.FindGameObjectsWithTag(obstacleTag).Where(o => o.GetComponent<Collider>() != null).Count() != obstaclesCollider.Count) return true;

        foreach (var objCol in FindObjectsOfType<Collider>())
        {
            if (objCol.bounds.max.x > max.x) return true;
            if (objCol.bounds.min.x < min.x) return true;
            if (objCol.bounds.max.z > max.z) return true;
            if (objCol.bounds.min.z < min.z) return true;
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
        if (repulsiveFunction == null)
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
        hmQuad.transform.position = 0.5f * (min+max);
        hmQuad.transform.localScale = new Vector2(width, depth);
        hmQuad.layer = LayerMask.NameToLayer("Visuals");

        hmRenderer = hmQuad.GetComponent<Renderer>();

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

                Vector3 position = new Vector3(min.x + (x * hmStepX) + (hmStepX / 2), 0, min.z + (z * hmStepZ) + (hmStepZ / 2));
                Vector2 gradient = MathTools.Gradient3(repulsiveFunction, position);

                float hmValue = gradient.magnitude;

                hmDensityTable[x + (z * heatmapMeshFineness)] = hmValue;
                Log($"{hmStepX * x:0.0} - {hmStepZ * z:0.0} : {hmValue}");
            }

        }

        hmRenderer.material.SetFloat("_UnitsPerSide", heatmapMeshFineness);
        hmRenderer.material.SetFloat("_MaxDen", heatmapClampValue);
        hmRenderer.material.SetFloatArray("_DensityTable", hmDensityTable);
    }

    /// <summary>
    /// Handles ending the heatmap.
    /// </summary>
    private void CloseHeatmap()
    {
        CancelInvoke(nameof(UpdateHeatmap));
        GameObject.Destroy(hmQuad);
        hmQuad = null;
        hmRenderer = null;
    }

    #endregion



    #region "Vectors field"

    /// <summary>
    /// Init vectors field, and invokes update func.
    /// </summary>
    /// <returns>True if init ended sucessfully</returns>
    private bool InitVectorsField()
    {
        if (repulsiveFunction == null)
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

                Vector3 position = new(min.x + (x * (width / vectorsMeshFineness)) + ((width / vectorsMeshFineness) / 2), 0, min.z + (z * (depth / vectorsMeshFineness)) + (depth / vectorsMeshFineness));
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
                    vectorObj.GetComponent<SpriteRenderer>().sprite = vfArrow;
                }

                else
                {
                    vectorObj.GetComponent<SpriteRenderer>().sprite = vfWarning;
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

        foreach (GameObject obj in vfVectors)
        {
            Vector2 gradient = MathTools.Gradient3(repulsiveFunction, obj.transform.position);

            var (angleInDegrees, sprite) = (float.IsNaN(gradient.x) || float.IsNaN(gradient.y)) ?
                 (0f, vfWarning) : (Mathf.Atan2(gradient.y, gradient.x) * Mathf.Rad2Deg, vfArrow);

            obj.transform.rotation = Quaternion.Euler(-90f, 0f, angleInDegrees);
            obj.GetComponent<SpriteRenderer>().sprite = sprite;

        }

    }

    /// <summary>
    /// Handles the ending of the vector field.
    /// </summary>
    private void CloseVectorsField()
    {
        CancelInvoke(nameof(UpdateVectorsField));

        //for (int i = vfVectors.Count - 1; i >= 0; i--)
        //{
        //    GameObject.Destroy(vfVectors[i]);
        //}

        vfVectors.Clear();
    }

    #endregion
}

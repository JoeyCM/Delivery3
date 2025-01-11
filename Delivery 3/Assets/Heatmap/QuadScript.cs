using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuadScript : MonoBehaviour
{
    Material mMaterial;
    MeshRenderer mMeshRenderer;

    float[] mPoints;
    int mHitCount;

    float mDelay;

    // A list of checkboxes (Toggles) and their respective HeatmapManagers
    [SerializeField] private List<Toggle> checkboxes; // Assign UI Checkboxes in the inspector
    [SerializeField] private List<HeatmapManager> heatmapManagers; // Assign HeatmapManager instances in the inspector

    private List<HeatmapManager> activeHeatmapManagers = new List<HeatmapManager>();

    void Start()
    {
        mDelay = 3;

        mMeshRenderer = GetComponent<MeshRenderer>();
        mMaterial = mMeshRenderer.material;

        mPoints = new float[32 * 3]; // 32 points 

        if (heatmapManagers == null || heatmapManagers.Count == 0)
        {
            Debug.LogError("No HeatmapManager instances assigned.");
        }

        if (checkboxes == null || checkboxes.Count != heatmapManagers.Count)
        {
            Debug.LogError("Ensure the number of checkboxes matches the number of HeatmapManager instances.");
        }
        else
        {
            // Set all checkboxes to off at the start
            foreach (var toggle in checkboxes)
            {
                if (toggle != null)
                {
                    toggle.isOn = false; // Ensure all checkboxes start unchecked
                }
            }

            AssignCheckboxListeners();
        }
    }

    void Update()
    {
        mDelay -= Time.deltaTime;
        if (mDelay <= 0)
        {
            // Spawn particles for all active HeatmapManagers
            foreach (var manager in activeHeatmapManagers)
            {
                SpawnHeatmapParticles(manager);
            }

            mDelay = 0.8f;
        }
    }

    private void AssignCheckboxListeners()
    {
        for (int i = 0; i < checkboxes.Count; i++)
        {
            int index = i; // Capture the index for the lambda
            if (checkboxes[i] != null)
            {
                checkboxes[i].onValueChanged.AddListener(isChecked =>
                {
                    ToggleHeatmapManager(index, isChecked);
                });
            }
        }
    }

    private void ToggleHeatmapManager(int index, bool isChecked)
    {
        if (index >= 0 && index < heatmapManagers.Count)
        {
            if (isChecked)
            {
                // Add the HeatmapManager to the active list if it's checked
                if (!activeHeatmapManagers.Contains(heatmapManagers[index]))
                {
                    activeHeatmapManagers.Add(heatmapManagers[index]);
                }
            }
            else
            {
                // Remove the HeatmapManager from the active list if it's unchecked
                activeHeatmapManagers.Remove(heatmapManagers[index]);
            }
        }
        else
        {
            Debug.LogError("Invalid HeatmapManager index.");
        }
    }

    private void SpawnHeatmapParticles(HeatmapManager manager)
    {
        if (manager != null)
        {
            List<Vector3> heatmapPositions = manager.GetHeatmapPositions();

            foreach (var position in heatmapPositions)
            {
                // Instantiate the projectile at the position with Y adjusted by +2
                GameObject go = Instantiate(Resources.Load<GameObject>("Projectile"));
                Vector3 adjustedPosition = position;
                adjustedPosition.y += 1f; // Adjust the Y position by 2
                go.transform.position = adjustedPosition; // Use adjusted position
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint cp in collision.contacts)
        {
            Vector3 StartOfRay = cp.point - cp.normal;
            Vector3 RayDir = cp.normal;

            Ray ray = new Ray(StartOfRay, RayDir);
            RaycastHit hit;

            bool hitit = Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("HeatMapLayer"));

            if (hitit)
            {
                addHitPoint(hit.textureCoord.x * 4 - 2, hit.textureCoord.y * 4 - 2);
            }
            Destroy(cp.otherCollider.gameObject);
        }
    }

    public void addHitPoint(float xp, float yp)
    {
        mPoints[mHitCount * 3] = xp;
        mPoints[mHitCount * 3 + 1] = yp;
        mPoints[mHitCount * 3 + 2] = Random.Range(1f, 3f);

        mHitCount++;
        mHitCount %= 32;

        mMaterial.SetFloatArray("_Hits", mPoints);
        mMaterial.SetInt("_HitCount", mHitCount);
    }
}

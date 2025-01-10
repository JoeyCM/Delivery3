using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadScript : MonoBehaviour
{
    Material mMaterial;
    MeshRenderer mMeshRenderer;

    float[] mPoints;
    int mHitCount;

    float mDelay;

    // Reference to the HeatmapManager script that we will assign in the inspector
    [SerializeField] private HeatmapManager heatmapManager;

    void Start()
    {
        mDelay = 3;

        mMeshRenderer = GetComponent<MeshRenderer>();
        mMaterial = mMeshRenderer.material;

        mPoints = new float[32 * 3]; // 32 points 

        // Ensure HeatmapManager is assigned
        if (heatmapManager == null)
        {
            Debug.LogError("HeatmapManager is not assigned in QuadScript.");
        }
    }

    void Update()
    {
        mDelay -= Time.deltaTime;
        if (mDelay <= 0)
        {
            // Spawn all particles at once based on heatmap data
            SpawnHeatmapParticles();

            mDelay = 0.5f;
        }
    }

    // Spawn particles based on heatmap positions
    private void SpawnHeatmapParticles()
    {
        // Ensure heatmapManager is not null before accessing the positions
        if (heatmapManager != null)
        {
            List<Vector3> heatmapPositions = heatmapManager.GetHeatmapPositions();

            foreach (var position in heatmapPositions)
            {
                // Instantiate the projectile at the position with Y adjusted by +10
                GameObject go = Instantiate(Resources.Load<GameObject>("Projectile"));
                Vector3 adjustedPosition = position;
                adjustedPosition.y += 2f;  // Adjust the Y position by 10
                go.transform.position = adjustedPosition;  // Use adjusted position
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint cp in collision.contacts)
        {
            Debug.Log("Contact with object " + cp.otherCollider.gameObject.name);

            Vector3 StartOfRay = cp.point - cp.normal;
            Vector3 RayDir = cp.normal;

            Ray ray = new Ray(StartOfRay, RayDir);
            RaycastHit hit;

            bool hitit = Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("HeatMapLayer"));

            if (hitit)
            {
                Debug.Log("Hit Object " + hit.collider.gameObject.name);
                Debug.Log("Hit Texture coordinates = " + hit.textureCoord.x + "," + hit.textureCoord.y);
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

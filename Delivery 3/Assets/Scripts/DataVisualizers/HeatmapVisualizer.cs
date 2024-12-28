using UnityEditor;
using UnityEngine;

// Parse imported data to extract positions or event locations.
// Aggregate data into a grid-based format.

[ExecuteInEditMode]
public class HeatmapVisualizer : MonoBehaviour
{
    public Vector3 gridOrigin = Vector3.zero;
    public int gridSizeX = 10;
    public int gridSizeZ = 10;
    public float cellSize = 1.0f;

    // Simulated heatmap data (replace with real data later)
    public float[,] heatmapData;

    void OnDrawGizmos()
    {
        if (heatmapData == null)
            return;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                float intensity = Mathf.Clamp01(heatmapData[x, z]); // Normalize to 0-1
                Gizmos.color = Color.Lerp(Color.blue, Color.red, intensity); // Blue to Red gradient
                Vector3 cellCenter = gridOrigin + new Vector3(x * cellSize, 0, z * cellSize);
                Gizmos.DrawCube(cellCenter, Vector3.one * cellSize * 0.9f);
            }
        }
    }
}

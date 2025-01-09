using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HeatmapManager : MonoBehaviour
{
    [SerializeField] private string serverUrl;
    [SerializeField] private string eventType; // e.g., "OnDeath" or "OnReceiveDamage"
    [SerializeField] private GameObject cubePrefab; // Prefab for heatmap cubes
    [SerializeField] private Transform parentTransform; // Parent for cubes

    private float cubeSizeMultiplier = 1.0f; // Initial size multiplier for the cubes
    private Color cubeColor = Color.red; // Default color

    private List<GameObject> generatedCubes = new List<GameObject>(); // To store instantiated cubes for later updates

    private void Start()
    {
        StartCoroutine(FetchHeatmapData());
    }

    private IEnumerator FetchHeatmapData()
    {
        string url = $"{serverUrl}?request=fetch&eventType={eventType}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Heatmap data received.");
                List<HeatmapData> heatmapData = ParseHeatmapData(request.downloadHandler.text);
                GenerateHeatmap(heatmapData);
            }
            else
            {
                Debug.LogError("Failed to fetch heatmap data: " + request.error);
            }
        }
    }

    private List<HeatmapData> ParseHeatmapData(string json)
    {
        HeatmapData[] dataArray = JsonUtility.FromJson<HeatmapDataWrapper>($"{{\"data\":{json}}}").data;
        return new List<HeatmapData>(dataArray);
    }

    private void GenerateHeatmap(List<HeatmapData> heatmapData)
    {
        if (heatmapData == null || heatmapData.Count == 0)
        {
            Debug.Log("No heatmap data to generate.");
            return;
        }

        // Find the max count for scaling
        int maxCount = 0;
        foreach (var data in heatmapData)
        {
            maxCount = Mathf.Max(maxCount, data.Count);
        }

        foreach (var data in heatmapData)
        {
            // Parse position from string
            Vector3 position = ParsePosition(data.Position);

            // Create a cube at the position
            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, parentTransform);
            generatedCubes.Add(cube); // Store the cube reference for later updates

            // Scale the cube based on count and multiplier (capped at 1.3)
            cube.transform.localScale = Vector3.one * Mathf.Clamp((0.5f + data.Count / (float)maxCount), 0.5f, 1.3f) * cubeSizeMultiplier;

            // Set the cube's color based on selected color
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = cubeColor;  // Use the selected color
            }
        }
    }

    private Vector3 ParsePosition(string positionString)
    {
        positionString = positionString.Trim('(', ')'); // Remove parentheses
        string[] parts = positionString.Split(','); // Split into components (x, y, z)
        return new Vector3(
            float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
            float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
            float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture)
        );
    }

    // Method to set cube size multiplier
    public void SetCubeSizeMultiplier(float value)
    {
        cubeSizeMultiplier = value;
        UpdateCubeSizes(); // Update cubes' size whenever the multiplier changes
    }

    // Method to update the sizes of all cubes
    public void UpdateCubeSizes()
    {
        foreach (var cube in generatedCubes)
        {
            if (cube != null)
            {
                // Update cube size based on the size multiplier
                cube.transform.localScale = Vector3.one * Mathf.Clamp((0.5f + cube.transform.localScale.x), 0.5f, 1.3f) * cubeSizeMultiplier;
            }
        }
    }

    // Method to set cube color
    public void SetCubeColor(Color color)
    {
        cubeColor = color;
        UpdateCubeColors(); // Update cubes' color whenever the color changes
    }

    // Method to update the colors of all cubes
    public void UpdateCubeColors()
    {
        foreach (var cube in generatedCubes)
        {
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = cubeColor;  // Apply the selected color
            }
        }
    }

    [System.Serializable]
    private class HeatmapDataWrapper
    {
        public HeatmapData[] data;
    }

    [System.Serializable]
    private class HeatmapData
    {
        public string Position;
        public int Count;
    }
}

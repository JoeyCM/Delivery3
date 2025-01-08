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
    [SerializeField] private Gradient colorGradient; // Gradient for heatmap colors

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

        // Find the max count for color scaling
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

            // Scale the cube based on count
            cube.transform.localScale = Vector3.one * (0.5f + data.Count / (float)maxCount);

            // Set the cube's color based on count
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = colorGradient.Evaluate(data.Count / (float)maxCount);
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
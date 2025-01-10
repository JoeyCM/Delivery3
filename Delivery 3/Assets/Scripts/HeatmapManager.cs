using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HeatmapManager : MonoBehaviour
{
    [SerializeField] private string serverUrl;
    [SerializeField] private string eventType; // e.g., "OnDeath" or "OnReceiveDamage"
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform parentTransform;

    [SerializeField] private float baseCubeSize = 1.0f;
    private float cubeSizeMultiplier = 0.5f;
    private Color cubeColor = Color.red;
    private Dictionary<Vector3, int> heatmapDataDensity = new Dictionary<Vector3, int>();
    private List<GameObject> generatedCubes = new List<GameObject>();

    public bool IsVisible { get; set; } = true;

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
                Debug.Log($"Heatmap data received for {eventType}.");
                List<HeatmapData> heatmapData = ParseHeatmapData(request.downloadHandler.text);
                AggregateHeatmapData(heatmapData);
                GenerateHeatmap();
            }
            else
            {
                Debug.LogError($"Failed to fetch heatmap data for {eventType}: {request.error}");
            }
        }
    }

    private List<HeatmapData> ParseHeatmapData(string json)
    {
        HeatmapData[] dataArray = JsonUtility.FromJson<HeatmapDataWrapper>($"{{\"data\":{json}}}").data;
        return new List<HeatmapData>(dataArray);
    }

    private void AggregateHeatmapData(List<HeatmapData> heatmapData)
    {
        heatmapDataDensity.Clear();

        foreach (var data in heatmapData)
        {
            Vector3 position = ParsePosition(data.Position);
            Vector3 roundedPosition = new Vector3(
                Mathf.Round(position.x),
                Mathf.Round(position.y),
                Mathf.Round(position.z)
            );

            if (heatmapDataDensity.ContainsKey(roundedPosition))
            {
                heatmapDataDensity[roundedPosition] += data.Count;
            }
            else
            {
                heatmapDataDensity[roundedPosition] = data.Count;
            }
        }
    }

    private void GenerateHeatmap()
    {
        ClearHeatmap();

        int maxCount = 0;
        foreach (var count in heatmapDataDensity.Values)
        {
            maxCount = Mathf.Max(maxCount, count);
        }

        foreach (var entry in heatmapDataDensity)
        {
            Vector3 position = entry.Key;
            int count = entry.Value;

            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, parentTransform);
            generatedCubes.Add(cube);

            float size = Mathf.Clamp(baseCubeSize + (count / (float)maxCount), baseCubeSize, 1.3f * baseCubeSize) * cubeSizeMultiplier;
            cube.transform.localScale = Vector3.one * size;


            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.Lerp(Color.green, Color.red, count / (float)maxCount);
            }

            cube.SetActive(IsVisible);
        }
    }

    public void ClearHeatmap()
    {
        foreach (var cube in generatedCubes)
        {
            Destroy(cube);
        }
        generatedCubes.Clear();
    }

    private Vector3 ParsePosition(string positionString)
    {
        positionString = positionString.Trim('(', ')');
        string[] parts = positionString.Split(',');
        return new Vector3(
            float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
            float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
            float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture)
        );
    }

    public void SetCubeSizeMultiplier(float value)
    {
        cubeSizeMultiplier = value;
        UpdateCubeSizes();
    }

    private void UpdateCubeSizes()
    {
        int maxCount = 0;
        foreach (var count in heatmapDataDensity.Values)
        {
            maxCount = Mathf.Max(maxCount, count);
        }

        foreach (var cube in generatedCubes)
        {
            Vector3 position = cube.transform.position;
            if (heatmapDataDensity.TryGetValue(position, out int count))
            {
                float size = Mathf.Clamp(baseCubeSize + (count / (float)maxCount), baseCubeSize, 1.3f * baseCubeSize) * cubeSizeMultiplier;
                cube.transform.localScale = Vector3.one * size;

            }
        }
    }

    public void SetCubeVisibility(bool isVisible)
    {
        IsVisible = isVisible;

        foreach (var cube in generatedCubes)
        {
            if (cube != null)
            {
                cube.SetActive(IsVisible);
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

using UnityEditor;
using UnityEngine;

public class DataVisualizerWindow : EditorWindow
{
    private bool showHeatmap = true;
    private bool showPaths = false;
    private float gridCellSize = 1.0f;

    [MenuItem("Tools/Data Visualization/Data Visualizer")]
    public static void ShowWindow()
    {
        GetWindow<DataVisualizerWindow>("Data Visualizer");
    }

    void OnGUI()
    {
        GUILayout.Label("Visualization Settings", EditorStyles.boldLabel);

        showHeatmap = EditorGUILayout.Toggle("Show Heatmap", showHeatmap);
        showPaths = EditorGUILayout.Toggle("Show Player Paths", showPaths);

        gridCellSize = EditorGUILayout.FloatField("Grid Cell Size", gridCellSize);

        if (GUILayout.Button("Refresh Data"))
        {
            // Reload or update data
            Debug.Log("Data refreshed!");
        }
    }
}

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

// Parse imported data to retrieve player position logs over time.
// Use Unity’s Handles to draw lines representing player movement paths

[ExecuteInEditMode]
public class PathVisualizer : MonoBehaviour
{
    public List<Vector3> playerPath = new List<Vector3>(); // Player positions

    void OnDrawGizmos()
    {
        if (playerPath == null || playerPath.Count < 2)
            return;

        Handles.color = Color.green;
        for (int i = 0; i < playerPath.Count - 1; i++)
        {
            Handles.DrawLine(playerPath[i], playerPath[i + 1]);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraManagerTMP : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown cameraDropdown; // Reference to the TextMeshPro Dropdown
    public TMP_Text currentCameraLabel; // Reference to the TMP_Text for displaying the current camera name
    public GameObject specialActionObject; // The GameObject that will only appear when index is 1

    [Header("Cameras")]
    public List<Camera> cameras = new List<Camera>(); // Manually assign cameras in the Inspector

    void Start()
    {
        // Validate assignments
        if (cameraDropdown == null)
        {
            Debug.LogError("Camera Dropdown is not assigned.");
            return;
        }

        if (currentCameraLabel == null)
        {
            Debug.LogError("Current Camera Label is not assigned.");
            return;
        }

        if (specialActionObject == null)
        {
            Debug.LogError("Special Action Object is not assigned.");
            return;
        }

        if (cameras == null || cameras.Count == 0)
        {
            Debug.LogError("No cameras assigned to the script.");
            return;
        }

        // Populate the dropdown and set the first camera active
        PopulateDropdown();
        ActivateCamera(0);

        // Subscribe to the dropdown's value change event
        cameraDropdown.onValueChanged.AddListener(OnCameraSelected);

        // Initially hide the special action object
        specialActionObject.SetActive(false);
    }

    // Populate the dropdown with camera GameObject names
    void PopulateDropdown()
    {
        List<string> cameraNames = new List<string>();

        foreach (Camera cam in cameras)
        {
            if (cam != null)
            {
                cameraNames.Add(cam.gameObject.name); // Use the GameObject's name
            }
            else
            {
                Debug.LogWarning("A null camera is assigned in the list.");
            }
        }

        cameraDropdown.ClearOptions(); // Clear any existing options
        cameraDropdown.AddOptions(cameraNames); // Add the camera names to the dropdown
    }

    // Event handler for dropdown selection
    void OnCameraSelected(int index)
    {
        ActivateCamera(index);
    }

    // Activate the selected camera and update the label
    void ActivateCamera(int index)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i] != null)
            {
                cameras[i].gameObject.SetActive(i == index);
            }
        }

        string activeCameraName = cameras[index].gameObject.name;
        UpdateCurrentCameraLabel(activeCameraName);

        // Show or hide the special action object based on the current index
        specialActionObject.SetActive(index == 1);
    }

    // Update the TMP_Text with the current camera name
    void UpdateCurrentCameraLabel(string cameraName)
    {
        if (currentCameraLabel != null)
        {
            currentCameraLabel.text = $"Current Camera: {cameraName}";
        }
    }
}
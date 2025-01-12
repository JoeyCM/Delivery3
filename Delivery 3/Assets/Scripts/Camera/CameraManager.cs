using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraManagerTMP : MonoBehaviour
{
    [SerializeField] private List<Camera> cameras;
    [SerializeField] private TMP_Dropdown cameraDropdown;

    private void Start()
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(i == 0);
        }

        PopulateDropdown();

        cameraDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void PopulateDropdown()
    {
        List<string> cameraNames = new List<string>();

        foreach (Camera cam in cameras)
        {
            cameraNames.Add(cam.name);
        }

        cameraDropdown.ClearOptions();
        cameraDropdown.AddOptions(cameraNames);
    }

    private void OnDropdownValueChanged(int index)
    {
        foreach (Camera cam in cameras)
        {
            cam.gameObject.SetActive(false);
        }

        cameras[index].gameObject.SetActive(true);
    }
}

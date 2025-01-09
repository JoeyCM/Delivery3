using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Use the TextMesh Pro namespace
using System.Collections.Generic;  // Required for List<T> support
using UnityEngine.EventSystems; // For EventSystem to manage UI interactions

public class HeatmapSettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider cubeSizeSlider;
    public TMP_Dropdown colorDropdown;  // TMP_Dropdown for color selection

    public TextMeshProUGUI cubeSizeValueText;
    public TextMeshProUGUI colorValueText;  // Text to display the selected color

    public HeatmapManager heatmapManager; // Reference to the HeatmapManager script
    public CameraController cameraController; // Reference to the CameraController script

    private bool isPanelActive = false;

    void Start()
    {
        // Ensure the panel starts as inactive
        settingsPanel.SetActive(false);

        // Set default slider values to match HeatmapManager settings
        cubeSizeSlider.value = 1.0f; // Initial default value (can be adjusted)
        
        // Set color dropdown options (10 predefined colors)
        colorDropdown.ClearOptions();
        colorDropdown.AddOptions(new List<string> { "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan", "Black", "White", "Gray", "Orange" });

        // Update value texts to display initial values
        cubeSizeValueText.text = cubeSizeSlider.value.ToString("F2");
        colorValueText.text = colorDropdown.options[colorDropdown.value].text;

        // Add listeners to update HeatmapManager settings and value texts when sliders are changed
        cubeSizeSlider.onValueChanged.AddListener(UpdateCubeSize);
        colorDropdown.onValueChanged.AddListener(UpdateColor);

        // Set default color in HeatmapManager (Red by default)
        heatmapManager.SetCubeColor(Color.red);
    }

    void Update()
    {
        // If the settings panel is active, ignore WASD input by disabling the CameraController's input handling
        if (isPanelActive)
        {
            cameraController.DisableInput();
        }
        else
        {
            cameraController.EnableInput();
        }
    }

    public void ToggleSettingsPanel()
    {
        // Disable interaction via Submit (space/enter keys) for the settings panel button
        EventSystem.current.SetSelectedGameObject(null);

        isPanelActive = !isPanelActive;
        settingsPanel.SetActive(isPanelActive);

        // Disable or enable camera input based on panel visibility
        if (isPanelActive)
        {
            cameraController.DisableInput(); // Disable input when panel is visible
        }
        else
        {
            cameraController.EnableInput(); // Enable input when panel is hidden
        }
    }

    private void UpdateCubeSize(float value)
    {
        Debug.Log($"Cube Size Changed: {value}");  // Debug log to check if the listener is being triggered
        // Cap the size multiplier to 1.3 max
        value = Mathf.Clamp(value, 0.5f, 16.3f);
        heatmapManager.SetCubeSizeMultiplier(value);  // Update cube size multiplier in HeatmapManager
        cubeSizeValueText.text = value.ToString("F2");  // Update UI text
    }

    private void UpdateColor(int index)
    {
        Color selectedColor = GetColorFromDropdownIndex(index);
        heatmapManager.SetCubeColor(selectedColor);  // Update color in HeatmapManager
        colorValueText.text = colorDropdown.options[index].text;  // Update UI text
    }

    private Color GetColorFromDropdownIndex(int index)
    {
        // Return a color based on the dropdown selection
        switch (index)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.magenta;
            case 5: return Color.cyan;
            case 6: return Color.black;
            case 7: return Color.white;
            case 8: return Color.gray;
            case 9: return new Color(1f, 0.647f, 0f); // Orange
            default: return Color.white;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HeatmapSettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider cubeSizeSlider;
    public TextMeshProUGUI cubeSizeValueText;

    [Header("Heatmap Managers")]
    public HeatmapManager deathsHeatmapManager;   // HeatmapManager for deaths
    public HeatmapManager damageHeatmapManager;  // HeatmapManager for damage

    [Header("Toggle Visibility")]
    public Toggle deathsToggle;  // Toggle for deaths cubes
    public Toggle damageToggle;  // Toggle for damage cubes

    public CameraController cameraController;

    private bool isPanelActive = false;

    void Start()
    {
        settingsPanel.SetActive(false);

        // Set slider initial value
        cubeSizeSlider.value = 0.5f;
        cubeSizeValueText.text = cubeSizeSlider.value.ToString("F2");

        // Add listeners to UI elements
        cubeSizeSlider.onValueChanged.AddListener(UpdateCubeSize);
        deathsToggle.onValueChanged.AddListener(ToggleDeathsVisibility);
        damageToggle.onValueChanged.AddListener(ToggleDamageVisibility);

        // Set initial cube sizes
        deathsHeatmapManager.SetCubeSizeMultiplier(0.5f);
        damageHeatmapManager.SetCubeSizeMultiplier(0.5f);
    }

    void Update()
    {
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
        EventSystem.current.SetSelectedGameObject(null);

        isPanelActive = !isPanelActive;
        settingsPanel.SetActive(isPanelActive);

        if (isPanelActive)
        {
            cameraController.DisableInput();
        }
        else
        {
            cameraController.EnableInput();
        }
    }

    private void UpdateCubeSize(float value)
    {
        Debug.Log($"Cube Size Multiplier Changed: {value}");

        // Update cube sizes for both heatmap managers
        deathsHeatmapManager?.SetCubeSizeMultiplier(value);
        damageHeatmapManager?.SetCubeSizeMultiplier(value);

        // Update the slider value text
        cubeSizeValueText.text = value.ToString("F2");
    }

    private void ToggleDeathsVisibility(bool isVisible)
    {
        Debug.Log($"Toggle Deaths Visibility: {isVisible}");
        deathsHeatmapManager?.SetCubeVisibility(isVisible);
    }

    private void ToggleDamageVisibility(bool isVisible)
    {
        Debug.Log($"Toggle Damage Visibility: {isVisible}");
        damageHeatmapManager?.SetCubeVisibility(isVisible);
    }
}
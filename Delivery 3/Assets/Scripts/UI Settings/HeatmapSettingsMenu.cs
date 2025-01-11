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
    public HeatmapManager playerDeathsHeatmapManager;   // HeatmapManager for deaths
    public HeatmapManager playerDamageHeatmapManager;  // HeatmapManager for damage
    public HeatmapManager enemiesDeathsHeatmapManager;  // HeatmapManager for damage

    [Header("Toggle Visibility")]
    public Toggle playerDeathsToggle;  // Toggle for deaths cubes
    public Toggle playerDamageToggle;  // Toggle for damage cubes
    public Toggle enemiesDeathsToggle;  // Toggle for damage cubes

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
        playerDeathsToggle.onValueChanged.AddListener(TogglePlayerDeathsVisibility);
        playerDamageToggle.onValueChanged.AddListener(TogglePlayerDamageVisibility);
        enemiesDeathsToggle.onValueChanged.AddListener(ToggleEnemiesDeathsVisibility);

        // Set initial cube sizes
        playerDeathsHeatmapManager.SetCubeSizeMultiplier(0.5f);
        playerDamageHeatmapManager.SetCubeSizeMultiplier(0.5f);
        enemiesDeathsHeatmapManager.SetCubeSizeMultiplier(0.5f);
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
        playerDeathsHeatmapManager?.SetCubeSizeMultiplier(value);
        playerDamageHeatmapManager?.SetCubeSizeMultiplier(value);
        enemiesDeathsHeatmapManager?.SetCubeSizeMultiplier(value);

        // Update the slider value text
        cubeSizeValueText.text = value.ToString("F2");
    }

    private void TogglePlayerDeathsVisibility(bool isVisible)
    {
        Debug.Log($"Toggle Player Deaths Visibility: {isVisible}");
        playerDeathsHeatmapManager?.SetCubeVisibility(isVisible);
    }

    private void TogglePlayerDamageVisibility(bool isVisible)
    {
        Debug.Log($"Toggle Player Damage Visibility: {isVisible}");
        playerDamageHeatmapManager?.SetCubeVisibility(isVisible);
    }

    private void ToggleEnemiesDeathsVisibility(bool isVisible)
    {
        Debug.Log($"Toggle Enemies Deaths Visibility: {isVisible}");
        enemiesDeathsHeatmapManager?.SetCubeVisibility(isVisible);
    }
}
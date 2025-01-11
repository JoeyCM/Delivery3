using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro
using UnityEngine.EventSystems; // For EventSystem to manage UI interactions

public class CameraSettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel; // Reference to the settings panel
    public Slider sensitivitySlider;
    public Slider movementSpeedSlider;
    public Slider verticalSpeedSlider;

    public TextMeshProUGUI sensitivityValueText; // Text to display sensitivity value
    public TextMeshProUGUI movementSpeedValueText; // Text to display movement speed value
    public TextMeshProUGUI verticalSpeedValueText; // Text to display vertical speed value

    public CameraController cameraController; // Reference to the CameraController script

    private bool isPanelActive = false; // Track whether the panel is active or not

    void Start()
    {
        // Ensure the panel starts as inactive
        settingsPanel.SetActive(false);

        // Configure slider ranges
        sensitivitySlider.minValue = 400f;
        sensitivitySlider.maxValue = 1400f;
        movementSpeedSlider.minValue = 5f;
        movementSpeedSlider.maxValue = 15f;
        verticalSpeedSlider.minValue = 0f; // Assuming a range for vertical speed
        verticalSpeedSlider.maxValue = 10f;

        // Set default slider values to match CameraController settings
        sensitivitySlider.value = cameraController.mouseSensitivity;
        movementSpeedSlider.value = cameraController.movementSpeed;
        verticalSpeedSlider.value = cameraController.verticalSpeed;

        // Update value texts to display initial values
        sensitivityValueText.text = cameraController.mouseSensitivity.ToString("F1");
        movementSpeedValueText.text = cameraController.movementSpeed.ToString("F1");
        verticalSpeedValueText.text = cameraController.verticalSpeed.ToString("F1");

        // Add listeners to update CameraController settings and value texts when sliders are changed
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        movementSpeedSlider.onValueChanged.AddListener(UpdateMovementSpeed);
        verticalSpeedSlider.onValueChanged.AddListener(UpdateVerticalSpeed);
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

    private void UpdateSensitivity(float value)
    {
        cameraController.mouseSensitivity = value;
        sensitivityValueText.text = value.ToString("F1"); // Update text to show new value
    }

    private void UpdateMovementSpeed(float value)
    {
        cameraController.movementSpeed = value;
        movementSpeedValueText.text = value.ToString("F1"); // Update text to show new value
    }

    private void UpdateVerticalSpeed(float value)
    {
        cameraController.verticalSpeed = value;
        verticalSpeedValueText.text = value.ToString("F1"); // Update text to show new value
    }
}
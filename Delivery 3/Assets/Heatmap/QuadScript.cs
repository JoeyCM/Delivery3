using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Ensure you include the TextMeshPro namespace

public class QuadScript : MonoBehaviour
{
    Material mMaterial;
    MeshRenderer mMeshRenderer;

    float[] mPoints;
    int mHitCount;

    float mDelay;

    [SerializeField] private List<Toggle> checkboxes; 
    [SerializeField] private List<HeatmapManager> heatmapManagers;

    private List<HeatmapManager> activeHeatmapManagers = new List<HeatmapManager>();
    private List<GameObject> spawnedParticles = new List<GameObject>();

    // Materials
    [SerializeField] private Material defaultMaterial; // Default material
    [SerializeField] private Material heatmapMaterial; // Heatmap material

    // UI sliders for controlling shader values
    [SerializeField] private Slider diameterSlider;
    [SerializeField] private Slider strengthSlider;
    [SerializeField] private Slider pulseSpeedSlider;

    // UI TextMeshPro texts for displaying slider values
    [SerializeField] private TextMeshProUGUI diameterText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI pulseSpeedText;

    void Start()
    {
        mDelay = 3;

        mMeshRenderer = GetComponent<MeshRenderer>();
        mMaterial = mMeshRenderer.material; // Retrieve the material already assigned to the GameObject

        mPoints = new float[256 * 3]; // 256 points 

        if (heatmapManagers == null || heatmapManagers.Count == 0)
        {
            Debug.LogError("No HeatmapManager instances assigned.");
        }

        if (checkboxes == null || checkboxes.Count != heatmapManagers.Count)
        {
            Debug.LogError("Ensure the number of checkboxes matches the number of HeatmapManager instances.");
        }
        else
        {
            // Set all checkboxes to off at the start
            foreach (var toggle in checkboxes)
            {
                if (toggle != null)
                {
                    toggle.isOn = false; // Ensure all checkboxes start unchecked
                }
            }

            AssignCheckboxListeners();
        }

        // Start with a different material (default material)
        mMaterial = defaultMaterial;
        mMeshRenderer.material = mMaterial;

        // Call RefreshMaterial at the start to reset the material and hits
        RefreshMaterial();

        // Initialize sliders with their ranges and values
        InitializeSliders();
    }

    void Update()
    {
        mDelay -= Time.deltaTime;
        if (mDelay <= 0)
        {
            mDelay = 0.8f;
        }
    }

    private void InitializeSliders()
    {
        // Set slider ranges for diameter (0 to 10), strength (0.1 to 50), and pulse speed (0 to 20)
        if (diameterSlider != null)
        {
            diameterSlider.minValue = 0f;
            diameterSlider.maxValue = 10f;
            diameterSlider.onValueChanged.AddListener(UpdateShaderDiameter);
            diameterSlider.value = mMaterial.GetFloat("_Diameter"); // Set initial value
            UpdateDiameterText(diameterSlider.value); // Update the text immediately
        }

        if (strengthSlider != null)
        {
            strengthSlider.minValue = 0.1f;
            strengthSlider.maxValue = 50f;
            strengthSlider.onValueChanged.AddListener(UpdateShaderStrength);
            strengthSlider.value = mMaterial.GetFloat("_Strength"); // Set initial value
            UpdateStrengthText(strengthSlider.value); // Update the text immediately
        }

        if (pulseSpeedSlider != null)
        {
            pulseSpeedSlider.minValue = 0f;
            pulseSpeedSlider.maxValue = 20f;
            pulseSpeedSlider.onValueChanged.AddListener(UpdateShaderPulseSpeed);
            pulseSpeedSlider.value = mMaterial.GetFloat("_PulseSpeed"); // Set initial value
            UpdatePulseSpeedText(pulseSpeedSlider.value); // Update the text immediately
        }
    }

    private void UpdateShaderDiameter(float value)
    {
        // Update the shader's _Diameter property
        mMaterial.SetFloat("_Diameter", value);
        UpdateDiameterText(value);  // Update the TextMeshPro UI text
    }

    private void UpdateShaderStrength(float value)
    {
        // Update the shader's _Strength property
        mMaterial.SetFloat("_Strength", value);
        UpdateStrengthText(value);  // Update the TextMeshPro UI text
    }

    private void UpdateShaderPulseSpeed(float value)
    {
        // Update the shader's _PulseSpeed property
        mMaterial.SetFloat("_PulseSpeed", value);
        UpdatePulseSpeedText(value);  // Update the TextMeshPro UI text
    }

    // Update the TextMeshPro Text elements for each slider's value
    private void UpdateDiameterText(float value)
    {
        if (diameterText != null)
        {
            diameterText.text = "Diameter: " + value.ToString("F2");  // Format to 2 decimal places
        }
    }

    private void UpdateStrengthText(float value)
    {
        if (strengthText != null)
        {
            strengthText.text = "Strength: " + value.ToString("F2");  // Format to 2 decimal places
        }
    }

    private void UpdatePulseSpeedText(float value)
    {
        if (pulseSpeedText != null)
        {
            pulseSpeedText.text = "Pulse Speed: " + value.ToString("F2");  // Format to 2 decimal places
        }
    }

    private void AssignCheckboxListeners()
    {
        for (int i = 0; i < checkboxes.Count; i++)
        {
            int index = i; 
            if (checkboxes[i] != null)
            {
                checkboxes[i].onValueChanged.AddListener(isChecked =>
                {
                    ToggleHeatmapManager(index, isChecked);
                });
            }
        }
    }

    private void ToggleHeatmapManager(int index, bool isChecked)
    {
        if (index >= 0 && index < heatmapManagers.Count)
        {
            if (isChecked)
            {
                // Add the HeatmapManager to the active list if it's checked
                if (!activeHeatmapManagers.Contains(heatmapManagers[index]))
                {
                    activeHeatmapManagers.Add(heatmapManagers[index]);
                    SpawnHeatmapParticles(heatmapManagers[index]);

                    // Refresh the material to reset hits and material settings
                    RefreshMaterial();
                }
            }
            else
            {
                // Remove the HeatmapManager from the active list if it's unchecked
                activeHeatmapManagers.Remove(heatmapManagers[index]);

                // If no heatmaps are active, reset the material
                if (activeHeatmapManagers.Count == 0)
                {
                    RefreshMaterial();
                }
            }
        }
        else
        {
            Debug.LogError("Invalid HeatmapManager index.");
        }
    }

    private void RefreshMaterial()
    {
        // Reset the material's _HitCount and _Hits array
        mHitCount = 0; // Reset hit count
        mMaterial.SetInt("_HitCount", mHitCount); // Apply reset to _HitCount
        
        // Ensure _Hits array is cleared
        float[] emptyHits = new float[256 * 3]; // Clear the hit array
        mMaterial.SetFloatArray("_Hits", emptyHits);
        
        // Reset shader values to defaults
        mMaterial.SetFloat("_Diameter", 1.0f); 
        mMaterial.SetFloat("_Strength", 1.0f);
        mMaterial.SetFloat("_PulseSpeed", 0.0f);
    }

    private void SpawnHeatmapParticles(HeatmapManager manager)
    {
        if (manager != null)
        {
            List<Vector3> heatmapPositions = manager.GetHeatmapPositions();

            foreach (var position in heatmapPositions)
            {
                // Instantiate the projectile at the position with Y adjusted by +1
                GameObject go = Instantiate(Resources.Load<GameObject>("Projectile"));
                Vector3 adjustedPosition = position;
                adjustedPosition.y += 1f; // Adjust the Y position by 1
                go.transform.position = adjustedPosition; // Use adjusted position

                // Store the particle to avoid re-spawning
                spawnedParticles.Add(go);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint cp in collision.contacts)
        {
            Vector3 StartOfRay = cp.point - cp.normal;
            Vector3 RayDir = cp.normal;

            Ray ray = new Ray(StartOfRay, RayDir);
            RaycastHit hit;

            bool hitit = Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("HeatMapLayer"));

            if (hitit)
            {
                addHitPoint(hit.textureCoord.x * 4 - 2, hit.textureCoord.y * 4 - 2);
            }
            Destroy(cp.otherCollider.gameObject);
        }
    }

    public void addHitPoint(float xp, float yp)
    {
        mPoints[mHitCount * 3] = xp;
        mPoints[mHitCount * 3 + 1] = yp;
        mPoints[mHitCount * 3 + 2] = Random.Range(1f, 3f);

        mHitCount++;
        mHitCount %= 256;

        mMaterial.SetFloatArray("_Hits", mPoints);
        mMaterial.SetInt("_HitCount", mHitCount);
    }

    // Method to switch to heatmap material when called
    public void HeatmapEntry()
    {
        // Switch material to the heatmap material
        mMaterial = heatmapMaterial;
        mMeshRenderer.material = mMaterial;

        // Reset the material for heatmap
        RefreshMaterial();
    }

    // Method to switch to heatmap material when called
    public void HeatmapExit()
    {
        // Switch material to the default material
        mMaterial = defaultMaterial;
        mMeshRenderer.material = mMaterial;
    }
}

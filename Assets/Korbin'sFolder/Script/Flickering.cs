using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    // Reference to the Light component
    private Light lightComponent;

    // The range for light intensity
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;

    // Time interval between flickers
    public float flickerSpeed = 0.1f;

    // Time between intensity changes
    private float nextFlickerTime = 0f;

    void Start()
    {
        // Get the Light component attached to this GameObject
        lightComponent = GetComponent<Light>();

        // If there is no light component, log an error
        if (lightComponent == null)
        {
            Debug.LogError("No Light component found on this object.");
        }
    }

    void Update()
    {
        // If the flicker time has passed, change the intensity
        if (Time.time >= nextFlickerTime)
        {
            // Randomize the light intensity within the specified range
            lightComponent.intensity = Random.Range(minIntensity, maxIntensity);

            // Set the next flicker time
            nextFlickerTime = Time.time + flickerSpeed;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlickerModule : MonoBehaviour
{
    public enum FlickerMode
    {
        ToggleEnabled,     // Toggles light.enabled on and off
        ModulateIntensity  // Keeps light on, but randomizes intensity
    }

    [Header("Flicker Setup")]
    public FlickerMode mode = FlickerMode.ToggleEnabled;

    [Tooltip("Assign all the lights you want this module to control.")]
    public List<GameObject> Spotlights;

    [Header("Flicker Timing (Random range)")]
    public float minTime = 0.05f;
    public float maxTime = 0.2f;

    [Header("Intensity Mode Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.0f;

    private List<Light> lightsToControl = new List<Light>();
    private List<float> originalIntensities = new List<float>();
    private Coroutine flickerCoroutine;
    private bool isFlickering = false;

    void Awake()
    {
        lightsToControl.Clear();
        originalIntensities.Clear();

        foreach (GameObject go in Spotlights)
        {
            if (go != null)
            {
                Light light = go.GetComponentInChildren<Light>();
                if (light != null)
                {
                    lightsToControl.Add(light);
                    originalIntensities.Add(light.intensity);
                }
            }
        }
    }


    // --- PUBLIC COMMANDS ---
    // Your "Choreographer" script will call these functions

    public void StartFlicker()
    {
        if (isFlickering) return; // Don't start if already running

        isFlickering = true;
        flickerCoroutine = StartCoroutine(DoFlicker());
    }

    public void StopFlicker()
    {
        if (!isFlickering) return; // Don't stop if not running

        isFlickering = false;
        StopCoroutine(flickerCoroutine);
        RestoreLights();
    }


    // --- THE FLICKER LOGIC ---

    IEnumerator DoFlicker()
    {
        // This loop will run until StopFlicker() is called
        while (isFlickering)
        {
            // Apply flicker to every light in the list
            foreach (Light light in lightsToControl)
            {
                if (mode == FlickerMode.ToggleEnabled)
                {
                    // Simple on/off toggle
                    light.enabled = !light.enabled;
                }
                else // Mode is ModulateIntensity
                {
                    // Ensure light is on, then change brightness
                    light.enabled = true;
                    light.intensity = Random.Range(minIntensity, maxIntensity);
                }
            }

            // Wait for a random amount of time before the next flicker
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    // Restores all lights to their original, pre-flicker state
    private void RestoreLights()
    {
        for (int i = 0; i < lightsToControl.Count; i++)
        {
            if (lightsToControl[i] != null)
            {
                lightsToControl[i].enabled = true;
                lightsToControl[i].intensity = originalIntensities[i];
            }
        }
    }
}
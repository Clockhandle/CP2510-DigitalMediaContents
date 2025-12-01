using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlickerModule : MonoBehaviour
{
    public enum FlickerMode
    {
        ToggleEnabled,      // Toggles light.enabled on and off
        ModulateIntensity   // Keeps light on, but randomizes intensity
    }

    [Header("Flicker Setup")]
    public FlickerMode mode = FlickerMode.ToggleEnabled;

    [Tooltip("Drag the Parent GameObject (LightSet) here to auto-fill the list.")]
    public Transform lightSetParent;

    [Tooltip("Assign lights here manually, OR leave empty and use lightSetParent above.")]
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
        // If the list is empty BUT we assigned a parent, grab the children!
        if (Spotlights.Count == 0 && lightSetParent != null)
        {
            foreach (Transform child in lightSetParent)
            {
                Spotlights.Add(child.gameObject);
            }
        }

        // --- ORIGINAL SETUP ---
        lightsToControl.Clear();
        originalIntensities.Clear();

        foreach (GameObject go in Spotlights)
        {
            if (go != null)
            {
                // We use GetComponentInChildren just in case the Light component 
                // is slightly nested, or on the object itself.
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

    public void StartFlicker()
    {
        if (isFlickering) return;

        isFlickering = true;
        flickerCoroutine = StartCoroutine(DoFlicker());
    }

    public void StopFlicker()
    {
        if (!isFlickering) return;

        isFlickering = false;
        if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
        RestoreLights();
    }


    // --- THE FLICKER LOGIC ---

    IEnumerator DoFlicker()
    {
        while (isFlickering)
        {
            foreach (Light light in lightsToControl)
            {
                if (mode == FlickerMode.ToggleEnabled)
                {
                    light.enabled = !light.enabled;
                }
                else // Mode is ModulateIntensity
                {
                    light.enabled = true;
                    light.intensity = Random.Range(minIntensity, maxIntensity);
                }
            }

            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void RestoreLights()
    {
        for (int i = 0; i < lightsToControl.Count; i++)
        {
            if (lightsToControl[i] != null)
            {
                lightsToControl[i].enabled = true;
                // Only restore intensity if we actually stored it
                if (i < originalIntensities.Count)
                {
                    lightsToControl[i].intensity = originalIntensities[i];
                }
            }
        }
    }
}
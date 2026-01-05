using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightToggleModule : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Drag the Parent GameObject (LightSet) here.")]
    public Transform lightSetParent;
    public List<GameObject> lightsToControl;

    [Header("Pulse Settings")]
    public float pulseDuration = 0.1f; // How long the flash lasts (0.1 is snappy)
    // We cache the actual Light components for performance
    private List<Light> targetLights = new List<Light>();

    void Start()
    {
        // --- AUTO-POPULATE ---
        if (lightsToControl.Count == 0 && lightSetParent != null)
        {
            foreach (Transform child in lightSetParent)
            {
                lightsToControl.Add(child.gameObject);
            }
        }

        // Find the actual Light component inside the prefabs
        foreach (GameObject go in lightsToControl)
        {
            if (go != null)
            {
                // GetComponentInChildren finds the light even if it's nested deep in the prefab
                Light l = go.GetComponentInChildren<Light>();
                if (l != null) targetLights.Add(l);
            }
        }
    }

    // --- PUBLIC COMMANDS (Drag to Choreographer) ---

    public void TurnOn()
    {
        foreach (Light l in targetLights)
        {
            if (l != null) l.enabled = true;
        }
    }

    public void TurnOff()
    {
        foreach (Light l in targetLights)
        {
            if (l != null) l.enabled = false;
        }
    }

    public void Toggle()
    {
        foreach (Light l in targetLights)
        {
            if (l != null) l.enabled = !l.enabled;
        }
    }

    public void Pulse()
    {
        StartCoroutine(DoPulse());
    }

    private IEnumerator DoPulse()
    {
        // 1. Turn On immediately
        foreach (Light l in targetLights)
        {
            if (l != null) l.enabled = true;
        }

        // 2. Wait for a fraction of a second
        yield return new WaitForSeconds(pulseDuration);

        // 3. Turn Off
        foreach (Light l in targetLights)
        {
            if (l != null) l.enabled = false;
        }
    }
}
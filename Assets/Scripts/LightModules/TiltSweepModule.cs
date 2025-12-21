using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TiltSweepModule : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Drag the Parent GameObject (LightSet) here to auto-fill the list.")]
    public Transform lightSetParent;

    [Tooltip("Add all lights manually here, OR leave empty and use lightSetParent above.")]
    public List<GameObject> lightsToAnimate;

    [Header("Animation Settings")]
    public float angle_Down = 15.0f;
    public float angle_Up = -45.0f;

    [Header("Timing (in Seconds)")]
    public float timeToDown = 0.5f;
    public float timeToUp = 1.5f;
    public float timeToHome = 1.0f;

    [Tooltip("The delay between each light starting its sweep, creating a 'wave'")]
    public float waveDelay = 0.1f;

    // We need to store the "home" rotation for every light
    private List<Quaternion> homeRotations = new List<Quaternion>();
    private bool isSweeping = false;

    void Start()
    {
        // --- NEW AUTO-POPULATE LOGIC ---
        // If list is empty but parent is assigned, grab children
        if (lightsToAnimate.Count == 0 && lightSetParent != null)
        {
            foreach (Transform child in lightSetParent)
            {
                lightsToAnimate.Add(child.gameObject);
            }
        }

        // --- ORIGINAL SETUP ---
        // Store the "home" rotation for every light in our list
        foreach (GameObject light in lightsToAnimate)
        {
            if (light != null)
            {
                homeRotations.Add(light.transform.localRotation);
            }
        }
    }

    // --- PUBLIC COMMAND ---
    public void StartSweepWave()
    {
        if (isSweeping) return;

        StartCoroutine(RunSweepWave());
    }

    // --- THE LOGIC (Unchanged) ---
    IEnumerator RunSweepWave()
    {
        isSweeping = true;

        for (int i = 0; i < lightsToAnimate.Count; i++)
        {
            if (lightsToAnimate[i] != null)
            {
                // Start the animation for just this one light
                StartCoroutine(DoSingleLightSweep(lightsToAnimate[i], homeRotations[i]));

                // Wait for the wave delay before starting the next light
                yield return new WaitForSeconds(waveDelay);
            }
        }

        // Note: We don't set isSweeping = false immediately because coroutines are still running.
        // But for a simple trigger, this is usually fine. 
        // If you want to trigger it again immediately, you might want to wait here.
        yield return new WaitForSeconds(timeToDown + timeToUp + timeToHome);
        isSweeping = false;
    }

    IEnumerator DoSingleLightSweep(GameObject light, Quaternion homeRot)
    {
        // Pre-calculate target rotations for this specific light
        Quaternion downRot = homeRot * Quaternion.Euler(angle_Down, 0, 0);
        Quaternion upRot = homeRot * Quaternion.Euler(angle_Up, 0, 0);

        Transform lightTransform = light.transform;
        float timer;

        // Sequence 1: Move to "Down"
        timer = 0.0f;
        while (timer < timeToDown)
        {
            lightTransform.localRotation = Quaternion.Slerp(homeRot, downRot, timer / timeToDown);
            timer += Time.deltaTime;
            yield return null;
        }

        // Sequence 2: Move to "Up"
        timer = 0.0f;
        while (timer < timeToUp)
        {
            lightTransform.localRotation = Quaternion.Slerp(downRot, upRot, timer / timeToUp);
            timer += Time.deltaTime;
            yield return null;
        }

        // Sequence 3: Return home
        timer = 0.0f;
        while (timer < timeToHome)
        {
            lightTransform.localRotation = Quaternion.Slerp(upRot, homeRot, timer / timeToHome);
            timer += Time.deltaTime;
            yield return null;
        }

        lightTransform.localRotation = homeRot; // Snap to final position
    }
}
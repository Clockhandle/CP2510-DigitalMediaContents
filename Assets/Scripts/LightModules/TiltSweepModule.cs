using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TiltSweepModule : MonoBehaviour
{
    [Tooltip("Add all lights this module should control (e.g., all 17 spotlights)")]
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
        // Store the "home" rotation for every light in our list
        foreach (GameObject light in lightsToAnimate)
        {
            homeRotations.Add(light.transform.localRotation);
        }
    }

    // --- PUBLIC COMMAND ---
    // The LightChoreographer will call this
    public void StartSweepWave()
    {
        if (isSweeping) return;

        // This *starts* the master coroutine that will trigger all the lights
        StartCoroutine(RunSweepWave());
    }

    // This "master" coroutine loops through the lights and starts
    // an *individual* animation coroutine for each one, after a delay.
    IEnumerator RunSweepWave()
    {
        isSweeping = true;

        for (int i = 0; i < lightsToAnimate.Count; i++)
        {
            // Start the animation for just this one light
            StartCoroutine(DoSingleLightSweep(lightsToAnimate[i], homeRotations[i]));

            // Wait for the wave delay before starting the next light
            yield return new WaitForSeconds(waveDelay);
        }

        isSweeping = false;
    }

    // This is the actual animation logic for one light
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
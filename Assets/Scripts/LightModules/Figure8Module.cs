using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Figure8Module : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Drag the Parent GameObject (LightSet) here. The script will grab all children automatically.")]
    public Transform lightSetParent;

    // We keep this list, but now the script fills it for you!
    // You can also leave 'lightSetParent' empty and fill this manually if you prefer.
    public List<GameObject> lightsToAnimate;

    [Header("Shape Settings")]
    public float widthAngle = 30.0f;
    public float heightAngle = 15.0f;

    [Header("Timing")]
    public float loopSpeed = 2.0f;
    public float waveDelay = 0.2f;
    public float returnHomeTime = 1.0f;

    private List<Quaternion> homeRotations = new List<Quaternion>();
    private bool isRunning = false;

    void Start()
    {
        // --- NEW AUTO-POPULATE LOGIC ---
        // If the list is empty BUT we assigned a parent, grab the children!
        if (lightsToAnimate.Count == 0 && lightSetParent != null)
        {
            foreach (Transform child in lightSetParent)
            {
                lightsToAnimate.Add(child.gameObject);
            }
        }

        // --- STANDARD SETUP ---
        // Capture home positions for whatever is in the list
        foreach (GameObject light in lightsToAnimate)
        {
            if (light != null)
                homeRotations.Add(light.transform.localRotation);
        }
    }

    // --- COMMANDS (Same as before) ---
    public void StartFigure8()
    {
        if (isRunning) return;
        StartCoroutine(RunMasterSequence());
    }

    public void StopFigure8()
    {
        isRunning = false;
    }

    // --- LOGIC (Exactly the same) ---
    IEnumerator RunMasterSequence()
    {
        isRunning = true;

        for (int i = 0; i < lightsToAnimate.Count; i++)
        {
            if (!isRunning) yield break;

            if (lightsToAnimate[i] != null)
            {
                StartCoroutine(AnimateSingleLight(lightsToAnimate[i], homeRotations[i]));
                yield return new WaitForSeconds(waveDelay);
            }
        }
    }

    IEnumerator AnimateSingleLight(GameObject light, Quaternion homeRot)
    {
        float timer = 0.0f;
        Transform t = light.transform;

        while (isRunning)
        {
            float yaw = Mathf.Sin(timer * loopSpeed) * widthAngle;
            float pitch = Mathf.Sin(timer * loopSpeed * 2.0f) * heightAngle;

            t.localRotation = homeRot * Quaternion.Euler(pitch, yaw, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        float returnTimer = 0.0f;
        Quaternion currentRot = t.localRotation;

        while (returnTimer < returnHomeTime)
        {
            float percentage = returnTimer / returnHomeTime;
            t.localRotation = Quaternion.Slerp(currentRot, homeRot, percentage);
            returnTimer += Time.deltaTime;
            yield return null;
        }

        t.localRotation = homeRot;
    }
}
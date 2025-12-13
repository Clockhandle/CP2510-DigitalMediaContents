using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FanOutModule : MonoBehaviour
{
    [Header("Setup")]
    public Transform lightSetParent;
    public List<GameObject> lightsToAnimate;

    [Header("Fan Settings")]
    [Tooltip("The total angle of the fan spread (e.g., 90 degrees).")]
    public float totalFanAngle = 90.0f;

    [Tooltip("Which axis to fan out on? (Usually Y for horizontal spread, X for vertical)")]
    public Vector3 fanAxis = Vector3.up;

    [Header("Timing")]
    public float openDuration = 1.0f;
    public float closeDuration = 1.0f;

    // Internal
    private List<Quaternion> homeRotations = new List<Quaternion>();
    private Coroutine activeCoroutine;

    void Start()
    {
        // Auto-Fetch
        if (lightsToAnimate.Count == 0 && lightSetParent != null)
            foreach (Transform child in lightSetParent) lightsToAnimate.Add(child.gameObject);

        // Store Homes
        foreach (GameObject go in lightsToAnimate)
            if (go != null) homeRotations.Add(go.transform.localRotation);
    }

    // --- COMMANDS ---

    public void OpenFan()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(AnimateFan(true));
    }

    public void CloseFan()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(AnimateFan(false));
    }

    // --- LOGIC ---

    IEnumerator AnimateFan(bool opening)
    {
        float timer = 0.0f;
        float duration = opening ? openDuration : closeDuration;
        int count = lightsToAnimate.Count;

        // Calculate the "Step" angle for each light
        // e.g. if spread is 90 and we have 9 lights, step is 10 degrees.
        float angleStep = totalFanAngle / (count - 1);
        float startAngle = -totalFanAngle / 2.0f; // Center the fan

        // Capture current rotations for smooth blending
        List<Quaternion> startRots = new List<Quaternion>();
        foreach (var l in lightsToAnimate) startRots.Add(l.transform.localRotation);

        while (timer < duration)
        {
            float t = timer / duration;
            t = Mathf.SmoothStep(0.0f, 1.0f, t); // Organic ease

            for (int i = 0; i < count; i++)
            {
                if (lightsToAnimate[i] != null)
                {
                    // Calculate target rotation for this specific light index
                    // If opening: Calculate the fan angle. If closing: Target is Home.
                    Quaternion targetRot;

                    if (opening)
                    {
                        float currentOffset = startAngle + (angleStep * i);
                        Quaternion offsetQ = Quaternion.AngleAxis(currentOffset, fanAxis);
                        targetRot = homeRotations[i] * offsetQ;
                    }
                    else
                    {
                        targetRot = homeRotations[i];
                    }

                    lightsToAnimate[i].transform.localRotation = Quaternion.Slerp(startRots[i], targetRot, t);
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // Final Snap
        // (Omitted for brevity, but same logic as other modules)
    }
}
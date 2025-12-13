using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookAtModule : MonoBehaviour
{
    [Header("Setup")]
    public Transform lightSetParent;
    public List<GameObject> lightsToAnimate;

    [Header("Targeting")]
    [Tooltip("Drag the Singer (or any object) here.")]
    public Transform targetToFollow;

    [Tooltip("Offset (e.g. aim at head height instead of feet).")]
    public Vector3 targetOffset = new Vector3(0, 1.5f, 0);

    [Header("Animation")]
    public float turnSpeed = 5.0f;

    private bool isTracking = false;

    void Start()
    {
        // Auto-Fetch
        if (lightsToAnimate.Count == 0 && lightSetParent != null)
            foreach (Transform child in lightSetParent) lightsToAnimate.Add(child.gameObject);
    }

    // --- COMMANDS ---

    public void StartTracking()
    {
        isTracking = true;
    }

    public void StopTracking()
    {
        isTracking = false;
        // Note: This script assumes other scripts (like Figure8) will take over.
        // If no other script is running, lights will stay where they are.
    }

    // --- LOGIC ---

    void Update()
    {
        if (!isTracking || targetToFollow == null) return;

        foreach (GameObject light in lightsToAnimate)
        {
            if (light != null)
            {
                // Calculate the rotation needed to look at the target
                Vector3 direction = (targetToFollow.position + targetOffset) - light.transform.position;
                Quaternion targetRot = Quaternion.LookRotation(direction);

                // Smoothly rotate towards it
                light.transform.rotation = Quaternion.Slerp(light.transform.rotation, targetRot, Time.deltaTime * turnSpeed);
            }
        }
    }
}
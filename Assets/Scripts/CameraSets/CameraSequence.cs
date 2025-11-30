using UnityEngine;
using System.Collections;
using Unity.Cinemachine; // Use "using Cinemachine;" for older versions

public class CameraSequence : MonoBehaviour
{
    [Header("Camera Setup")]
    [Tooltip("The Cinemachine Camera used for this specific sequence.")]
    public CinemachineCamera cam;

    [Header("Movement Settings (Optional)")]
    [Tooltip("Check this if the camera should move along a track during this sequence.")]
    public bool animateDolly = false;

    [Tooltip("If moving, how long does the move take?")]
    public float moveDuration = 5.0f;

    [Tooltip("Start position on track (0.0 to 1.0)")]
    public float startPathPosition = 0.0f;

    [Tooltip("End position on track (0.0 to 1.0)")]
    public float endPathPosition = 1.0f;

    private CinemachineSplineDolly dollyComponent;

    void Awake()
    {
        if (cam != null)
        {
            dollyComponent = cam.GetComponent<CinemachineSplineDolly>();
        }
    }

    // --- PUBLIC COMMANDS ---

    public void StartSequence()
    {
        if (cam != null)
        {
            cam.Priority = 100;
            Debug.Log($"CameraSequence: Starting '{gameObject.name}'");
        }

        if (animateDolly && dollyComponent != null)
        {
            StartCoroutine(AnimateDolly());
        }
    }

    public void StopSequence()
    {
        if (cam != null)
        {
            cam.Priority = 0;
        }
        StopAllCoroutines();
    }


    IEnumerator AnimateDolly()
    {
        float timer = 0f;

        dollyComponent.CameraPosition = startPathPosition;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;

            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            dollyComponent.CameraPosition = Mathf.Lerp(startPathPosition, endPathPosition, smoothT);

            yield return null;
        }

        dollyComponent.CameraPosition = endPathPosition;
    }
}
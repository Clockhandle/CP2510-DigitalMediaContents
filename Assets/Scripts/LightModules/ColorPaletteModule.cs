using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorPaletteModule : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Drag the Parent GameObject (LightSet) here.")]
    public Transform lightSetParent;
    public List<GameObject> lightsToAnimate;

    [Header("The Palette")]
    [Tooltip("Define all your concert colors here. Reference them by Element ID (0, 1, 2) in the Inspector.")]
    public List<Color> palette; // e.g., 0=Red, 1=Blue, 2=Purple, 3=Golden

    [Header("Settings")]
    public float fadeDuration = 0.5f;

    private List<Light> targetLights = new List<Light>();
    private Coroutine activeFadeCoroutine;

    void Start()
    {
        if (lightsToAnimate.Count == 0 && lightSetParent != null)
        {
            foreach (Transform child in lightSetParent)
            {
                lightsToAnimate.Add(child.gameObject);
            }
        }

        foreach (GameObject go in lightsToAnimate)
        {
            if (go != null)
            {
                Light l = go.GetComponentInChildren<Light>();
                if (l != null) targetLights.Add(l);
            }
        }
    }

    // --- PUBLIC COMMAND TO USE IN INSPECTOR ---

    // In your LightChoreographer event, select "SwitchToColorIndex" and type the number (e.g. 0)
    public void SwitchToColorIndex(int index)
    {
        // Safety check to prevent crashing if you type a wrong number
        if (index < 0 || index >= palette.Count)
        {
            Debug.LogWarning($"Color Palette Index {index} is out of range! Check your module.");
            return;
        }

        StartColorChange(palette[index]);
    }

    // --- LOGIC ---

    private void StartColorChange(Color targetColor)
    {
        if (activeFadeCoroutine != null) StopCoroutine(activeFadeCoroutine);

        if (fadeDuration <= 0.01f)
        {
            ApplyColorInstant(targetColor);
        }
        else
        {
            activeFadeCoroutine = StartCoroutine(FadeToColor(targetColor));
        }
    }

    private void ApplyColorInstant(Color target)
    {
        foreach (Light l in targetLights)
        {
            if (l != null) l.color = target;
        }
    }

    IEnumerator FadeToColor(Color target)
    {
        float timer = 0.0f;
        List<Color> startColors = new List<Color>();

        foreach (Light l in targetLights)
            startColors.Add(l != null ? l.color : target);

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;

            for (int i = 0; i < targetLights.Count; i++)
            {
                if (targetLights[i] != null)
                {
                    targetLights[i].color = Color.Lerp(startColors[i], target, t);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        ApplyColorInstant(target);
    }
}
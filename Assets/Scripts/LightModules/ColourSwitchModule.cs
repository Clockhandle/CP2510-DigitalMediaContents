using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColourSwitchModule : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Drag the Parent GameObject (LightSet) here.")]
    public Transform lightSetParent;
    public List<GameObject> lightsToAnimate;

    [Header("The Color Playlist")]
    [Tooltip("Add colors in the order they should appear in the song.")]
    public List<Color> colorSequence;

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    [Tooltip("If true, after the last color, it goes back to the first. If false, it stops at the last.")]
    public bool loopSequence = true;
    [Tooltip("If checked, the sequence resets to 0 every time the game starts.")]
    public bool autoResetOnStart = true;

    // Internal State
    private int _currentIndex = -1; // Starts at -1 so the first 'Next' call goes to 0
    private List<Light> targetLights = new List<Light>();
    private Coroutine activeFadeCoroutine;

    void Start()
    {
        // --- AUTO-POPULATE ---
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

        if (autoResetOnStart) _currentIndex = -1;
    }

    // --- PUBLIC COMMANDS (Drag these to Choreographer) ---

    // THE MAIN TRIGGER: Advances to the next color in the list
    public void NextColor()
    {
        if (colorSequence.Count == 0) return;

        // Advance the index
        _currentIndex++;

        // Handle Looping or Clamping
        if (_currentIndex >= colorSequence.Count)
        {
            if (loopSequence)
            {
                _currentIndex = 0; // Loop back to start
            }
            else
            {
                _currentIndex = colorSequence.Count - 1; // Stay on last color
                return; // Don't re-trigger if we are already there
            }
        }

        // Trigger the change
        StartColorChange(colorSequence[_currentIndex]);
    }

    // Call this if you want to force the sequence back to the start (e.g. at the start of a new Verse)
    public void ResetSequence()
    {
        _currentIndex = -1;
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
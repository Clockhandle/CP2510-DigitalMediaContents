using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LightCue
{
    public float relativeTime;   // Seconds AFTER the section starts
    public UnityEvent onTrigger;
}

// 2. The container: A song section
[System.Serializable]
public class LightSection
{
    public string sectionName;       // e.g. "Verse 1"
    public float sectionStartTime;   // When does this section start in the song?
    public List<LightCue> cues;      // The light hits inside this section
}

public class LightChoreographer : MonoBehaviour
{
    [Header("Concert Setup")]
    public AudioSource songAudio;
    public float currentSongTime; // Debug viewer

    [Header("The Light Setlist")]
    public List<LightSection> lightSections;

    // Internal list for the script to actually run
    private List<RuntimeLightCue> _flatRunList;
    private int _nextCueIndex = 0;

    // Helper class to flatten the data
    private class RuntimeLightCue
    {
        public float absoluteTriggerTime;
        public UnityEvent action;
        public string debugName;
    }

    void Start()
    {
        BuildRunList();

        if (songAudio != null && !songAudio.isPlaying)
            songAudio.Play();
    }

    void BuildRunList()
    {
        _flatRunList = new List<RuntimeLightCue>();

        foreach (var section in lightSections)
        {
            foreach (var cue in section.cues)
            {
                float absTime = section.sectionStartTime + cue.relativeTime;

                _flatRunList.Add(new RuntimeLightCue
                {
                    absoluteTriggerTime = absTime,
                    action = cue.onTrigger,
                });
            }
        }

        // Sort by time
        _flatRunList = _flatRunList.OrderBy(x => x.absoluteTriggerTime).ToList();
    }

    void Update()
    {
        if (songAudio == null || _flatRunList == null) return;

        currentSongTime = songAudio.time;

        if (_nextCueIndex >= _flatRunList.Count) return;

        RuntimeLightCue nextItem = _flatRunList[_nextCueIndex];

        if (currentSongTime >= nextItem.absoluteTriggerTime)
        {
            TriggerLight(nextItem);
            _nextCueIndex++;
        }
    }

    void TriggerLight(RuntimeLightCue item)
    {
        if (item.action != null)
        {
            // This executes whatever function you dragged into the inspector
            item.action.Invoke();
            Debug.Log($"<color=yellow>[LIGHT {item.absoluteTriggerTime:F2}s]</color> {item.debugName}");
        }
    }
}
using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using System.Linq;

// 1. The smallest unit: A single camera change
[System.Serializable]
public class CameraCue
{
    public float relativeTime;   // Seconds AFTER the section starts
    public CinemachineCamera cam;
}

// 2. The container: A song section (Verse, Chorus, etc.)
[System.Serializable]
public class SongSection
{
    public string sectionName;       // e.g., "Chorus 1"
    public float sectionStartTime;   // When does this section start in the song?
    public List<CameraCue> cues;     // The cuts inside this section
}

public class CameraChoreographer : MonoBehaviour
{
    [Header("Concert Setup")]
    public AudioSource songAudio;
    public float currentSongTime; // Debug viewer

    [Header("The Setlist")]
    public List<SongSection> songSections;

    // Internal list for the script to actually run
    private List<RuntimeCue> _flatRunList;
    private int _nextCueIndex = 0;

    // Helper class to flatten the data for easy processing
    private class RuntimeCue
    {
        public float absoluteTriggerTime;
        public CinemachineCamera cam;
        public string debugName;
    }

    void Start()
    {
        BuildRunList();

        if (songAudio != null)
            songAudio.Play();
    }

    // This converts your Sections -> One big list of times sorted automatically
    void BuildRunList()
    {
        _flatRunList = new List<RuntimeCue>();

        foreach (var section in songSections)
        {
            foreach (var cue in section.cues)
            {
                // The Math: Section Start + Relative Time
                float absTime = section.sectionStartTime + cue.relativeTime;

                _flatRunList.Add(new RuntimeCue
                {
                    absoluteTriggerTime = absTime,
                    cam = cue.cam,
                });
            }
        }

        // Sort by time so they play in order, even if you typed them out of order
        _flatRunList = _flatRunList.OrderBy(x => x.absoluteTriggerTime).ToList();

        // Reset all cams
        foreach (var item in _flatRunList)
        {
            if (item.cam != null) item.cam.Priority = 0;
        }
    }

    void Update()
    {
        if (songAudio == null || _flatRunList == null) return;

        currentSongTime = songAudio.time;

        if (_nextCueIndex >= _flatRunList.Count) return;

        // Check the next item in our calculated list
        RuntimeCue nextItem = _flatRunList[_nextCueIndex];

        if (currentSongTime >= nextItem.absoluteTriggerTime)
        {
            TriggerCam(nextItem);
            _nextCueIndex++;
        }
    }

    void TriggerCam(RuntimeCue item)
    {
        // Disable previous active cam (optional, but good practice)
        if (_nextCueIndex > 0)
        {
            var prevCam = _flatRunList[_nextCueIndex - 1].cam;
            if (prevCam != null) prevCam.Priority = 0;
        }

        // Enable new cam
        if (item.cam != null)
        {
            item.cam.Priority = 10;
            Debug.Log($"<color=cyan>[{item.absoluteTriggerTime:F2}s]</color> {item.debugName}");
        }
    }
}
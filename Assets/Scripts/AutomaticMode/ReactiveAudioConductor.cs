using UnityEngine;
using UnityEngine.Events;

public class ReactiveAudioConductor : MonoBehaviour
{
    [Header("Setup")]
    public AudioSource audioSource;

    [Header("Frequency Analyzer")]
    [Tooltip("Which part of the sound spectrum to watch?")]
    public BandTrigger[] triggers;

    // Standard FFT data container
    private float[] spectrumData = new float[512];

    void Update()
    {
        if (audioSource == null || !audioSource.isPlaying) return;

        // 1. Get the raw audio data (Fast Fourier Transform)
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Blackman);

        // 2. Check every trigger you set up
        foreach (var trigger in triggers)
        {
            trigger.CheckTrigger(spectrumData);
        }
    }

    [System.Serializable]
    public class BandTrigger
    {
        public string name = "Bass Kicks";

        [Header("Frequency Range (0-512)")]
        [Tooltip("Low end of the frequency (Bass is around 0-10)")]
        public int minIndex = 0;
        [Tooltip("High end of the frequency (Bass is around 0-10)")]
        public int maxIndex = 10;

        [Header("Sensitivity")]
        [Range(0f, 1f)]
        public float threshold = 0.1f; // How loud must it be to fire?
        public float cooldown = 0.2f;  // Prevent firing 60 times a second

        [Header("Action")]
        public UnityEvent onBeat;      // What happens? (e.g., Toggle Lights)

        // Internal timer
        private float lastTriggerTime;

        public void CheckTrigger(float[] spectrum)
        {
            // Calculate average volume of this frequency band
            float averageVal = 0;
            for (int i = minIndex; i < maxIndex; i++)
            {
                // Safety check
                if (i < spectrum.Length) averageVal += spectrum[i];
            }
            averageVal /= (maxIndex - minIndex);

            // Audio volume is often very small, scale it up slightly for easier reading
            averageVal *= 100;

            // Check if it beats the threshold AND cooldown is finished
            if (averageVal > threshold && Time.time > lastTriggerTime + cooldown)
            {
                onBeat.Invoke();
                lastTriggerTime = Time.time;
            }
        }
    }
}

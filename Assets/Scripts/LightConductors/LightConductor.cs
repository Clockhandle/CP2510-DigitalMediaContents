using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightChoreographer : MonoBehaviour
{
    [Header("Test Key")]
    [Tooltip("Press this key to start the show sequence.")]
    public KeyCode startShowKey = KeyCode.Alpha1;

    [Header("Module Sub-Managers")]
    [Tooltip("Drag your FlickerModule GameObject here.")]
    public FlickerModule flickerModule;

    [Tooltip("Drag your TiltSweepModule GameObject here.")]
    public TiltSweepModule tiltSweepModule;

    private bool isShowRunning = false;


    void Update()
    {
        if (Input.GetKeyDown(startShowKey) && !isShowRunning)
        {
            StartCoroutine(RunShowSequence());
        }
    }


    IEnumerator RunShowSequence()
    {
        isShowRunning = true;
        Debug.Log("SHOW STARTING...");

        // --- ACT 1: Flicker (using the module) ---
        Debug.Log("Act 1: Tell FlickerModule to start.");
        flickerModule.StartFlicker();

        yield return new WaitForSeconds(3.0f);

        Debug.Log("Act 1: Tell FlickerModule to stop.");
        flickerModule.StopFlicker();

        yield return new WaitForSeconds(1.0f);


        // --- ACT 2: Fan Sweep (using the module) ---
        Debug.Log("Act 2: Tell TiltSweepModule to start its wave.");
        tiltSweepModule.StartSweepWave();

        // Give the sweep time to finish
        // Note: Make sure this wait is long enough for the sweep to complete!
        yield return new WaitForSeconds(5.0f);


        // --- !!! NEW: ACT 3 - COMBINED EFFECT !!! ---
        Debug.Log("Act 3: Starting Flicker AND Sweep Wave TOGETHER!");

        // Call both start commands back-to-back.
        // They will both start running in parallel.
        flickerModule.StartFlicker();
        tiltSweepModule.StartSweepWave();

        // Let them run together for 6 seconds
        // (Adjust this time as needed for your show)
        yield return new WaitForSeconds(6.0f);

        // Stop the flicker
        Debug.Log("Act 3: Stopping flicker.");
        flickerModule.StopFlicker();
        // We don't need to "stop" the tilt sweep,
        // because its coroutine stops itself when the animation is done.


        // --- SHOW END ---
        yield return new WaitForSeconds(1.0f); // Wait for flicker to restore
        Debug.Log("SHOW FINISHED.");
        isShowRunning = false;
    }
}
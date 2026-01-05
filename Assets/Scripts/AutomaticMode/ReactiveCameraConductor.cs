using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class ReactiveCameraConductor : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Drag the Main Camera (the one with the Cinemachine Brain) here.")]
    public CinemachineBrain mainCameraBrain;

    [Tooltip("Drag the Parent GameObject holding all your cameras.")]
    public Transform cameraParent;

    // Auto-filled list of cameras
    public List<CinemachineCamera> availableCameras;

    private CinemachineCamera currentCam;

    void Start()
    {
        // 1. Try to find the brain automatically if you forgot to drag it in
        if (mainCameraBrain == null)
            mainCameraBrain = Camera.main.GetComponent<CinemachineBrain>();

        // 2. Auto-Fetch all cameras
        if (cameraParent != null)
        {
            var cams = cameraParent.GetComponentsInChildren<CinemachineCamera>(true);
            foreach (var c in cams)
            {
                availableCameras.Add(c);
                c.Priority = 0;
            }
        }
    }

    public void SwitchToRandomCamera()
    {
        if (availableCameras.Count == 0) return;

        // --- THE SYNC FIX ---
        // If the camera is currently in the middle of a blend (like your 6-second sweep),
        // we DO NOT interrupt it. We wait for the movement to finish.
        if (mainCameraBrain != null && mainCameraBrain.IsBlending)
        {
            return;
        }

        // 1. Pick a random camera
        int randomIndex = Random.Range(0, availableCameras.Count);
        CinemachineCamera newCam = availableCameras[randomIndex];

        // 2. Avoid picking the same one twice
        if (newCam == currentCam && availableCameras.Count > 1)
        {
            SwitchToRandomCamera();
            return;
        }

        // 3. Switch Logic
        if (currentCam != null) currentCam.Priority = 0;

        newCam.Priority = 10;
        currentCam = newCam;
    }
}
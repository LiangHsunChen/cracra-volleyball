using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public SetupManager _setupManager;

    public float movementSpeed = 8f;
    public float jumpForce = 135f;

    private Camera cam;
    public Vector3 cameraLeftEdge;
    public Vector3 cameraRightEdge;

    private void Awake()
    {
        cam = Camera.main;
        cameraLeftEdge = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight / 2, cam.transform.position.z));
        cameraRightEdge = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight / 2, cam.transform.position.z));

        Instance = this;
    }

    private void Update()
    {
        // Reset button for testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            _setupManager.Reset();            
        }
    }
}

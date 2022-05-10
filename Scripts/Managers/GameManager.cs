using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public readonly float LEFT = -1f;
    public readonly float RIGHT = 1f;

    // Game objects
    public SetupManager _setupManager;
    public GameObject net;
    public GameObject ball;
    public GameObject player;
    public GameObject bot;
    public GameObject ground;
    public GameObject leftWall;
    public GameObject rightWall;

    // Game objects' components
    // Rigidbody2D
    public Rigidbody2D botRigidbody2D;
    public Rigidbody2D ballRigidbody2D;
    // Collider2D
    public CapsuleCollider2D playerCapsuleCollider2D;
    public CircleCollider2D ballCircleCollider2D;
    public EdgeCollider2D groundCollider2D;
    public CapsuleCollider2D netCollider2D;

    // Net's horizontal range from x to y
    public Vector2 netHorizontalRange;
    public float netExtent;
    public float netExtentY;
    public float playerExtent;
    public float desiredDropPointY;
    public Vector3 netPos;
    private float leftWallWidth;
    private float rightWallWidth;

    // Game values
    public float movementSpeed = 7f;
    public float jumpForce = 120f;
    public float smashRange = 0.35f;
    public float smashForce = 140f;
    public float spinForce = 1800f;
    public float liftForce = 10f;

    // Camera
    private Camera cam;
    public Vector3 cameraLeftEdge;
    public Vector3 cameraRightEdge;

    private bool resetOn = true;

    private void Awake()
    {
        // Rigidbody2D
        botRigidbody2D = bot.GetComponent<Rigidbody2D>();
        ballRigidbody2D = ball.GetComponent<Rigidbody2D>();

        // Collider2D
        playerCapsuleCollider2D = player.GetComponent<CapsuleCollider2D>();
        ballCircleCollider2D = ball.GetComponent<CircleCollider2D>();
        groundCollider2D = ground.GetComponent<EdgeCollider2D>();
        netCollider2D = net.GetComponent<CapsuleCollider2D>();

        netExtent = netCollider2D.size.y / 2;
        netExtentY = netCollider2D.size.x / 2;
        playerExtent = playerCapsuleCollider2D.size.x / 2;
        netHorizontalRange = new(-netExtent - playerExtent, netExtent + playerExtent);
        netPos = net.transform.position;
        leftWallWidth = leftWall.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        rightWallWidth = rightWall.GetComponent<SpriteRenderer>().bounds.size.x / 2;

        desiredDropPointY = ground.transform.position.y + (ground.GetComponent<SpriteRenderer>().bounds.size.y / 2) +
            (ball.GetComponent<SpriteRenderer>().bounds.size.y / 2);

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

        // Toggle reset when ball drops on the ground
        if (Input.GetKeyDown(KeyCode.T))
        {
            resetOn = !resetOn;
        }

        if (cameraRightEdge != cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight / 2, cam.transform.position.z)))
        {
            cameraRightEdge = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight / 2, cam.transform.position.z));
            _setupManager.WallsSetup();
        }

    }

    private void FixedUpdate()
    {
        if (resetOn)
        {
            if (ballCircleCollider2D.IsTouching(groundCollider2D))
            {
                _setupManager.Reset();
            }
        }
    }
}

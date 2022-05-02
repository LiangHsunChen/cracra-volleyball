using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{


    private GameManager gm;

    public GameObject ball;
    public GameObject player;
    public GameObject leftWall;
    public GameObject rightWall;

    private Vector3 playerPosition;
    private Vector3 ballStartPosition;
    private Vector3 leftWallPosition;
    private Vector3 rightWallPosition;

    private float ballExtent;
    private float leftWallWidth;
    private float rightWallWidth;

    private Rigidbody2D ballRigidbody2D;

    private void Start()
    {
        gm = GameManager.Instance;
        ballRigidbody2D = ball.GetComponent<Rigidbody2D>();
        ballStartPosition = ballRigidbody2D.position;

        Setup();
    }

    // Setup game objects
    void Setup()
    {
        BallSetup();
        PlayerSetup();
        WallsSetup();
    }

    // Reset game objects to initial state
    public void Reset()
    {
        BallSetup();
        ballRigidbody2D.velocity = Vector2.zero;
        ballRigidbody2D.rotation = 0f;
        ballRigidbody2D.angularVelocity = 0f;

        PlayerSetup();
    }

    // Setup ball's position
    void BallSetup()
    {
        ballExtent = ball.GetComponent<CircleCollider2D>().bounds.size.x / 2;
        ball.transform.position = new Vector3(gm.cameraRightEdge.x - ballExtent, ballStartPosition.y, ballStartPosition.z);
    }

    // Setup player's position
    void PlayerSetup()
    {
        playerPosition = player.transform.position;
        player.transform.position = new Vector3(gm.cameraRightEdge.x - ballExtent, playerPosition.y, playerPosition.z);
    }

    // Setup walls' position
    void WallsSetup()
    {
        leftWallPosition = leftWall.transform.position;
        rightWallPosition = rightWall.transform.position;
        leftWallWidth = leftWall.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        rightWallWidth = rightWall.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        leftWall.transform.position = new Vector3(gm.cameraLeftEdge.x - leftWallWidth, gm.cameraLeftEdge.y, leftWallPosition.z);
        rightWall.transform.position = new Vector3(gm.cameraRightEdge.x + rightWallWidth, gm.cameraRightEdge.y, rightWallPosition.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{


    private GameManager gm;

    private Vector3 playerPosition;
    private Vector3 ballStartPosition;
    private Vector3 leftWallPosition;
    private Vector3 rightWallPosition;

    private float ballExtent;

    private Rigidbody2D ballRigidbody2D;

    private void Start()
    {
        gm = GameManager.Instance;
        ballRigidbody2D = gm.ball.GetComponent<Rigidbody2D>();
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
        ballExtent = gm.ball.GetComponent<CircleCollider2D>().bounds.size.x / 2;
        gm.ball.transform.position = new Vector3(gm.cameraRightEdge.x - ballExtent, ballStartPosition.y, ballStartPosition.z);
    }

    // Setup player's position
    void PlayerSetup()
    {
        playerPosition = gm.player.transform.position;
        gm.player.transform.position = new Vector3(gm.cameraRightEdge.x - ballExtent, playerPosition.y, playerPosition.z);
    }

    // Setup walls' position
    public void WallsSetup()
    {
        leftWallPosition = gm.leftWall.transform.position;
        rightWallPosition = gm.rightWall.transform.position;
        gm.leftWall.transform.position = new Vector3(gm.cameraLeftEdge.x - gm.leftWallWidth, gm.cameraLeftEdge.y, leftWallPosition.z);
        gm.rightWall.transform.position = new Vector3(gm.cameraRightEdge.x + gm.rightWallWidth, gm.cameraRightEdge.y, rightWallPosition.z);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    private static readonly float LEFT = -1f;
    private static readonly float RIGHT = 1f;

    private GameManager gm;

    public Animator animator;
    public GameObject net;
    public GameObject ball;
    public GameObject ground;

    private float horizontalInput = 0f;
    private bool hasJumped = false;
    // Bot
    private Vector3 desiredPosition;
    private float botExtent;
    // Net
    private Vector2 netHorizontalRange;
    private float netExtent;
    // Ball
    private Rigidbody2D ballRigidbody2D;
    private float desiredDropPointY;
    private Vector2 ballCurrentPosition;
    private float calcInterval;
    private Vector2 gravityAccel;
    private Vector2 ballPrevDropPoint;
    private Vector2 ballDropPoint;


    private Rigidbody2D _rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        netExtent = net.GetComponent<CapsuleCollider2D>().size.y / 2;
        botExtent = GetComponent<CapsuleCollider2D>().size.x / 2;
        netHorizontalRange = new(-netExtent - botExtent, netExtent + botExtent);
        ballRigidbody2D = ball.GetComponent<Rigidbody2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        desiredDropPointY = ground.transform.position.y + (ground.GetComponent<SpriteRenderer>().bounds.size.y / 2) +
            (ball.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        ballDropPoint = new(0, 0);
        calcInterval = 0.05f;
        gravityAccel = calcInterval * calcInterval * Physics2D.gravity * ballRigidbody2D.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Animator setting
        animator.SetFloat("Horizontal", horizontalInput);
    }

    private void FixedUpdate()
    {
        // Calculate ball drop point
        //if (NeedsRecalculate())
        //{
        //    ballDropPoint = CalculateBallDropPoint();
        //}
        ballDropPoint = CalculateBallDropPoint();

        // Move to the ball drop point
        horizontalInput = MoveTowards(ballDropPoint);

        //if (hasJumped)
        //{
        //    ConditionalJump();
        //}
    }

    // Check if the ball's drop point needs recalculate
    private bool NeedsRecalculate()
    {
        return ball.GetComponent<Ball>().hasCollided;
    }

    // Calculate the result drop point of the ball when it goes under the character's head
    private Vector2 CalculateBallDropPoint()
    {
        ball.GetComponent<Ball>().hasCollided = false;

        Vector2 resultVelocity = ballRigidbody2D.velocity * calcInterval + gravityAccel;
        Vector2 resultDropPoint = CalculateNextBallDropPoint(ballRigidbody2D.position, resultVelocity);
        Vector2 prevDropPoint = ballRigidbody2D.position;
        // Calculate the drop point until it goes under the character's head
        for (int i = 0; i < 1000; i++)
        {
            if (resultDropPoint.y <= desiredDropPointY)
            {
                return prevDropPoint;
            }
            resultVelocity += gravityAccel;
            prevDropPoint = resultDropPoint;
            resultDropPoint = CalculateNextBallDropPoint(resultDropPoint, resultVelocity);
        }
        return resultDropPoint;
    }

    // Calculate the next drop point
    private Vector2 CalculateNextBallDropPoint(Vector2 ballCurrentPosition, Vector2 ballVelocity)
    {
        return ballCurrentPosition + ballVelocity;
    }

    // Moves to the desired position and
    // return the horizontal input (-1|0|1) that moves towards target
    private float MoveTowards(Vector2 targetPoint)
    {
        float resultMovement = 0;
        Vector3 charPosition = transform.position;

        // If the character is close to the target point then don't move
        if (Mathf.Abs(charPosition.x - targetPoint.x) < 0.25f)
        {
            return resultMovement;
        }

        // Move depends on the target location
        if (charPosition.x < targetPoint.x)
        {
            resultMovement = RIGHT;
        }
        else if (charPosition.x > targetPoint.x)
        {
            resultMovement = LEFT;
        }

        // Get bot's position after movement
        desiredPosition = charPosition + new Vector3(resultMovement * gm.movementSpeed, 0, 0) * Time.deltaTime;
        ConditionalMove(desiredPosition);

        return resultMovement;
    }

    // Can only move if the desired position is not within the net range
    private void ConditionalMove(Vector3 desiredPosition)
    {
        // Prevent bot to pass the net
        if (!IsInNetRange(desiredPosition))
        {
            transform.position = desiredPosition;
        }
    }

    // Check if the desired position is within the net range
    private bool IsInNetRange(Vector3 desiredPosition)
    {
        return desiredPosition.x > netHorizontalRange.x && desiredPosition.x < netHorizontalRange.y;
    }

    // Jump if is on the ground
    private void ConditionalJump()
    {
        if (Mathf.Abs(_rigidbody2D.velocity.y) == 0) // Can only jump when on the ground
        {
            _rigidbody2D.AddForce(new Vector2(0, gm.jumpForce), ForceMode2D.Impulse);
        }
        hasJumped = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Remove the reflect force from ball
        if (collision.gameObject.CompareTag("Ball"))
        {
            _rigidbody2D.velocity = Vector2.zero;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    private GameManager gm;

    public Animator animator;

    private float horizontalInput = 0f;
    private bool hasJumped = false;
    // Bot
    private Vector3 desiredPosition;
    // Ball
    private Vector2 ballCurrentPosition;
    private float calcInterval;
    private Vector2 gravityAccel;
    private Vector2 ballPrevDropPoint;
    private Vector2 ballDropPoint;

    private Rigidbody2D _rigidbody2D;
    private CapsuleCollider2D _capsuleCollider2D;

    private Smash smash;

    private bool isFollowingBall = false;
    private bool isJumpInCD = false;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        smash = GetComponent<Smash>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        ballDropPoint = new(0, 0);
        calcInterval = 0.05f;
        gravityAccel = calcInterval * calcInterval * Physics2D.gravity * gm.ballRigidbody2D.gravityScale;
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
        
        if (isFollowingBall)
        {
            horizontalInput = MoveTowards(gm.ballRigidbody2D.position);
        }
        else
        {
            // Move to the ball drop point
            ballDropPoint = CalculateBallDropPoint();
            horizontalInput = MoveTowards(ballDropPoint);
        }

        if (BallIsAbove())
        {
            ConditionalJump();
            isFollowingBall = true;
            StartCoroutine(InJumpCD());
        }

        if (_capsuleCollider2D.IsTouching(gm.groundCollider2D))
        {
            isFollowingBall = false;
        }

        //if (BallIsNearBy())
        //{
        //    ConditionalJump();
        //}

    }

    private IEnumerator InJumpCD()
    {
        isJumpInCD = true;
        yield return new WaitForSeconds(0.5f);
        isJumpInCD = false;
    }

    // Determine if the ball is above the character
    private bool BallIsAbove()
    {
        float ballDropX = ballDropPoint.x;
        float ballPosX = gm.ballRigidbody2D.position.x;
        float charPosX = _rigidbody2D.position.x;
        // If the character horizontal position is between the ball's drop point and the ball's current position.
        bool isInRange = Mathf.Clamp(charPosX, Mathf.Min(ballDropX, ballPosX), Mathf.Max(ballDropX, ballPosX)) == charPosX;

        //return Mathf.Abs(ballDropX - ballPosX) < 1f && isInRange
        return Mathf.Abs(charPosX - ballPosX) < 0.5f;
    }

    private bool BallIsNearBy()
    {
        return _capsuleCollider2D.Distance(gm.ballCircleCollider2D).distance < 1f;
    }

    // Check if the ball's drop point needs recalculate
    private bool NeedsRecalculate()
    {
        return gm.ball.GetComponent<Ball>().hasCollided;
    }

    // Calculate the result drop point of the ball when it goes under the character's head
    private Vector2 CalculateBallDropPoint()
    {
        gm.ball.GetComponent<Ball>().hasCollided = false;

        Vector2 resultVelocity = gm.ballRigidbody2D.velocity * calcInterval + gravityAccel;
        Vector2 resultDropPoint = CalculateNextBallDropPoint(gm.ballRigidbody2D.position, resultVelocity);
        Vector2 prevDropPoint = gm.ballRigidbody2D.position;
        // Calculate the drop point until it goes under the character's head
        for (int i = 0; i < 1000; i++)
        {
            if (resultDropPoint.y <= gm.desiredDropPointY)
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
        return desiredPosition.x > gm.netHorizontalRange.x && desiredPosition.x < gm.netHorizontalRange.y;
    }

    // Jump if is on the ground
    private void ConditionalJump()
    {
        if (IsOnTheGround()) // Can only jump while on the ground
        {
            _rigidbody2D.AddForce(new Vector2(0, gm.jumpForce), ForceMode2D.Impulse);
        }
    }

    // Check if the object is on the ground
    private bool IsOnTheGround()
    {
        return _capsuleCollider2D.Distance(gm.groundCollider2D).distance < -0.004f && _capsuleCollider2D.IsTouching(gm.groundCollider2D);
    }


    private bool CanSmash()
    {
        float netHeight = gm.netPos.y + gm.netExtentY;
        float botPosY = _rigidbody2D.position.y;

        return botPosY > netHeight;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Remove the reflect force from ball
        if (collision.gameObject.CompareTag("Ball"))
        {
            _rigidbody2D.velocity = Vector2.zero;

            if (CanSmash())
            {
                smash.SmashBall("Bot");
            }
        }
    }
}

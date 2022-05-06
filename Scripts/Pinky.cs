using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : MonoBehaviour
{
    private GameManager gm;

    public Animator animator;
    public GameObject net;
    public GameObject ball;
    public GameObject ground;

    private float horizontalInput = 0f;
    private bool hasJumped = false;
    private bool pressedSpace = false;
    // Player's position after movement
    private Vector3 desiredPosition;
    private float playerExtent;
    // Net's horizontal range from x to y
    private Vector2 netHorizontalRange;
    private float netExtent;

    private Rigidbody2D _rigidbody2D;
    private CapsuleCollider2D _capsuleCollider2D;
    private Rigidbody2D ballRigidbody2D;
    private CircleCollider2D ballCircleCollider2D;
    private EdgeCollider2D groundCollider2D;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        ballRigidbody2D = ball.GetComponent<Rigidbody2D>();
        ballCircleCollider2D = ball.GetComponent<CircleCollider2D>();
        groundCollider2D = ground.GetComponent<EdgeCollider2D>();
        netExtent = net.GetComponent<CapsuleCollider2D>().size.y / 2;
        playerExtent = _capsuleCollider2D.size.x / 2;
        netHorizontalRange = new(-netExtent - playerExtent, netExtent + playerExtent);
    }

    // Update is called once per frame
    void Update()
    {
        // Horizontal movement
        horizontalInput = Input.GetAxisRaw("Horizontal");
        // Animator setting
        animator.SetFloat("Horizontal", horizontalInput);
        // Detact jump keypress
        if (Input.GetButtonDown("Jump"))
        {
            hasJumped = true;
        }
        // Detact space keypress
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pressedSpace = true;
        }
    }

    private void FixedUpdate()
    {
        // Get player's position after movement
        desiredPosition = transform.position + new Vector3(horizontalInput * gm.movementSpeed, 0, 0) * Time.deltaTime;
        ConditionalMove(desiredPosition);

        if (hasJumped)
        {
            ConditionalJump();
        }

        if (pressedSpace)
        {
            DoSpaceAction();
            pressedSpace = false;
        }
    }

    private void DoSpaceAction()
    {
        if (IsOnTheGround())
        {
            //Pounce();
            SmashBall();
        }
        else
        {
            SmashBall();
        }
    }

    private void SmashBall()
    {
        // Do nothing if the ball in not within smash range
        if (_capsuleCollider2D.Distance(ballCircleCollider2D).distance > gm.smashRange)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {

                smashWithDirection(Vector2.left + Vector2.up);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {

                smashWithDirection(Vector2.left + Vector2.down);
            }
            else
            {
                smashWithDirection(Vector2.left);
            }
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            smashWithDirection(Vector2.up + new Vector2(-0.5f, 0f));
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            smashWithDirection(Vector2.down + new Vector2(-0.3f ,0f));
        }
        else
        {
            smashWithDirection(Vector2.down + new Vector2(-0.5f, 0f));
        }
    }

    private void smashWithDirection(Vector2 direciton)
    {
        ballRigidbody2D.velocity = Vector2.zero;
        ballRigidbody2D.angularVelocity = gm.spinForce;
        ballRigidbody2D.AddForce(direciton * gm.smashForce, ForceMode2D.Impulse);
        StartCoroutine(IgnoreCollisionWith(ballCircleCollider2D));
    }

    private IEnumerator IgnoreCollisionWith(Collider2D collider2D)
    {
        Physics2D.IgnoreCollision(_capsuleCollider2D, ballCircleCollider2D, true);
        yield return new WaitForSeconds(0.1f);
        Physics2D.IgnoreCollision(_capsuleCollider2D, ballCircleCollider2D, false);
    }

    // Can only move if the desired position is not within the net range
    private void ConditionalMove(Vector3 desiredPosition)
    {
        // Prevent player to pass the net
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
        if (IsOnTheGround()) // Can only jump when on the ground
        {
            _rigidbody2D.AddForce(new Vector2(0, gm.jumpForce), ForceMode2D.Impulse);
        }
        hasJumped = false;
    }

    // Check if the object is on the ground
    private bool IsOnTheGround()
    {
        return _capsuleCollider2D.IsTouching(groundCollider2D);
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

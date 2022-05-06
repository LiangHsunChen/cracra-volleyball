using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : MonoBehaviour
{
    private GameManager gm;

    public Animator animator;

    private float horizontalInput = 0f;
    private bool hasJumped = false;
    private bool pressedSpace = false;
    // Player's position after movement
    private Vector3 desiredPosition;

    private Rigidbody2D _rigidbody2D;
    private CapsuleCollider2D _capsuleCollider2D;

    private Smash smash;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        
        smash = GetComponent<Smash>();
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
            smash.SmashBall("Player");
        }
        else
        {
            smash.SmashBall("Player");
        }
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
        return desiredPosition.x > gm.netHorizontalRange.x && desiredPosition.x < gm.netHorizontalRange.y;
    }

    // Jump if is on the ground
    private void ConditionalJump()
    {
        if (IsOnTheGround()) // Can only jump while on the ground
        {
            _rigidbody2D.AddForce(new Vector2(0, gm.jumpForce), ForceMode2D.Impulse);
        }
        hasJumped = false;
    }

    // Check if the object is on the ground
    private bool IsOnTheGround()
    {
        return _capsuleCollider2D.IsTouching(gm.groundCollider2D);
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

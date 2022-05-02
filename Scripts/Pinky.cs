using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : MonoBehaviour
{
    private GameManager gm;

    public Animator animator;
    public GameObject net;

    private float horizontalInput = 0f;
    private bool hasJumped = false;
    // Player's position after movement
    private Vector3 desiredPosition;
    private float playerExtent;
    // Net's horizontal range from x to y
    private Vector2 netHorizontalRange;
    private float netExtent;


    private Rigidbody2D _rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        netExtent = net.GetComponent<CapsuleCollider2D>().size.y / 2;
        playerExtent = GetComponent<CapsuleCollider2D>().size.x / 2;
        netHorizontalRange = new(-netExtent - playerExtent, netExtent + playerExtent);
        _rigidbody2D = GetComponent<Rigidbody2D>();
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

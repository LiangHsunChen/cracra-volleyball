using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private GameManager gm;
    private Rigidbody2D _rigidbody2D;

    public bool hasCollided;

    private void Awake()
    {
        hasCollided = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.ballCircleCollider2D.IsTouching(gm.groundCollider2D))
        {

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //StartCoroutine(SetAfterDelay());

        hasCollided = true;
    }

    private IEnumerator SetAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        hasCollided = true;
    }
}

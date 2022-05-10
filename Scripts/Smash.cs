using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smash : MonoBehaviour
{
    public GameManager gm;

    private Collider2D _collider2D;

    // R:Right L:Left U:Up D:Down
    private static Vector2 RU = Vector2.up + Vector2.right;
    private static Vector2 RUU = Vector2.up + new Vector2(0.5f, 0f);
    private static Vector2 RD = Vector2.down + Vector2.right;
    private static Vector2 RDD = Vector2.down + new Vector2(0.5f, 0f);
    private static Vector2 LU = Vector2.up + Vector2.left;
    private static Vector2 LUU = Vector2.up + new Vector2(-0.5f, 0f);
    private static Vector2 LD = Vector2.down + Vector2.left;
    private static Vector2 LDD = Vector2.down + new Vector2(-0.5f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;

        _collider2D = GetComponent<Collider2D>();
    }

    public void BotSmashBalLUp()
    {
        SmashWithDirection(RU);
    }

    public void SmashBall(string character)
    {
        // Do nothing if the ball is not within smash range
        if (_collider2D.Distance(gm.ballCircleCollider2D).distance > gm.smashRange)
        {
            return;
        }

        if (character == "Player")
        {
            PlayerSmashBall();
        }
        else if (character == "Bot")
        {
            BotSmashBall();
        }
    }

    private void PlayerSmashBall()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.UpArrow)) // < ^
            {
                SmashWithDirection(LU);
            }
            else if (Input.GetKey(KeyCode.DownArrow)) // < v
            {
                SmashWithDirection(LD);
            }
            else
            {
                SmashWithDirection(Vector2.left);
            }
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            SmashWithDirection(LUU);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            SmashWithDirection(Vector2.down + new Vector2(-0.3f, 0f));
        }
        else
        {
            SmashWithDirection(LDD);
        }
    }

    private void BotSmashBall()
    {
        float botPosY = gm.botRigidbody2D.position.y;
        float botposX = gm.botRigidbody2D.position.x;
        float netHeight = gm.netPos.y + gm.netExtentY;
        float netLeftPoint = gm.netPos.x - gm.netExtent;
        float wallToNetDis = Mathf.Abs(gm.leftWall.transform.position.x - gm.netPos.x);

        if (botPosY > netHeight) // Bot higher than net
        {
            Vector2 direction = new(0, 0);
            //if (Mathf.Abs(botposX - netLeftPoint) > wallToNetDis * 2 / 3) // Bot closer to wall
            //{
            //    switch (Random.Range(1, 3))
            //    {
            //        case 1:
            //            direction = RU;
            //            break;

            //        case 2:
            //            direction = Vector2.right;
            //            break;
            //    }
            //}
            if (Mathf.Abs(botposX - netLeftPoint) < wallToNetDis * 1 / 3) // Bot closer to net
            {
                switch (Random.Range(1, 3))
                {
                    case 1:
                        switch(Random.Range(1, 3))
                        {
                            case 1:
                                direction = RU;
                                break;

                            case 2:
                                direction = Vector2.right;
                                break;
                        }
                        break;

                    case 2:
                        direction = RD;
                        break;
                }
            }
            else if (Mathf.Abs(botposX - netLeftPoint) < gm.playerExtent + 1f) // Bot is very close to net
            {
                direction = RDD;
            }
            else
            {
                switch (Random.Range(1, 3))
                {
                    case 1:
                        direction = RU;
                        break;

                    case 2:
                        direction = Vector2.right;
                        break;
                }
            }
            SmashWithDirection(direction);
        }
        else // Bot lower than net
        {
            Debug.Log(4);
            SmashWithDirection(RU);
        }
    }

    private void SmashWithDirection(Vector2 direciton)
    {
        gm.ballRigidbody2D.velocity = Vector2.zero;
        gm.ballRigidbody2D.angularVelocity = gm.spinForce;
        gm.ballRigidbody2D.AddForce(direciton * gm.smashForce, ForceMode2D.Impulse);
        StartCoroutine(IgnoreCollisionWith(gm.ballCircleCollider2D));
    }

    private IEnumerator IgnoreCollisionWith(Collider2D collider2D)
    {
        Physics2D.IgnoreCollision(_collider2D, collider2D, true);
        yield return new WaitForSeconds(0.1f);
        Physics2D.IgnoreCollision(_collider2D, collider2D, false);
    }
}

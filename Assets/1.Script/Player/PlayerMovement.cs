using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    CircleCollider2D circleCollider;

    [SerializeField]
    float moveSpped = 260f;
    [SerializeField]
    float jumpPower = 7;
    [SerializeField]
    float maxSpeed = 260f;
    [SerializeField]
    float slideRate = 0.25f;
    [SerializeField]
    float AttackSlideRate = 0.1f;

    int whatisGround;
    float moveDir;
    bool isGround;
    int jumpCount;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        whatisGround = 1 << LayerMask.NameToLayer("Plaform");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        GroundCheck();
        PlayerAnim();
    }
    private void FixedUpdate()
    {
        if(PlayerFlip() || Mathf.Abs (moveDir * rb.velocity.x) < maxSpeed)      //최고속도에 도달하기 전까지..
        {
            rb.AddForce(new Vector2(moveDir * Time.fixedDeltaTime * moveSpped, 0));
        }
        else
        {
            rb.velocity = new Vector2(moveDir * maxSpeed, rb.velocity.y);       // 최고 속도에 도달하면 velocity로 그냥 밀기
        }

        rb.AddForce(new Vector2(moveDir * Time.fixedDeltaTime * moveSpped, rb.velocity.y));
    }

    void PlayerInput()
    {
        moveDir = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) /*&& !isPlayeringAnim("Attack")*/)
        {
            if (jumpCount < 1)
            {
                jumpCount++;
                rb.velocity += new Vector2(0, jumpPower);
                MyAnimSetTrigger("Jump");
            }
            else if (!isGround)
                return;
        }
    }

    void GroundFriction()
    {
        float refVelocity = 0;
        //if (isGround)
        //{
        //    if (isPlayeringAnim("Attack"))
        //    {
        //        rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, 0f, ref refVelocity, slideRate + AttackSlideRate), rb.velocity.y);
        //    }
        //}
        //else
        if (Mathf.Abs(moveDir) <= 0.01f)
        {
            rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, 0f, ref refVelocity, slideRate), rb.velocity.y);
        }
    }

    void PlayerAnim()
    {
        if (isGround)
        {
            if ((Mathf.Abs(moveDir) <= 0.01f || Mathf.Abs(rb.velocity.x) <= 0.01f) && Mathf.Abs(rb.velocity.y) <= 0.01f)
            {
                MyAnimSetTrigger("Idle");
            }
            else if (Mathf.Abs(rb.velocity.x) > 0.01 && Mathf.Abs(rb.velocity.y) <= 0.01f)
            {
                MyAnimSetTrigger("Walk");
            }
        }
    }

    bool PlayerFlip()
    {
        bool flipSprite = (sr.flipX ? (moveDir > 0f) : (moveDir < 0f));
        if (flipSprite)
        {
            sr.flipX = !sr.flipX;
        }

        return flipSprite;
    }

    void GroundCheck()
    {
        if (Physics2D.Raycast(circleCollider.bounds.center, Vector2.down, 1f, whatisGround))
        {
            isGround = true;
            jumpCount = 0;  // 2단 점프를 위한 초기화
            anim.ResetTrigger("Idle");  //test
        }
        else
        {
            isGround = false;
        }
    }

    bool isPlayeringAnim(string AnimName)
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsName(AnimName))
        {
            return true;
        }
        return false;
    }

    void MyAnimSetTrigger (string AnimName)
    {
        if (!isPlayeringAnim(AnimName))
        {
            anim.SetTrigger(AnimName);
        }
    }

}

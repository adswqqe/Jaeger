using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    CapsuleCollider2D capsuleCollider;
    Transform PlayerattackRangeTr;
    Transform PlayerCatchTr;

    [SerializeField]
    float moveSpped = 280f;
    [SerializeField]
    float jumpPower = 7;
    [SerializeField]
    float maxSpeed = 290f;
    [SerializeField]
    float slideRate = 0.25f;
    [SerializeField]
    float AttackSlideRate = 0.1f;
    [SerializeField]
    float AttackMoveSpped = 0.1f;
    [SerializeField]
    float dashDistance = 2f;
    
    const float DOUBLE_CLICK_TIME = 0.2f;
    const float DASH_COOL_TIME = 2.0f;
    const int MAX_DASH_NUMBER_OF_TIME = 3;
    int curDashNumberOfTime = 3;
    float curDashCoolTime = 0f;
    float lastLeftClickTime;
    float lastRightClickTime;
    int whatisGround;
    float moveDir;
    bool isGround;
    int jumpCount;
    float lastMoveDir;
    bool isAttack = false;

    public void Init(Transform PlayerattackRangeTr, Transform PlayerCatchTr)
    {
        this.PlayerattackRangeTr = PlayerattackRangeTr;
        this.PlayerCatchTr = PlayerCatchTr;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        whatisGround = 1 << LayerMask.NameToLayer("Plaform");

        StartCoroutine("CoolTimeCoroutine");
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
        if (isAttack)
        {
            rb.velocity = new Vector2(moveDir * AttackMoveSpped, rb.velocity.y);
        }
        else if (PlayerFlip() || Mathf.Abs (moveDir * rb.velocity.x) < maxSpeed)      //최고속도에 도달하기 전까지..
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
        if (isAttack)
            return;

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

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            float timeSinceLastClick = Time.time - lastLeftClickTime;

            if (timeSinceLastClick <= DOUBLE_CLICK_TIME)    //Double Click
            {
                Debug.Log("Left double");
                lastMoveDir = moveDir;
                HandleDash(Vector2.left);
            }

            lastLeftClickTime = Time.time;
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            float timeSinceLastClick = Time.time - lastRightClickTime;

            if (timeSinceLastClick <= DOUBLE_CLICK_TIME)    //Double Click
            {
                lastMoveDir = moveDir;
                Debug.Log("Right double");
                HandleDash(Vector2.right);
            }

            lastRightClickTime = Time.time;
        }

        
    }

    bool CanMove(Vector3 dir, float distance)
    {
        Debug.Log(Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), dir, distance, whatisGround).collider == null);
        return Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), dir, distance, whatisGround).collider == null;
    }

    void HandleDash(Vector2 dir)
    {
        //Debug.DrawLine(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z)
        //                , new Vector3(transform.position.x + (lastMoveDir * dashDistance), transform.position.y + 0.5f, transform.position.z), Color.red);
        if (curDashNumberOfTime <= 0)        // 만약 대쉬 횟수가 0보다 적다면
            return;
        else
            curDashNumberOfTime--;

        if (CanMove(dir, dashDistance))
            transform.position = new Vector3(transform.position.x + (lastMoveDir * dashDistance), transform.position.y, transform.position.z);
        else
        {
            var hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z),
                            new Vector3(transform.position.x + (lastMoveDir * dashDistance), transform.position.y, transform.position.z), dashDistance, whatisGround);
            if (hit.distance > 0.6)
                transform.position = new Vector3(transform.position.x + (lastMoveDir * (hit.distance * 0.5f)), transform.position.y, transform.position.z); // 벽과 캐릭터의 절반만 대쉬하도록.. 자꾸 낑김
            Debug.Log(hit.distance);
        }
    }

    void GroundFriction()
    {
        float refVelocity = 0;
        //if (isGround)
        //{
        //    if (isAttack)
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
            if ((Mathf.Abs(moveDir) <= 0.001f && Mathf.Abs(rb.velocity.x) <= 0.01f) && Mathf.Abs(rb.velocity.y) <= 0.01f && !isAttack)     //(Mathf.Abs(moveDir) <= 0.001f && Mathf.Abs(rb.velocity.x) <= 0.001f) 
                                                                                                                                //  ||처리를 하니 자꾸 키 방향바꿀때마다 idle로 돌아가서 &&처리
            {
                    MyAnimSetTrigger("Idle");
            }
            else if (Mathf.Abs(rb.velocity.x) > 0.001 && Mathf.Abs(rb.velocity.y) <= 0.001f && !isAttack)
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
            PlayerattackRangeTr.localPosition = new Vector3(-PlayerattackRangeTr.localPosition.x, PlayerattackRangeTr.localPosition.y, PlayerattackRangeTr.localPosition.z);
            PlayerCatchTr.localPosition = new Vector3(-PlayerCatchTr.localPosition.x, PlayerCatchTr.localPosition.y, PlayerCatchTr.localPosition.z);

        }

        if (sr.flipX)
        {
            //PlayerattackRangeTr.localPosition = new Vector3(-(PlayerattackRangeTr.localPosition.x), PlayerattackRangeTr.localPosition.y, PlayerattackRangeTr.localPosition.z);
            //Debug.Log(PlayerattackRangeTr.localScale);
        }
        else
        {
            //PlayerattackRangeTr.localPosition = new Vector3(Mathf.Abs(PlayerattackRangeTr.localPosition.x), PlayerattackRangeTr.localPosition.y, PlayerattackRangeTr.localPosition.z);
        }


        return flipSprite;
    }

    void GroundCheck()
    {
        if (Physics2D.Raycast(capsuleCollider.bounds.center, Vector2.down, 1f, whatisGround))
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

    public void OnAttacking(bool isAttacking)
    {
        isAttack = isAttacking;
    }

    IEnumerator CoolTimeCoroutine()
    {
        
        while(true)
        {
            if (curDashNumberOfTime < MAX_DASH_NUMBER_OF_TIME)
            {
                curDashCoolTime += Time.deltaTime;
                if (curDashCoolTime >= DASH_COOL_TIME)
                {
                    curDashNumberOfTime++;
                    curDashCoolTime = 0f;
                    Debug.Log(curDashNumberOfTime);
                }
            }
            yield return new WaitForSeconds(0.02f);
        }
    }

}

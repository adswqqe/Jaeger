using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Action<bool> isAttacking;

    [SerializeField]
    Transform attackLocation;
    [SerializeField]
    Transform catchPos;
    [SerializeField]
    float attackRange;
    [SerializeField]
    LayerMask enemis;
    [SerializeField]
    GameObject GoShadowCircle;
    [SerializeField]
    PlayerWeaponAttack playerWeaponAttack;
    [SerializeField]
    RuntimeAnimatorController playerNoWeaponAnimator;
   
    public CameraShakManager cameraShak;

    Animator anim;
    Rigidbody2D rb;
    GameObject catchMonsterObj;
    ShadowCircle shadowCircle;

    [SerializeField]
    float comboRate = 0.4f;

    float lastPushPunchBtnTime;
    int attackCounter = 0;
    float LastAttack = .0f;
    float resetTimer = 0f;
    int comboIndex = 0;
    bool isCatching = false;
    bool isStart = false;
    public string[] comboParams = { "Attack", "Attack2", "Attack3" };


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        isStart = true;

    }

    private void OnEnable()
    {
        if (isStart)
            anim.runtimeAnimatorController = playerNoWeaponAnimator;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();

    }

    void PlayerInput()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            StillWeapon();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCatching)
            {
                MyAnimSetTrigger("Catch");
                isAttacking?.Invoke(true);
                Collider2D[] hits = Physics2D.OverlapCircleAll(attackLocation.position, attackRange, enemis);
                int noEnemyCount = 0;
                foreach (var enemy in hits)
                {
                    if (enemy.GetComponent<Monster>().IsCatchAble())
                    {
                        isCatching = true;
                        isAttacking?.Invoke(true);
                        catchMonsterObj = enemy.gameObject;
                        catchMonsterObj.GetComponent<Monster>().Catch(catchPos.position, isCatching, GetComponent<SpriteRenderer>().flipX);
                        break;
                    }
                    else
                        noEnemyCount++;
                }
                
                if(noEnemyCount >= hits.Length)
                    StartCoroutine("AttackEnableDelay", "Catch");

            }
            else
            {
                MyAnimSetTrigger("Throw");
                StartCoroutine("AttackEnableDelay", "Throw");
                isCatching = false;
                catchMonsterObj.GetComponent<Monster>().Catch(catchPos.position, isCatching, GetComponent<SpriteRenderer>().flipX);
                catchMonsterObj = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && comboIndex < comboParams.Length && !isCatching)
        {
            isAttacking?.Invoke(true);
            MyAnimSetTrigger(comboParams[comboIndex]);
            if (comboIndex == 1)
                StartCoroutine(cameraShak.Shake());

            Collider2D[] hits = Physics2D.OverlapCircleAll(attackLocation.position, attackRange, enemis);

            if (hits != null)
            {
                foreach (var enemy in hits)
                {
                    enemy.SendMessage("TakeDamage", 30);
                }
            }
            comboIndex++;
            resetTimer = 0f;
        }

        if (comboIndex > 0)
        {
            resetTimer += Time.deltaTime;
            if (resetTimer > comboRate)
            {
                //if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0)
                //    MyAnimSetTrigger("Walk");
                //else
                //    MyAnimSetTrigger("Reset");
                if (comboIndex >= 3)
                    comboIndex = 2;
                StartCoroutine("AttackEnableDelay", comboParams[comboIndex]);
                comboIndex = 0;
            }
        }
    }

    void StillWeapon()
    {
        //shadowCircle = GameObject.Instantiate(GoShadowCircle, new Vector3(transform.position.x , (GetComponent<SpriteRenderer>().size.y / 2) + transform.position.y, transform.position.z)
        //                                                                , Quaternion.identity);
        shadowCircle = Instantiate(GoShadowCircle, new Vector3(attackLocation.position.x, (GetComponent<SpriteRenderer>().size.y / 2) + transform.position.y, transform.position.z)
                                                                        , Quaternion.identity).GetComponent<ShadowCircle>();
        shadowCircle.isLeft = GetComponent<SpriteRenderer>().flipX;
        shadowCircle.StillMonsterWeapon += OnStillWeapon;
        
    }

    bool isPlayeringAnim(string AnimName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(AnimName))
        {
            return true;
        }
        return false;
    }

    void MyAnimSetTrigger(string AnimName)
    {
        if (!isPlayeringAnim(AnimName))
        {
            anim.SetTrigger(AnimName);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackLocation.position, attackRange);
    }

    void OnStillWeapon()
    {
        shadowCircle.StillMonsterWeapon -= OnStillWeapon;
        playerWeaponAttack.enabled = true;
        this.enabled = false;
        
    }

    IEnumerator AttackEnableDelay(string animName)
    {
        while(true)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f && anim.GetCurrentAnimatorStateInfo(0).IsName(animName) || ((isPlayeringAnim("Idle") || isPlayeringAnim("Walk") || isPlayeringAnim("Jump"))))
            //if (isPlayeringAnim("Idle") || isPlayeringAnim("Walk") || isPlayeringAnim("Jump"))
            {
                if (rb.velocity.y > 0.01)
                {
                    MyAnimSetTrigger("Jump");
                }
                else if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0)
                {
                    MyAnimSetTrigger("Walk");
                }
                else
                {
                    MyAnimSetTrigger("Reset");
                }


                isAttacking?.Invoke(false);
                StopCoroutine("AttackEnableDelay");
            }

            yield return new WaitForSeconds(0.02f);
        }
    }
}

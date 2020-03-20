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

    Animator anim;
    Rigidbody2D rb;
    GameObject catchMonsterObj;

    [SerializeField]
    float comboRate = 0.4f;

    float lastPushPunchBtnTime;
    int attackCounter = 0;
    float LastAttack = .0f;
    float resetTimer = 0f;
    int comboIndex = 0;
    bool isCatching = false;
    public string[] comboParams = { "Attack", "Attack2", "Attack3" };


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();

        attackLocation.localScale = new Vector3(1, -1, 1);
    }

    void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCatching)
            {
                MyAnimSetTrigger("Catch");

                Collider2D[] hits = Physics2D.OverlapCircleAll(attackLocation.position, attackRange, enemis);
                foreach (var enemy in hits)
                {
                    if (enemy == null)
                        break;

                    if (enemy.GetComponent<Monster>().IsCatchAble())
                    {
                        isCatching = true;
                        isAttacking?.Invoke(true);
                        catchMonsterObj = enemy.gameObject;
                        catchMonsterObj.GetComponent<Monster>().Catch(catchPos.position, isCatching);
                        break;
                    }
                }
            }
            else
            {
                MyAnimSetTrigger("Throw");
                StartCoroutine("AttackEnableDelay");
                isCatching = false;
                catchMonsterObj.GetComponent<Monster>().Catch(catchPos.position, isCatching);
                catchMonsterObj = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && comboIndex < comboParams.Length && !isCatching)
        {
            isAttacking?.Invoke(true);
            MyAnimSetTrigger(comboParams[comboIndex]);
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
                StartCoroutine("AttackEnableDelay");
                comboIndex = 0;
            }
        }
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

    IEnumerator AttackEnableDelay()
    {
        while(true)
        {
            if (isPlayeringAnim("Idle") || isPlayeringAnim("Walk") || isPlayeringAnim("Jump"))
            {
                isAttacking?.Invoke(false);


                if (rb.velocity.y > 0.01)
                {
                    MyAnimSetTrigger("Jump");
                }
                else if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0)
                    MyAnimSetTrigger("Walk");
                else
                {
                    MyAnimSetTrigger("Reset");
                }


                StopCoroutine("AttackEnableDelay");
            }

            yield return new WaitForSeconds(0.02f);
        }
    }
}

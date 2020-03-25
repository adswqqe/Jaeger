using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponAttack : MonoBehaviour
{
    [SerializeField]
    PlayerAttack playerAttack;
    [SerializeField]
    RuntimeAnimatorController playerWeaponAnimator;
    [SerializeField]
    Transform attackLocation;
    [SerializeField]
    float attackRange;
    [SerializeField]
    LayerMask enemis;

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = playerWeaponAnimator;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            MyAnimSetTrigger("Attack");
            StartCoroutine("AttackEnableDelay", "Attack");

            Collider2D[] hits = Physics2D.OverlapCircleAll(attackLocation.position, attackRange, enemis);

            if (hits != null)
            {
                foreach (var enemy in hits)
                {
                    enemy.SendMessage("TakeDamage", 30);
                }
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

    IEnumerator AttackEnableDelay(string animName)
    {
        while (true)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f && anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
            {
                playerAttack.enabled = true;
                this.enabled = false;

                
                StopCoroutine("AttackEnableDelay");
            }

            yield return new WaitForSeconds(0.02f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PlayerMovement playerMovement;
    PlayerAttack playerAttack;
    Animator anim;

    [SerializeField]
    Transform playerAttackRangeTr;
    [SerializeField]
    Transform PlayerCatchTr;
    


    const float DOUBLE_CLICK_TIME = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = gameObject.AddComponent<PlayerMovement>();
        playerMovement.Init(playerAttackRangeTr, PlayerCatchTr);
        playerAttack = GetComponent<PlayerAttack>();
        anim = GetComponent<Animator>();
        BindEvents();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void PlayerInput()
    {

    }

    void BindEvents()
    {
        playerAttack.isAttacking += playerMovement.OnAttacking;
    }

    void UnBindEvents()
    {
        playerAttack.isAttacking -= playerMovement.OnAttacking;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Animator anim;


    [SerializeField]
    float maxHp = 100;

    float currentHp;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakeDamage(int Damage)
    {
        currentHp -= Damage;

        anim.SetTrigger("Hit");
        Debug.Log("HP : " + currentHp);
        if (currentHp < 0)
        {
            MyAnimSetTrigger("Die");
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

}

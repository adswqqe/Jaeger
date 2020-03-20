using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;

    [SerializeField]
    float maxHp = 100;
    [SerializeField]
    bool isCatchAbleMonster = true;

    bool isCatching = false;
    float currentHp;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayingAnim();
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


    public void Catch(Vector3 catchPos, bool isCatched)
    {
        if (isCatched)
        {
            isCatching = true;
            anim.SetTrigger("Hit");
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.position = new Vector3(catchPos.x, catchPos.y - 0.5f, catchPos.z);
        }
        else
        {
            isCatching = false;
            rb.bodyType = RigidbodyType2D.Dynamic;

        }

    }

    public bool IsCatchAble()
    {
        return isCatchAbleMonster;
    }

    void PlayingAnim()
    {
        if (isCatching)
            MyAnimSetTrigger("Hit");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    CircleCollider2D circleCollider;
    [SerializeField]
    float maxHp = 100;
    [SerializeField]
    bool isCatchAbleMonster = true;
    [SerializeField]
    bool isStillAbleMonster = true;
    [SerializeField]
    GameObject weapon;

    bool isCatching = false;
    float currentHp;
    bool isMonsterDirLeft;
    bool isGround;
    int whatisGround;


    // Start is called before the first frame update
    void Start()
    {
        whatisGround = 1 << LayerMask.NameToLayer("Plaform");
        currentHp = maxHp;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayingAnim();
        GroundCheck();
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


    public void Catch(Vector3 catchPos, bool isCatched, bool isLeft)
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
            StartCoroutine("Throwing");


        }

        isMonsterDirLeft = isLeft;
    }

    public bool IsCatchAble()
    {
        return isCatchAbleMonster;
    }

    public bool isStillAble()
    {
        return isStillAbleMonster;
    }

    public void StilledWeapon()
    {
        weapon.SetActive(false);
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

    void GroundCheck()
    {
        if (Physics2D.Raycast(circleCollider.bounds.center, Vector2.down, 0.55f, whatisGround))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
        Debug.Log(isGround);
    }


    IEnumerator Throwing()
    {
        float ThrowingTime = 0;
        while(true)
        {
            ThrowingTime += Time.deltaTime;
            if (isMonsterDirLeft)
                rb.AddForce(new Vector2(-1, 0.3f) * 300f);
            else
                rb.AddForce(new Vector2(1, 0.3f) * 300f);

            if (isGround || ThrowingTime > 1.0f)
            {
                rb.velocity = Vector2.zero;
                break;
            }

            yield return new WaitForSeconds(0.02f);
        }
    }

}

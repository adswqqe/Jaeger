using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCircle : MonoBehaviour
{
    public Action StillMonsterWeapon;
    public bool isLeft;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLeft)
            rb.velocity = Vector2.left * 10f;
        else
            rb.velocity = Vector2.right * 10f;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Monster")
        {
            if(collision.gameObject.GetComponent<Monster>().isStillAble())
            {
                StillMonsterWeapon?.Invoke();
                collision.gameObject.GetComponent<Monster>().StilledWeapon();
            }
        }

        Destroy(this.gameObject);
    }
}

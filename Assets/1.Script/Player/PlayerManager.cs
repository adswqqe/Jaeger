using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            AttackPunch();
        }
    }

    void AttackPunch()
    {

    }
}

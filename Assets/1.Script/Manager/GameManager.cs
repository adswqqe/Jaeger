using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera cmVcam;
    [SerializeField]
    PlayerManager player;
    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }

    void InitGame()
    {
        var Playerobj = Instantiate(player, Vector3.zero, Quaternion.identity);
        cmVcam.Follow = Playerobj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

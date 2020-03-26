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
    [SerializeField]
    Transform playerSpawnPos;
    [SerializeField]
    CameraShakManager cameraShak;
    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }

    void InitGame()
    {
        var Playerobj = Instantiate(player, playerSpawnPos.position, Quaternion.identity);
        cmVcam.Follow = Playerobj.transform;
        Playerobj.GetComponent<PlayerAttack>().cameraShak = cameraShak;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

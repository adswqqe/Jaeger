using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CameraShakManager : MonoBehaviour
{
    public float ShakeDuration = 0.3f;
    public float ShakeAmplitude = 1.2f;
    public float ShakeFrequency = 2.0f;

    private float ShakeElapsedTime = 0f;

    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    private void Start()
    {
        if (virtualCamera != null)
            virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            ShakeElapsedTime = ShakeDuration;
        }

        
    }

    public IEnumerator Shake()
    {
        ShakeElapsedTime = ShakeDuration;
        while (true)
        {
            if (virtualCamera != null || virtualCameraNoise != null)
            {
                if (ShakeElapsedTime > 0)
                {
                    virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
                    virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

                    ShakeElapsedTime -= Time.deltaTime;
                }
                else
                {
                    virtualCameraNoise.m_AmplitudeGain = 0f;
                    ShakeElapsedTime = 0f;
                    Debug.Log("asdasd");
                    break;
                }
            }

            yield return null;
        }
    }
}

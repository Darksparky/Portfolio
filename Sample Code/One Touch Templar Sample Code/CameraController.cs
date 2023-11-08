using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    private CinemachineVirtualCamera vCam;
    private float effectTimer = 0;
    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin perlin =
               vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = 0f;
    }

    // Update is called once per frame
    void Update()
    { 
        if (effectTimer > 0)
        {
            effectTimer -= Time.deltaTime;
            if(effectTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin perlin =
                vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                perlin.m_AmplitudeGain = 0f;
            }
        }
    }
  

/*    public void ShakeMe()
    {
        gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + yOffset, player.transform.position.z - zOffset);
        gameObject.GetComponent<Shake>().start = true;
    }
*/
    public void ShakeMe(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin perlin =
        vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = intensity;
        effectTimer = time;
    }
    
    
}

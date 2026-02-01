using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    private CinemachineBasicMultiChannelPerlin perlin;
    private float shakeTimer;

    private void Awake()
    {
        Instance = this;
        perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float t_intensity, float t_time)
    {
        perlin.AmplitudeGain = t_intensity;
        shakeTimer = t_time;
    }

    private void Update()
    {
        perlin.FrequencyGain = shakeTimer;

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                

                perlin.AmplitudeGain = 0f;
            }
        }
    }
}
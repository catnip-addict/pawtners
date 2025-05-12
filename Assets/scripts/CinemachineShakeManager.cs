using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CinemachineShakeManager : MonoBehaviour
{
    public static CinemachineShakeManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

    }

    public void Shake(CinemachineCamera cam, float intensity, float duration)
    {
        StartCoroutine(ShakeCoroutine(cam, intensity, duration));
    }

    private IEnumerator ShakeCoroutine(CinemachineCamera cam, float intensity, float duration)
    {
        if (cam == null) yield break;

        if (!cam.TryGetComponent<CinemachineBasicMultiChannelPerlin>(out var perlin)) yield break;

        perlin.AmplitudeGain = intensity;

        float timer = duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        perlin.AmplitudeGain = 0f;
    }
}

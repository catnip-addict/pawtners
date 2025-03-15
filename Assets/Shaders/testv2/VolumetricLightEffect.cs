using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Effects/Volumetric Light Screen")]
public class VolumetricLightEffect : MonoBehaviour
{
    [Header("Rendering Settings")]
    [Range(1, 4)]
    public int downsample = 2;
    [Range(16, 128)]
    public int sampleCount = 64;
    [Range(1, 10)]
    public float stepOptimize = 2f;

    [Header("Light Settings")]
    public Color lightColor = Color.white;
    [Range(0, 10)]
    public float lightIntensity = 1.0f;
    [Range(0.01f, 5.0f)]
    public float density = 1.0f;
    [Range(0, 1)]
    public float scattering = 0.5f;

    [Header("Noise Settings")]
    [Range(0.1f, 10.0f)]
    public float noiseScale = 1.0f;
    [Range(0, 1)]
    public float noiseSpeed = 0.1f;

    // References
    private Material volumetricMaterial;
    private Camera cam;
    private RenderTexture volumeRT;

    // Shader properties
    private static readonly int _InverseViewMatrix = Shader.PropertyToID("_InverseViewMatrix");
    private static readonly int _InverseProjectionMatrix = Shader.PropertyToID("_InverseProjectionMatrix");
    private static readonly int _LightColor = Shader.PropertyToID("_LightColor");
    private static readonly int _LightIntensity = Shader.PropertyToID("_LightIntensity");
    private static readonly int _SampleCount = Shader.PropertyToID("_SampleCount");
    private static readonly int _Density = Shader.PropertyToID("_Density");
    private static readonly int _ScatteringCoefficient = Shader.PropertyToID("_ScatteringCoefficient");
    private static readonly int _NoiseScale = Shader.PropertyToID("_NoiseScale");
    private static readonly int _NoiseSpeed = Shader.PropertyToID("_NoiseSpeed");
    private static readonly int _StepOptimize = Shader.PropertyToID("_StepOptimize");

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode |= DepthTextureMode.Depth;

        Shader shader = Shader.Find("Hidden/VolumetricLightScreen");
        if (shader == null)
        {
            Debug.LogError("SHADER NOT FOUND: 'Hidden/VolumetricLightScreen'");
            enabled = false;
            return;
        }

        volumetricMaterial = new Material(shader);
        Debug.Log("Shader found and material created successfully");
    }

    void Start()
    {
        // Force camera to generate depth texture
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    void OnDisable()
    {
        if (volumeRT != null)
        {
            RenderTexture.ReleaseTemporary(volumeRT);
            volumeRT = null;
        }

        if (volumetricMaterial != null)
        {
            DestroyImmediate(volumetricMaterial);
            volumetricMaterial = null;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (volumetricMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // Update shader parameters
        volumetricMaterial.SetMatrix(_InverseViewMatrix, cam.cameraToWorldMatrix);
        volumetricMaterial.SetMatrix(_InverseProjectionMatrix, cam.projectionMatrix.inverse);
        volumetricMaterial.SetColor(_LightColor, lightColor);
        volumetricMaterial.SetFloat(_LightIntensity, lightIntensity);
        volumetricMaterial.SetInt(_SampleCount, sampleCount);
        volumetricMaterial.SetFloat(_Density, density);
        volumetricMaterial.SetFloat(_ScatteringCoefficient, scattering);
        volumetricMaterial.SetFloat(_NoiseScale, noiseScale);
        volumetricMaterial.SetFloat(_NoiseSpeed, noiseSpeed);
        volumetricMaterial.SetFloat(_StepOptimize, stepOptimize);

        // Create downsampled render texture for better performance
        int rtW = source.width / downsample;
        int rtH = source.height / downsample;

        volumeRT = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);

        // Blit through the effect at lower resolution
        Graphics.Blit(source, volumeRT, volumetricMaterial);

        // Blit back to destination
        Graphics.Blit(volumeRT, destination);

        // Clean up
        RenderTexture.ReleaseTemporary(volumeRT);
    }
}

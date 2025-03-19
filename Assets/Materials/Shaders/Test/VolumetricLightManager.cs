using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class VolumetricLightManager : MonoBehaviour
{
    public enum VolumetricResolution { Full, Half, Quarter, Eighth }

    [Header("Volumetric Settings")]
    public VolumetricResolution resolution = VolumetricResolution.Half;
    [Range(16, 128)]
    public int sampleCount = 64;
    [Range(0, 10)]
    public float lightIntensityMultiplier = 2.0f;
    [Range(0, 10)]
    public float densityMultiplier = 1.0f;
    [Range(0, 1)]
    public float scatteringCoefficient = 0.5f;
    [Range(0, 1)]
    public float extinctionCoefficient = 0.1f;
    [Range(0, 10)]
    public float noiseScale = 1.0f;
    [Range(0, 5)]
    public float noiseSpeed = 0.5f;
    [Range(0, 1)]
    public float jitterStrength = 0.5f;
    [Range(1, 100)]
    public float maxRayLength = 50f;

    [Header("References")]
    public Shader volumetricShader;

    private Light lightComponent;
    private Material volumetricMaterial;
    private Camera mainCamera;
    private RenderTexture volumeRT;

    void OnEnable()
    {
        lightComponent = GetComponent<Light>();

        if (volumetricShader == null)
            volumetricShader = Shader.Find("Custom/VolumetricLight");

        if (volumetricShader != null && volumetricMaterial == null)
            volumetricMaterial = new Material(volumetricShader);

        // Find the main camera
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Make sure camera has depth texture enabled
        if (mainCamera != null)
            mainCamera.depthTextureMode |= DepthTextureMode.Depth;

        // Register to rendering pipeline
        Camera.onPostRender += OnCameraPostRender;
    }

    void OnDisable()
    {
        Camera.onPostRender -= OnCameraPostRender;

        if (volumeRT != null)
        {
            volumeRT.Release();
            volumeRT = null;
        }
    }

    void Update()
    {
        // Only update if we have all required components
        if (volumetricMaterial == null || lightComponent == null || mainCamera == null)
            return;

        // Update material parameters
        volumetricMaterial.SetVector("_LightPos", lightComponent.transform.position);
        volumetricMaterial.SetVector("_LightDir", lightComponent.transform.forward);
        volumetricMaterial.SetColor("_LightColor", lightComponent.color);
        volumetricMaterial.SetFloat("_LightIntensity", lightComponent.intensity * lightIntensityMultiplier);
        volumetricMaterial.SetFloat("_DensityMultiplier", densityMultiplier);
        volumetricMaterial.SetFloat("_SpotAngle", lightComponent.spotAngle);
        volumetricMaterial.SetFloat("_ScatteringCoef", scatteringCoefficient);
        volumetricMaterial.SetFloat("_ExtinctionCoef", extinctionCoefficient);
        volumetricMaterial.SetFloat("_SampleCount", sampleCount);
        volumetricMaterial.SetFloat("_NoiseScale", noiseScale);
        volumetricMaterial.SetFloat("_NoiseSpeed", noiseSpeed);
        volumetricMaterial.SetFloat("_JitterStrength", jitterStrength);
        volumetricMaterial.SetFloat("_MaxRayLength", maxRayLength);

        // Set downsample factor based on resolution enum
        int downsampleFactor = 1;
        switch (resolution)
        {
            case VolumetricResolution.Half: downsampleFactor = 2; break;
            case VolumetricResolution.Quarter: downsampleFactor = 4; break;
            case VolumetricResolution.Eighth: downsampleFactor = 8; break;
        }
        volumetricMaterial.SetFloat("_DownsampleFactor", downsampleFactor);

        // Create or resize render texture
        int rtWidth = mainCamera.pixelWidth / downsampleFactor;
        int rtHeight = mainCamera.pixelHeight / downsampleFactor;

        if (volumeRT == null || volumeRT.width != rtWidth || volumeRT.height != rtHeight)
        {
            if (volumeRT != null)
                volumeRT.Release();

            volumeRT = new RenderTexture(rtWidth, rtHeight, 0, RenderTextureFormat.ARGBHalf);
            volumeRT.filterMode = FilterMode.Bilinear;
            volumeRT.wrapMode = TextureWrapMode.Clamp;
            volumeRT.Create();
        }
    }

    void OnPostRender()
    {
        // Unity-provided message function with the correct signature
        // This will be called when this GameObject's camera finishes rendering
        if (volumetricMaterial == null || volumeRT == null)
            return;

        RenderVolumetricEffect();
    }

    void OnCameraPostRender(Camera cam)
    {
        if (cam != mainCamera || volumetricMaterial == null || volumeRT == null)
            return;

        RenderVolumetricEffect();
    }

    void RenderVolumetricEffect()
    {
        // Draw volumetric effect into the render texture
        Graphics.Blit(null, volumeRT, volumetricMaterial);

        // Upscale and blend with the screen
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = currentRT;

        GL.PushMatrix();
        GL.LoadOrtho();

        volumetricMaterial.SetTexture("_MainTex", volumeRT);
        volumetricMaterial.SetPass(0);

        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0);
        GL.Vertex3(0, 0, 0);

        GL.TexCoord2(1, 0);
        GL.Vertex3(1, 0, 0);

        GL.TexCoord2(1, 1);
        GL.Vertex3(1, 1, 0);

        GL.TexCoord2(0, 1);
        GL.Vertex3(0, 1, 0);
        GL.End();

        GL.PopMatrix();
    }

    void OnDrawGizmosSelected()
    {
        // Draw the cone visualization for spot lights
        if (lightComponent != null && lightComponent.type == LightType.Spot)
        {
            Gizmos.color = new Color(lightComponent.color.r, lightComponent.color.g, lightComponent.color.b, 0.2f);

            float angle = lightComponent.spotAngle * 0.5f * Mathf.Deg2Rad;
            float length = maxRayLength;
            float radius = Mathf.Tan(angle) * length;

            Vector3 forward = transform.forward * length;
            DrawCone(transform.position, forward, radius);
        }
    }

    private void DrawCone(Vector3 position, Vector3 direction, float radius)
    {
        Vector3 up = Vector3.Cross(direction, Vector3.right).normalized;
        if (up.magnitude < 0.001f)
            up = Vector3.Cross(direction, Vector3.forward).normalized;

        Vector3 right = Vector3.Cross(up, direction).normalized;

        int segments = 12;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * (2 * Mathf.PI / segments);
            float angle2 = (i + 1) * (2 * Mathf.PI / segments);

            Vector3 point1 = position + direction + (up * Mathf.Sin(angle1) + right * Mathf.Cos(angle1)) * radius;
            Vector3 point2 = position + direction + (up * Mathf.Sin(angle2) + right * Mathf.Cos(angle2)) * radius;

            Gizmos.DrawLine(position, point1);
            Gizmos.DrawLine(point1, point2);
        }
    }
}
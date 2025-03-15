using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Effects/Simple Volumetric Light")]
public class SimpleVolumetricEffect : MonoBehaviour
{
    [Header("Rendering Settings")]
    [Range(1, 4)] public int downsample = 2;
    [Range(8, 64)] public int sampleCount = 32;
    
    [Header("Light Settings")]
    public Color lightColor = Color.white;
    [Range(0, 10)] public float lightIntensity = 1.0f;
    [Range(0.01f, 5.0f)] public float density = 0.5f;
    
    [Header("Noise Settings")]
    [Range(0.1f, 10.0f)] public float noiseScale = 1.0f;
    [Range(0, 1)] public float noiseSpeed = 0.1f;
    
    [Header("Debug")]
    public bool showInEditor = true;
    
    private Material volumetricMaterial;
    private Camera cam;
    
    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        
        // Make sure camera is set up to render depth
        cam.depthTextureMode |= DepthTextureMode.Depth;
        
        // Create shader material if needed
        if (volumetricMaterial == null)
        {
            Shader shader = Shader.Find("Hidden/SimplifiedVolumetricLight");
            if (shader == null)
            {
                Debug.LogError("Could not find shader 'Hidden/SimplifiedVolumetricLight'");
                enabled = false;
                return;
            }
            volumetricMaterial = new Material(shader);
        }
    }
    
    private void OnDisable()
    {
        if (volumetricMaterial != null) 
        {
            DestroyImmediate(volumetricMaterial);
            volumetricMaterial = null;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (volumetricMaterial == null || (!Application.isPlaying && !showInEditor))
        {
            Graphics.Blit(source, destination);
            return;
        }
        
        // Update material properties
        volumetricMaterial.SetColor("_LightColor", lightColor);
        volumetricMaterial.SetFloat("_LightIntensity", lightIntensity);
        volumetricMaterial.SetInt("_SampleCount", sampleCount);
        volumetricMaterial.SetFloat("_Density", density);
        volumetricMaterial.SetFloat("_NoiseScale", noiseScale);
        volumetricMaterial.SetFloat("_NoiseSpeed", noiseSpeed);
        
        // Create temp render texture with downsampling for better performance
        int rtW = source.width / downsample;
        int rtH = source.height / downsample;
        RenderTexture temp = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
        
        // Process effect at lower resolution
        Graphics.Blit(source, temp, volumetricMaterial);
        
        // Blit back to destination
        Graphics.Blit(temp, destination);
        
        // Release temporary RT
        RenderTexture.ReleaseTemporary(temp);
    }
}

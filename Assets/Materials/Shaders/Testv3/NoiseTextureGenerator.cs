using UnityEngine;

[ExecuteInEditMode]
public class NoiseTextureGenerator : MonoBehaviour
{
    public int textureSize = 256;
    public bool regenerateOnStart = true;
    public bool useAsGlobal = true;
    
    [Range(1, 10)]
    public float noiseScale = 1.0f;
    
    private Texture2D noiseTexture;
    
    private void Start()
    {
        if (regenerateOnStart)
        {
            GenerateNoiseTexture();
        }
    }
    
    [ContextMenu("Generate Noise Texture")]
    public void GenerateNoiseTexture()
    {
        // Create texture
        noiseTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        noiseTexture.wrapMode = TextureWrapMode.Repeat;
        noiseTexture.filterMode = FilterMode.Bilinear;
        
        // Fill with random values
        Color[] pixels = new Color[textureSize * textureSize];
        
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float perlinValue = Mathf.PerlinNoise(
                    (float)x / textureSize * noiseScale, 
                    (float)y / textureSize * noiseScale
                );
                
                float perlinValue2 = Mathf.PerlinNoise(
                    (float)x / textureSize * noiseScale * 2.3f + 0.5f, 
                    (float)y / textureSize * noiseScale * 2.3f + 0.5f
                );
                
                pixels[y * textureSize + x] = new Color(perlinValue, perlinValue2, 0, 1);
            }
        }
        
        noiseTexture.SetPixels(pixels);
        noiseTexture.Apply();
        
        // Set as global texture 
        if (useAsGlobal)
        {
            Shader.SetGlobalTexture("_NoiseTexture", noiseTexture);
        }
        
        Debug.Log("Noise texture generated: " + textureSize + "x" + textureSize);
    }
    
    private void OnDestroy()
    {
        if (noiseTexture != null)
        {
            DestroyImmediate(noiseTexture);
        }
    }
}

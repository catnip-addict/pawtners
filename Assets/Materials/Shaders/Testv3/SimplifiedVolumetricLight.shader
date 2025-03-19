Shader "Hidden/SimplifiedVolumetricLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _LightIntensity ("Light Intensity", Range(0, 10)) = 1
        _SampleCount ("Sample Count", Range(8, 64)) = 32
        _Density ("Density", Range(0.01, 5.0)) = 0.5
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1.0
        _NoiseSpeed ("Noise Speed", Range(0, 1)) = 0.1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewVector : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D_float _CameraDepthTexture;
            
            float4 _MainTex_ST;
            float4 _LightColor;
            float _LightIntensity;
            int _SampleCount;
            float _Density;
            float _NoiseScale;
            float _NoiseSpeed;
            
            // Simple noise function
            float noise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float2 uv = (i.xy + float2(37.0, 17.0) * i.z) + f.xy;
                float2 rg = tex2Dlod(_MainTex, float4((uv + 0.5) / 256.0, 0, 0)).yx;
                return lerp(rg.x, rg.y, f.z);
            }
            
            // Calculate view vector for ray marching
            float3 GetViewVector(float2 uv)
            {
                float3 viewVector = float3((uv * 2.0 - 1.0) * float2(1, _ProjectionParams.x), 1.0);
                return mul(unity_CameraInvProjection, float4(viewVector, 1.0)).xyz;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                // Calculate view ray direction for pixel
                o.viewVector = GetViewVector(v.uv);
                
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                // Sample the original color
                float4 mainColor = tex2D(_MainTex, i.uv);
                
                // Get the scene depth
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float linearDepth = LinearEyeDepth(depth);
                
                // Calculate ray direction for this pixel
                float3 rayDir = normalize(i.viewVector);
                
                // Setup ray marching parameters
                float stepSize = linearDepth / _SampleCount;
                float3 rayStep = rayDir * stepSize;
                float3 rayPos = _WorldSpaceCameraPos;
                
                // Light direction in world space
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                
                // Accumulated light
                float accum = 0.0;
                
                // Perform ray marching
                for (int step = 0; step < _SampleCount; step++)
                {
                    // Move along the ray
                    rayPos += rayStep;
                    
                    // Sample noise at this position
                    float n = noise(rayPos * _NoiseScale + float3(_Time.y * _NoiseSpeed, 0, 0));
                    
                    // Simple lighting model: more density around light direction
                    float d = max(dot(normalize(rayDir), normalize(lightDir)), 0.0);
                    float density = n * d * _Density;
                    
                    // Accumulate light contribution
                    accum += density * stepSize;
                    
                    // Early termination
                    if (step * stepSize > linearDepth)
                        break;
                }
                
                // Apply light color and intensity
                float4 volumeLight = _LightColor * accum * _LightIntensity;
                
                // Blend with the original color (additive blend)
                float4 finalColor = mainColor + volumeLight;
                
                return finalColor;
            }
            ENDCG
        }
    }
}

// Create a file named VolumetricLightScreen.shader
Shader "Hidden/VolumetricLightScreen"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                float3 viewRay : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4x4 _InverseViewMatrix;
            float4x4 _InverseProjectionMatrix;
            float4 _LightColor;
            float _LightIntensity;
            int _SampleCount;
            float _Density;
            float _ScatteringCoefficient;
            float _NoiseScale;
            float _NoiseSpeed;
            float _StepOptimize;

            // Hash function must be defined before it's used!
            float hash(float n)
            {
                return frac(sin(n) * 43758.5453);
            }

            // Simple noise function
            float noise(float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);
                float n = p.x + p.y * 57.0 + 113.0 * p.z;
                float res = lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
                                     lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
                                lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
                                     lerp(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
                return res;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                // Get view ray for volumetric sampling
                float4 clipPos = float4(v.uv * 2.0 - 1.0, 1.0, 1.0);
                float4 viewRay = mul(_InverseProjectionMatrix, clipPos);
                o.viewRay = viewRay.xyz / viewRay.w;
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample scene color and depth
                fixed4 sceneColor = tex2D(_MainTex, i.uv);
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float linearDepth = LinearEyeDepth(depth);
                
                // Get world position from depth and view ray
                float3 worldPos = _WorldSpaceCameraPos + linearDepth * i.viewRay;
                
                // Ray marching setup
                float3 rayStart = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewRay);
                float rayLength = linearDepth;
                float stepSize = rayLength / _SampleCount * _StepOptimize;
                
                // Volumetric lighting accumulation
                float3 light = float3(0, 0, 0);
                float transmittance = 1.0;
                
                // Ray marching loop
                for (int i = 0; i < _SampleCount; i++)
                {
                    float3 samplePos = rayStart + rayDir * stepSize * i;
                    
                    // Add noise variation
                    float noiseVal = noise(samplePos * _NoiseScale + _Time.y * _NoiseSpeed);
                    float density = _Density * (0.5 + 0.5 * noiseVal);
                    
                    // Calculate light contribution at this point
                    float scattering = _ScatteringCoefficient * density;
                    float3 lightContrib = _LightColor.rgb * _LightIntensity * scattering;
                    
                    // Accumulate light with transmittance
                    light += lightContrib * transmittance;
                    
                    // Update transmittance
                    transmittance *= exp(-density * stepSize);
                    
                    // Early termination for efficiency
                    if (transmittance < 0.01)
                        break;
                }
                
                // Blend with original scene
                return fixed4(sceneColor.rgb + light, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
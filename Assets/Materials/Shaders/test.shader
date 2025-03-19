Shader "Custom/VolumetricLight2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _LightIntensity ("Light Intensity", Range(0, 10)) = 1
        _SampleCount ("Sample Count", Range(16, 128)) = 64
        _Density ("Density", Range(0.01, 5.0)) = 1.0
        _ScatteringCoefficient ("Scattering", Range(0, 1)) = 0.5
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1.0
        _NoiseSpeed ("Noise Speed", Range(0, 1)) = 0.1
        _Downsample ("Downsample", Range(1, 4)) = 2
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend One One
        ZWrite Off
        Cull Front
        
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
                float3 worldPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _LightColor;
            float _LightIntensity;
            int _SampleCount;
            float _Density;
            float _ScatteringCoefficient;
            float _NoiseScale;
            float _NoiseSpeed;
            int _Downsample;
            
            // Light position is the camera position for now
            float3 _LightPos;
            
            // Simple hash function for noise
            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }
            
            // 3D noise function
            float noise(float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);
                
                float n = p.x + p.y * 157.0 + 113.0 * p.z;
                return lerp(lerp(lerp(hash(p + float3(0, 0, 0)), 
                                     hash(p + float3(1, 0, 0)), f.x),
                               lerp(hash(p + float3(0, 1, 0)), 
                                     hash(p + float3(1, 1, 0)), f.x), f.y),
                          lerp(lerp(hash(p + float3(0, 0, 1)), 
                                     hash(p + float3(1, 0, 1)), f.x),
                               lerp(hash(p + float3(0, 1, 1)), 
                                     hash(p + float3(1, 1, 1)), f.x), f.y), f.z);
            }
            
            // Fbm (Fractal Brownian Motion) for more interesting noise
            float fbm(float3 p)
            {
                float f = 0.0;
                float a = 0.5;
                
                for (int i = 0; i < 4; i++)
                {
                    f += a * noise(p);
                    p *= 2.0;
                    a *= 0.5;
                }
                
                return f;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(o.worldPos - _WorldSpaceCameraPos);
                return o;
            }
            
            float sampleDensity(float3 pos)
            {
                // Add some animated noise to the density
                float noise = fbm(pos * _NoiseScale + _Time.y * _NoiseSpeed);
                return _Density * noise;
            }
            
            float calculateLight(float3 start, float3 dir, float rayLength)
            {
                // Ray marching parameters
                float stepSize = rayLength / _SampleCount;
                float3 step = dir * stepSize;
                float3 currentPos = start;
                
                // Accumulated light
                float transmittance = 1.0;
                float lightEnergy = 0.0;
                
                // Get the main light direction
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                
                // March along the ray
                for (int i = 0; i < _SampleCount; i++)
                {
                    // Sample density at current position
                    float density = sampleDensity(currentPos);
                    
                    if (density > 0.0)
                    {
                        // Calculate light contribution at this step
                        float lightTransmittance = exp(-density * stepSize);
                        transmittance *= lightTransmittance;
                        
                        // Phase function (simplified Henyey-Greenstein)
                        float cosAngle = dot(dir, lightDir);
                        float phase = 0.5 + 0.5 * _ScatteringCoefficient * cosAngle;
                        
                        // Accumulate light energy
                        lightEnergy += density * stepSize * transmittance * phase * _LightIntensity;
                        
                        // Early termination for optimization
                        if (transmittance < 0.01)
                            break;
                    }
                    
                    // Move to next position
                    currentPos += step;
                }
                
                return lightEnergy;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Ray origin and direction
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.worldPos - rayOrigin);
                
                // Calculate ray length (distance to intersection with volume)
                float rayLength = length(i.worldPos - rayOrigin);
                
                // Calculate volumetric lighting
                float light = calculateLight(rayOrigin, rayDir, rayLength);
                
                // Apply light color
                fixed4 col = _LightColor * light;
                
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}
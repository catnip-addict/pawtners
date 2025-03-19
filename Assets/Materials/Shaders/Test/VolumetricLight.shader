Shader "Custom/VolumetricLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _LightPos ("Light Position", Vector) = (0,0,0,1)
        _LightDir ("Light Direction", Vector) = (0,0,1,0)
        _LightIntensity ("Light Intensity", Range(0, 10)) = 2.0
        _DensityMultiplier ("Density Multiplier", Range(0, 10)) = 1.0
        _SpotAngle ("Spot Angle", Range(1, 179)) = 45.0
        _ScatteringCoef ("Scattering Coefficient", Range(0, 1)) = 0.5
        _ExtinctionCoef ("Extinction Coefficient", Range(0, 1)) = 0.1
        _SampleCount ("Sample Count", Range(16, 128)) = 64
        _NoiseScale ("Noise Scale", Range(0, 10)) = 1.0
        _NoiseSpeed ("Noise Speed", Range(0, 5)) = 0.5
        _DownsampleFactor ("Downsample Factor", Range(1, 8)) = 2
        _MaxRayLength ("Max Ray Length", Range(1, 100)) = 50
        _JitterStrength ("Jitter Strength", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend One One
        ZTest Always
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
                float4 screenPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 _LightColor;
            float4 _LightPos;
            float4 _LightDir;
            float _LightIntensity;
            float _DensityMultiplier;
            float _SpotAngle;
            float _ScatteringCoef;
            float _ExtinctionCoef;
            int _SampleCount;
            float _NoiseScale;
            float _NoiseSpeed;
            float _DownsampleFactor;
            float _MaxRayLength;
            float _JitterStrength;
            
            // Simple hash function for jittering
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            // 3D noise function
            float noise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float2 uv = (i.xy + float2(37.0, 17.0) * i.z) + f.xy;
                float2 rg = lerp(
                    lerp(hash(uv + float2(0.0, 0.0)), hash(uv + float2(1.0, 0.0)), f.x),
                    lerp(hash(uv + float2(0.0, 1.0)), hash(uv + float2(1.0, 1.0)), f.x),
                    f.y);
                return lerp(rg.x, rg.y, f.z);
            }
            
            // Phase function: Henyey-Greenstein
            float phaseHG(float cosTheta, float g)
            {
                float g2 = g * g;
                return (1.0 - g2) / pow(1.0 + g2 - 2.0 * g * cosTheta, 1.5);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Get screen UV and depth
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV));
                
                // Ray setup
                float3 worldPos = i.worldPos;
                float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos);
                
                // Ray length limiter
                float rayLength = min(depth, _MaxRayLength);
                
                // Calculate ray start and end points
                float3 rayStart = _WorldSpaceCameraPos;
                float3 rayEnd = rayStart + viewDir * rayLength;
                
                // Step size calculation
                float stepSize = rayLength / _SampleCount;
                
                // Jitter the ray start position to reduce banding
                float jitter = hash(screenUV + _Time.xx) * _JitterStrength;
                rayStart += viewDir * stepSize * jitter;
                
                // Initialize accumulated light
                float4 result = float4(0, 0, 0, 0);
                float transmittance = 1.0;
                
                // Adaptive step size based on distance from camera
                float adaptiveStepSizeFactor = 1.0;
                
                // Raymarch through volume
                for (int step = 0; step < _SampleCount; step++)
                {
                    // Calculate current position
                    float t = step / float(_SampleCount);
                    adaptiveStepSizeFactor = 1.0 + t * 2.0; // Steps get larger as we go further
                    float currentStepSize = stepSize * adaptiveStepSizeFactor;
                    
                    float3 currentPos = rayStart + viewDir * (step * currentStepSize);
                    
                    // Calculate light direction and distance
                    float3 lightDir = normalize(_LightPos.xyz - currentPos);
                    float lightDistance = length(_LightPos.xyz - currentPos);
                    
                    // For spot lights, check if we're in the cone
                    float cosTheta = dot(-lightDir, normalize(_LightDir.xyz));
                    float spotEffect = smoothstep(cos(radians(_SpotAngle)), cos(radians(_SpotAngle * 0.8)), cosTheta);
                    
                    // Apply directional falloff and distance attenuation
                    float attenuation = 1.0 / (1.0 + lightDistance * lightDistance * 0.1);
                    
                    // Add time-based animated noise for more realistic look
                    float3 noiseCoord = currentPos * _NoiseScale + _Time.y * _NoiseSpeed;
                    float densityNoise = noise(noiseCoord);
                    
                    // Density calculation
                    float density = densityNoise * _DensityMultiplier * spotEffect;
                    
                    // Skip low density regions
                    if (density > 0.01)
                    {
                        // Apply extinction (light absorption)
                        float extinction = density * _ExtinctionCoef;
                        
                        // Phase function for directional scattering
                        float phase = phaseHG(dot(viewDir, lightDir), 0.2);
                        
                        // Calculate in-scattered light
                        float3 scatteredLight = _LightColor.rgb * _LightIntensity * attenuation * phase 
                                                * density * _ScatteringCoef * transmittance;
                        
                        // Accumulate light and update transmittance
                        result.rgb += scatteredLight;
                        transmittance *= exp(-extinction * currentStepSize);
                        
                        // Early termination if transmittance is too low
                        if (transmittance < 0.01)
                            break;
                    }
                }
                
                // Blend with the scene color
                return result;
            }
            ENDCG
        }
    }
}
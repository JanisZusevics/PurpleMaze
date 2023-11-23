Shader "Custom/Water" {
    Properties {
        // ... existing properties ...
        _WindStrength ("Wind Strength", Float) = 0.2
        _WindFrequency ("Wind Frequency", Float) = 0.3
        _WindDirection ("Wind Direction", Vector) = (1,0,0)
        _TileWidth ("Tile Width", Float) = 1100.0
        _TileLength ("Tile Length", Float) = 1100.0
    }

    SubShader {
        Tags {
            "LightMode" = "ForwardBase"
        }

        Pass {
            Cull Off

            CGPROGRAM
            #pragma vertex vp
            #pragma fragment fp

            #include "UnityPBSLighting.cginc"
            #include "AutoLight.cginc"

            struct VertexData {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            int _ShellIndex;
            int _ShellCount;
            float _ShellLength;
            float _Density;
            float _NoiseMin, _NoiseMax;
            float _Thickness;
            float _Attenuation;
            float _OcclusionBias;
            float _ShellDistanceAttenuation;
            float _Curvature;
            float _DisplacementStrength;
            float3 _ShellColor;
            float3 _ShellDirection;
            float3 _GlobalInteractionDirection;

            float _WindStrength;
            float _WindFrequency;
            float3 _WindDirection;

            float _TileWidth;
            float _TileLength;

            float hash(uint n) {
                n = (n << 13U) ^ n;
                n = n * (n * n * 15731U + 0x789221U) + 0x1376312589U;
                return float(n & uint(0x7fffffffU)) / float(0x7fffffff);
            }

            v2f vp(VertexData v) {
                v2f i;
                float shellHeight = (float)_ShellIndex / (float)_ShellCount;
                v.vertex.xyz += v.normal.xyz * _ShellLength * shellHeight;
                i.normal = normalize(UnityObjectToWorldNormal(v.normal));
                float k = pow(shellHeight, _Curvature);
                v.vertex.xyz += _GlobalInteractionDirection * k * _DisplacementStrength;

                // Wind effect with gradual tapering and edge blending
                float windEffect = sin(_Time.y * _WindFrequency + v.vertex.x + v.vertex.z) * _WindStrength;
                float taperStart = 0.3; // Start of tapering range
                float taperEnd = 0.7; // End of tapering range
                float taperFactor = smoothstep(taperStart, taperEnd, (float)_ShellIndex / (float)_ShellCount); 
                float distX = min(v.vertex.x, _TileWidth - v.vertex.x);
                float distZ = min(v.vertex.z, _TileLength - v.vertex.z);
                float edgeDist = min(distX, distZ);
                float edgeStart = 0.1; // Start of edge blending range
                float edgeEnd = 0.3; // End of edge blending range
                float edgeFactor = smoothstep(edgeStart, edgeEnd, edgeDist);
                windEffect *= taperFactor * edgeFactor;
                v.vertex.xyz += windEffect * _WindDirection;

                i.worldPos = mul(unity_ObjectToWorld, v.vertex);
                i.pos = UnityObjectToClipPos(v.vertex);
                i.uv = v.uv;

                return i;
            }

            float4 fp(v2f i) : SV_TARGET {
                float2 newUV = i.uv * _Density;
                float2 localUV = frac(newUV) * 2 - 1;
                float localDistanceFromCenter = length(localUV);
                uint2 tid = newUV;
                uint seed = tid.x + 100 * tid.y + 100 * 10;
                float shellIndex = _ShellIndex;
                float shellCount = _ShellCount;
                float rand = lerp(_NoiseMin, _NoiseMax, hash(seed));
                float h = shellIndex / shellCount;

                int outsideThickness = (localDistanceFromCenter) > (_Thickness * (rand - h));
                if (outsideThickness && _ShellIndex > 0) discard;

                float ndotl = DotClamped(i.normal, _WorldSpaceLightPos0) * 0.5f + 0.5f;
                ndotl = ndotl * ndotl;
                float ambientOcclusion = pow(h, _Attenuation);
                ambientOcclusion += _OcclusionBias;
                ambientOcclusion = saturate(ambientOcclusion);

                return float4(_ShellColor * ndotl * ambientOcclusion, 1.0);
            }

            ENDCG
        }
    }
}

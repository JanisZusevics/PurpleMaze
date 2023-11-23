Shader "Custom/WaterForMetal" {
    Properties {
        _ShellCount ("Shell Count", Range(1, 10)) = 5
        _ShellSpacing ("Shell Spacing", Float) = 0.1
        _MainTex ("Main Texture", 2D) = "white" {}
        _Transparency ("Transparency", Range(0, 1)) = 1.0
    }

    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float layerOffset : TEXCOORD3;
            };

            float _ShellCount;
            float _ShellSpacing;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Transparency;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Using the UVs to create a layering effect
                float layerFactor = fmod(o.uv.y * _ShellCount, 1.0);
                o.layerOffset = layerFactor * _ShellSpacing;

                return o;
            }

            half4 frag(v2f i) : SV_Target {
                // Applying the layer offset to the texture coordinates
                half4 col = tex2D(_MainTex, i.uv + float2(i.layerOffset, 0));
                col.a *= _Transparency * (1.0 - i.layerOffset);
                col.rgb *= saturate(dot(normalize(i.worldNormal), _WorldSpaceLightPos0.xyz));
                return col;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
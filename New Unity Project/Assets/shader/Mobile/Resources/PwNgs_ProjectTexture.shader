// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4x4 _Object2World', a built-in variable

//author: zhang shiliang

Shader "PwNgs/ProjectTexture" {
    Properties {
        _Color("main color", Color) = (1, 1, 1, 1)
        _MainTex("main texture", 2D) = "" {}
        _range("range", Range(0.1, 2)) = 1
        _center("center", Vector) = (0, 0, 0, 0)
        _alpha_threshold("alpha threshold", Range(0, 1)) = 0.2
    }

    SubShader {
        Tags {
            "Queue" = "Transparent"
        }
        Pass {
            Name "project"
            
CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct appdata members vertex)
#pragma exclude_renderers d3d11 xbox360
#pragma exclude_renderers gles
#pragma exclude_renderers xbox360
#pragma vertex vert
#pragma fragment frag
            uniform float4 _Color;
            uniform sampler2D _MainTex;
            uniform float _range;
            uniform float4 _center;
            // uniform float4x4 _Object2World;

            struct appdata {
                float4 vertex : POSITION ;
            };
            struct v2f {
                float4 pos: POSITION;
                float3 wp: TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.wp = (float3)mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            half4 frag(v2f i) : COLOR {
                float3 ua = float3(1, 0, 0) / (_range * 2);
                float3 va = float3(0, 0, 1) / (_range * 2);
                float2 uv;
                uv.x = dot(i.wp - (float3)_center, ua) + 0.5;
                uv.y = dot(i.wp - (float3)_center, va) + 0.5;
                if(uv.x > 0 && uv.x < 1 && uv.y > 0 && uv.y < 1)
                    return tex2D(_MainTex, uv) * _Color;
                else
                    return 0;
            }
ENDCG
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
        }
    }

Fallback "Diffuse"
}

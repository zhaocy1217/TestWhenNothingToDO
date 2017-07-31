// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "GOE/Role/TempRoleDiffRimCubeMap" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _EmissionIntensity ("EmissionIntensity", Range(0, 2)) = 1
        _FresnelPow ("FresnelPow", Range(1, 11)) = 3
        _BackColor ("BackColor", Color) = (0.1843137,0.4627451,0.9176471,1)
        _RimColor ("RimColor", Color) = (0.9333333,0.682353,0.3647059,1)
        _CubeMap ("CubeMap", Cube) = "_Skybox" {}
        _CubeMapIntensity ("CubeMapIntensity", Range(0, 4)) = 0
        _ReflectIntensity ("ReflectIntensity", Range(0, 1)) = 0
        _MaskTex ("MaskTex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            //#pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform float _EmissionIntensity;
            uniform float _FresnelPow;
            uniform float4 _BackColor;
            uniform float4 _RimColor;
            uniform samplerCUBE _CubeMap;
            uniform float _CubeMapIntensity;
            uniform float _ReflectIntensity;
            uniform sampler2D _MaskTex; uniform float4 _MaskTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_544 = (_MainTex_var.rgb*_Color.rgb); // Diffuse Color
                float3 node_2460 = (node_544*_EmissionIntensity); // Ambient Light
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(i.uv0, _MaskTex));
                float3 node_4452 = (texCUBE(_CubeMap,viewReflectDirection).rgb*_CubeMapIntensity*_MaskTex_var.r);
                float3 emissive = lerp(node_2460,node_4452,_ReflectIntensity);
                float node_7782 = max(0,dot(lightDirection,normalDirection)); // Lambert
                float3 finalColor = emissive + ((node_544*node_7782*_LightColor0.rgb)+(node_544*max(0,dot((-1*lightDirection),normalDirection))*_BackColor.rgb)+(pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPow)*1.4*_RimColor.rgb*node_7782));
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

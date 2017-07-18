// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

Shader "GOE/Role2/UIRole" {
    Properties {
		_MainTex("Diffuse", 2D) = "white" {}
        _ambientColor ("ambientColor", Color) = (0.5,0.5,0.5,1)
        _fresnelDir ("fresnelDir", Vector) = (0,0,0,0)
        _diffColor ("diffColor", Color) = (0.5,0.5,0.5,1)
        _fresnelColor ("fresnelColor", Color) = (0.5,0.5,0.5,1)
        _fresnelPower ("fresnelPower", Range(1, 11)) = 1
        _fresnelIntensity ("fresnelIntensity", Range(0, 4)) = 0
        _specColor ("specColor", Color) = (0.5,0.5,0.5,1)
        _specPower ("specPower", Range(1, 11)) = 1
        _specIntensity ("specIntensity", Range(0, 4)) = 1
        _diffDir ("diffDir", Vector) = (0,0,0,0)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
			cull off
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
            //#pragma multi_compile_fwdbase_fullshadows
            //#pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 

            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _fresnelColor;
            uniform float4 _fresnelDir;
            uniform float4 _ambientColor;
            uniform float _fresnelIntensity;
            uniform float _fresnelPower;
            uniform float _specPower;
            uniform float4 _specColor;
            uniform float4 _diffColor;
            uniform float _specIntensity;
            uniform float4 _diffDir;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _Diffuse_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (_Diffuse_var.rgb*_ambientColor.rgb);
                float node_6211 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float3 finalColor = emissive + (((_Diffuse_var.rgb*max(0,dot(normalize(_diffDir.rgb),normalDirection)))*_diffColor.rgb)+(pow(node_6211,_fresnelPower)*_fresnelIntensity*max(0,dot(normalize(_fresnelDir.rgb),normalDirection))*_Diffuse_var.rgb*_fresnelColor.rgb)+(pow((1.0 - node_6211),_specPower)*_specColor.rgb*_specIntensity*_Diffuse_var.rgb*_Diffuse_var.a));
                fixed4 finalRGBA = fixed4(finalColor,1);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

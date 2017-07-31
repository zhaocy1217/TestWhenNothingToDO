// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge


Shader "GOE/Role/CubeMapSpecCapAlphaTest" {
	Properties{
		_MainTex("MainTex", 2D) = "white" {}
	_Color("Color", Color) = (0.5,0.5,0.5,1)
		_Gloss("Gloss", Range(0, 1)) = 0.5
		_SpecColor("Spec Color", Color) = (1,1,1,1)
		_EmissionInstensity("EmissionInstensity", Range(0, 1)) = 0
		_MaskTex("MaskTex", 2D) = "black" {}
	_CubeMap("CubeMap", Cube) = "_Skybox" {}
	_CubeMapIntensity("CubeMapIntensity", Range(0, 6)) = 0
		_GlowColor("GlowColor", Color) = (0.5,0.5,0.5,1)
		_GlowInstensity("GlowInstensity", Range(0, 3)) = 0
		_CapTex("CapTex", 2D) = "white" {}
	_CapColor("CapColor", Color) = (0.5,0.5,0.5,1)
		_CapIntensity("CapIntensity", Range(0, 3)) = 0
		_Cutoff("Alpha cutoff", Range(0,1)) = 0
	}
		SubShader{
		Tags{
		"RenderType" = "Opaque"
	}
		Pass{
		Name "FORWARD"
		Tags{
		"LightMode" = "ForwardBase"
	}
		Cull Off
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#define UNITY_PASS_FORWARDBASE
#include "UnityCG.cginc"
#include "AutoLight.cginc"
//#pragma multi_compile_fwdbase_fullshadows
#pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 

		uniform float4 _LightColor0;
	uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
	uniform float4 _Color;
	uniform float _Gloss;
	uniform float4 _SpecColor;
	uniform samplerCUBE _CubeMap;
	uniform float _CubeMapIntensity;
	uniform sampler2D _MaskTex; uniform float4 _MaskTex_ST;
	uniform float _EmissionInstensity;
	uniform float4 _GlowColor;
	uniform float _GlowInstensity;
	uniform float4 _CapColor;
	uniform float _CapIntensity;
	uniform sampler2D _CapTex; uniform float4 _CapTex_ST;
	uniform float _Cutoff;
	struct VertexInput {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 texcoord0 : TEXCOORD0;
	};
	struct VertexOutput {
		float4 pos : SV_POSITION;
		float4 uv0 : TEXCOORD0;
		float4 posWorld : TEXCOORD1;
		float3 normalDir : TEXCOORD2;
	};
	VertexOutput vert(VertexInput v) {
		VertexOutput o = (VertexOutput)0;
		o.uv0.xy = v.texcoord0;
		half2 capCoord;
		capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz, v.normal);
		capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz, v.normal);
		o.uv0.zw = capCoord * 0.5f + 0.5f;
		o.normalDir = UnityObjectToWorldNormal(v.normal);
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		float3 lightColor = _LightColor0.rgb;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	float4 frag(VertexOutput i) : COLOR{
		i.normalDir = normalize(i.normalDir);
	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
	float3 normalDirection = i.normalDir;
	float3 viewReflectDirection = reflect(-viewDirection, normalDirection);
	float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
	float3 lightColor = _LightColor0.rgb;
	float3 halfDirection = normalize(viewDirection + lightDirection);
	////// Lighting:
	////// Emissive:
	float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
	clip(_MainTex_var.a -= _Cutoff);
	float3 node_544 = (_MainTex_var.rgb*_Color.rgb); // MainTex Color
	float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(i.uv0, _MaskTex));
	float4 _CapTex_var = tex2D(_CapTex,TRANSFORM_TEX(i.uv0.zw, _CapTex));
	float3 emissive = ((texCUBE(_CubeMap,viewReflectDirection).rgb*_CubeMapIntensity*node_544*_MaskTex_var.r) + (node_544*_EmissionInstensity) + (_GlowColor.rgb*_GlowInstensity*_MaskTex_var.g) + (_CapColor.rgb*_CapIntensity*_CapTex_var.rgb));
	float node_7782 = max(0,dot(lightDirection,normalDirection)); // Lambert
	float3 finalColor = emissive + (((node_544*node_7782) + (node_7782*pow(max(0,dot(normalDirection,halfDirection)),exp2(lerp(1,11,(_MainTex_var.a + (_Gloss*1.0 + -0.5)))))*_SpecColor.rgb))*_LightColor0.rgb);
	return fixed4(finalColor,1);
	}
		ENDCG
	}
	}
		FallBack "Diffuse"
}
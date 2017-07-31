// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "GOE/Depleted/Lightmap Reflect Receive Shadow Fuck"
{
	Properties
	{
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_RefTex("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_EnvironmentPower("Environment Factor", Range(0,1)) = 0
	_RefTex2("Diffuse2 (RGB) Transparent (A)", 2D) = "white" {}
	_EnvironmentPower2("Environment Factor2", Range(0,1)) = 0
		_IllumFactor("Illumin Factor", Range(1,2)) = 1

	}


		SubShader{

		Tags{ "Queue" = "Geometry+100" "IgnoreProjector" = "True" }
		Pass
	{

		Tags{ "LightMode" = "ForwardBase" }
		Fog{ Mode off }
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
//#pragma multi_compile_fwdbase
#pragma fragmentoption ARB_precision_hint_fastest
#pragma exclude_renderers d3d11 flash
#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "LightFace.cginc"

		sampler2D _MainTex;
	sampler2D _RefTex;
	sampler2D _RefTex2;
	half4 _MainTex_ST;

	fixed4 _Color;
	fixed _EnvironmentPower;
	fixed _EnvironmentPower2;
	// half4 unity_LightmapST;
	// sampler2D unity_Lightmap;

	half _IllumFactor;
	struct v2f
	{
		float4	pos : SV_POSITION;
		half4	uv : TEXCOORD0;
		half4  reflectUV :TEXCOORD1;
		half4   uv_Lightface : TEXCOORD2;
		float4  depth : TEXCOORD3;

	};


	struct appdata
	{
		float4 vertex : POSITION;
		float4 normal : NORMAL;
		float4 texcoord : TEXCOORD0;
		float4 texcoord1 : TEXCOORD1;
	};

	float3 reflect(float3 I,float3 N)
	{
		return I - 2.0*N *dot(N,I);
	}

	v2f vert(appdata v)
	{
		WORLD_POS
		float4 viewPos = mul(UNITY_MATRIX_MV, float4(v.vertex.xyz, 1));
		v2f o;
		o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

		o.uv.zw = worldPos.xz;// half2(0, 0);

#ifdef LIGHTMAP_ON
		o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#else
		o.uv.zw = half2(1,1);
#endif

		o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
		o.reflectUV = ComputeScreenPos(o.pos);
#ifdef LIGHT_FACE_ON	
		o.uv_Lightface.xy = LightFaceUV(worldPos);
		o.uv_Lightface.zw = LightFaceShadowUV(worldPos);
#else
		o.uv_Lightface = half4(0,0,0,0);
#endif
		o.depth = SimulateFogVS(o.pos.xyz, worldPos.xyz);
		return o;
	}


	float4 frag(v2f i) : COLOR
	{
		fixed4 tex = tex2D(_MainTex, i.uv.xy);
	half4 reflectiveColor = tex2D(_RefTex, (i.reflectUV.xy / i.reflectUV.w));
	half4 reflectiveColor2 = tex2D(_RefTex2, ((i.uv.zw*1.2)));
	tex.a = tex.a;
	tex.rgb *= _Color;
	tex.rgb *= _IllumFactor;

#ifdef LIGHTMAP_ON
	fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv.zw));
#else
	fixed3 lm = fixed3(1,1,1);
#endif
	_EnvironmentPower *= (tex.a);
	tex.rgb = tex.rgb + reflectiveColor2*_EnvironmentPower2 + reflectiveColor.rgb * _EnvironmentPower;

#ifdef LIGHT_FACE_ON	
	LightFaceColorReceiveShadow(tex.xyzw, i.uv_Lightface.xy, i.uv_Lightface.zw, lm, 1);
#else
	tex.rgb = tex.rgb * lm.rgb;// *UNITY_LIGHTMODEL_AMBIENT.rgb;
#endif
	tex.xyz = SimulateFog(i.depth, tex, 1);

	return tex;
	}

		ENDCG
	}
	}
}




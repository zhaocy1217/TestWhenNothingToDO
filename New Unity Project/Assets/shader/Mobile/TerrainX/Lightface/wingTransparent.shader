// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Depleted/WingTransparent" 
{

Properties {
	_MainTex ("Base", 2D) = "white" {}
//	_BumpMap("Normal", 2D) = "bump" {}
	_Noise ("Noise", 2D) = "bump" {}
	_RefColor ("Ref Color", Color) = (1,1,1,1)
	_ReflectionTex("_ReflectionTex", 2D) = "black" {}
	_ReflectionPower("_ReflectionTex Power", Range(0, 10)) = 1
	_DirectionUv("Wet scroll direction (2 samples)", Vector) = (1.0,1.0, -0.2,-0.2)
	_TexAtlasTiling("Tex atlas tiling", Vector) = (8.0,8.0, 4.0,4.0)
	
	_CullOff("Alpha Test Value", Range(0, 1)) = 0.1
	_Speed ("Speed", Range (0, 10)) = 1
}

CGINCLUDE		

struct v2f_full
{
	half4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
//	half2 uv_BumpMap : TEXCOORD1;
	half4 noiseScrollUv : TEXCOORD1;	
};
	
#include "UnityCG.cginc"		
#include "Lighting.cginc"
half4 _DirectionUv;
half4 _TexAtlasTiling;
fixed4 _RefColor;
fixed _CullOff;
half _ReflectionPower;
float _Speed;
sampler2D _MainTex;
sampler2D _Noise;	
//sampler2D _BumpMap;		
sampler2D _ReflectionTex;
			
ENDCG 

SubShader {
	Tags { "RenderType"="Geometry+200" }

	LOD 400 
	
	Pass 
	{
		//cull off
		//AlphaTest Greater [_CullOff]
	Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		half4 _MainTex_ST;
		half4 _BumpMap_ST;

//		float4 unity_LightmapST;	
//		sampler2D unity_Lightmap;
		
		v2f_full vert (appdata_full v) 
		{
			v2f_full o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv.xy = TRANSFORM_TEX(v.texcoord ,_MainTex);
		//	o.uv_BumpMap.xy = TRANSFORM_TEX(v.texcoord1, _BumpMap);
			o.noiseScrollUv.xyzw = v.texcoord.xyxy * _TexAtlasTiling + _Time.xxxx * _Speed * _DirectionUv;
										
			return o; 
		}
				
		fixed4 frag (v2f_full i) : COLOR0 
		{
			fixed4 tex = tex2D (_MainTex, i.uv.xy);
			//clip(tex.a - _CullOff);
			half3 nrml =   UnpackNormal(tex2D(_Noise, i.noiseScrollUv.xy));
			nrml +=  UnpackNormal(tex2D(_Noise, i.noiseScrollUv.zw));
			
			nrml.xy *= 0.25;
										
			//fixed4 rtRefl = tex2D (_ReflectionTex, (i.screen.xy / i.screen.w) + nrml.xy);
			fixed4 rtRefl = tex2D (_ReflectionTex,  nrml.xy);			
			rtRefl *= _RefColor; 
		
			
			tex.xyz  += (rtRefl.xyz * _ReflectionPower * tex.a);
			//tex.a += 0.15;
			return tex;	
		}	
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
	
		ENDCG
	}
} 

}

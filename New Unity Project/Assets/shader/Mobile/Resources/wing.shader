// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Transparent/IceFire/CullOut/Wing" 
{

Properties {
	_MainColor("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base", 2D) = "white" {}
//	_BumpMap("Normal", 2D) = "bump" {}
	_Noise ("Noise", 2D) = "bump" {}
	_RefColor ("Ref Color", Color) = (1,1,1,1)
	_ReflectionTex("_ReflectionTex", 2D) = "black" {}
	_ReflectionPower("_ReflectionTex Power", Range(0, 2)) = 1
	_DirectionUv("Wet scroll direction (2 samples)", Vector) = (1.0,1.0, -0.2,-0.2)
	_TexAtlasTiling("Tex atlas tiling", Vector) = (8.0,8.0, 4.0,4.0)
	
	_CullOff("Alpha Test Value", Range(0, 1)) = 0.1
	
	_Shininess ("Shininess", Range (0, 1)) = 0.078125
	_Speed ("Speed", Range (0, 10)) = 1

	_ReflectionTex2("_ReflectionTex2", 2D) = "black" {}
	_RefColor2("Ref Color", Color) = (0,0,0,1)
	_ReflectionTexTiling("_ReflectionTex2 atlas tiling", Vector) = (8.0,8.0, 8.0, 8.0)
	_AlphaSpeed("Alpha Speed", Range(0, 20)) = 3
}

CGINCLUDE		

struct v2f_full
{
	half4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
//	half2 uv_BumpMap : TEXCOORD1;
	half4 noiseScrollUv : TEXCOORD1;	
	// half4 uvgrab : TEXCOORD2;
};
	
#include "UnityCG.cginc"		
#include "Lighting.cginc"
half4 _DirectionUv;
half4 _TexAtlasTiling;
half4 _ReflectionTexTiling;
fixed4 _RefColor;
fixed _CullOff;
fixed4 _RefColor2;
half _ReflectionPower;
half _Shininess;
float _Speed;
half _AlphaSpeed;
sampler2D _MainTex;
half4 _MainColor;
sampler2D _Noise;	
sampler2D _ReflectionTex;
sampler2D _ReflectionTex2;
ENDCG 

SubShader {
	Tags { "Queue" = "Transparent+1000" "RenderType" = "Transparent" }
	//Blend SrcAlpha oneminusSrcAlpha
	Pass 
	{
		cull off
		
		CGPROGRAM
		half4 _MainTex_ST;
		half4 _BumpMap_ST;

//		float4 unity_LightmapST;	
//		sampler2D unity_Lightmap;
		float4 _ReflectionTex2_ST;
		v2f_full vert (appdata_full v) 
		{
			v2f_full o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv.xy = TRANSFORM_TEX(v.texcoord ,_MainTex);
		//	o.uv_BumpMap.xy = TRANSFORM_TEX(v.texcoord1, _BumpMap);
			o.noiseScrollUv.xyzw = v.texcoord.xyxy;// *_TexAtlasTiling + _Time.xxxx * _Speed * _DirectionUv;
										
			return o; 
		}
		
		fixed4 frag (v2f_full i) : COLOR0 
		{
			float2 uv = i.noiseScrollUv.xy;
			i.noiseScrollUv.xyzw = i.noiseScrollUv.xyzw *_TexAtlasTiling + _Time.xxxx * _Speed * _DirectionUv;
			fixed4 tex = tex2D (_MainTex, i.uv.xy);
			tex.rgb *= _MainColor.rgb;
		//	clip(tex.a - _CullOff);
//			half3 h = normalize (_WorldSpaceLightPos0 +  _WorldSpaceCameraPos);
//			half3 normal = UnpackNormal(tex2D(_BumpMap, i.uv_BumpMap));
//			
//			fixed diff = max (0, dot (normal, _WorldSpaceLightPos0));
//	
//			float nh = max (0, dot (normal, h));
//			float spec = pow (nh, _Shininess*128.0) * tex.a;
//	
//			fixed4 c;
//			c.rgb = (tex * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (1 * 2);			
			
			
			half3 nrml = tex2D(_Noise, i.noiseScrollUv.xy);// UnpackNormal(tex2D(_Noise, i.noiseScrollUv.xy));
			nrml += tex2D(_Noise, i.noiseScrollUv.zw);// UnpackNormal(tex2D(_Noise, i.noiseScrollUv.zw));
			
			nrml.xy *= 0.25;
										
			//fixed4 rtRefl = tex2D (_ReflectionTex, (i.screen.xy / i.screen.w) + nrml.xy);
			fixed4 rtRefl = tex2D (_ReflectionTex,  nrml.xy);			
			rtRefl *= _RefColor; 
		
			tex.xyz  += (rtRefl.xyz * _ReflectionPower);
			
			float2 origin = uv;// i.uvgrab.xy;
			//origin = TRANSFORM_TEX(origin, _ReflectionTex2);
			origin.xy += _Time.xx*_ReflectionTexTiling.zw;
			origin.xy *= _ReflectionTexTiling.xy;
			half4 reflectiveColor = tex2D(_ReflectionTex2, origin.xy);
			tex.xyz += _RefColor2.xyz*reflectiveColor.xyz;
			//tex.a = abs(sin(_Time.x*_AlphaSpeed));
			tex.a = 0.1;
			return tex;	
		}	
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
	
		ENDCG
	}
} 

//FallBack "Transparent/IceFire/CullOut/Wing"

}

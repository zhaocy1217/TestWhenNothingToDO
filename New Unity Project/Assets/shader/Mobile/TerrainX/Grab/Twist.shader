Shader "GOE/Grab/Twist" {
Properties {
	_MainColor("Main", Color) = (1,1,1,1)
	_DispMap ("Displacement Map (RG)", 2D) = "white" {}
	_MaskTex ("Mask (R)", 2D) = "white" {}
	_MainTex ("Main Tex", 2D) = "white" {}
	_Rate("Rate", Range(0,1)) = 0.5
	_Height("Height", Range(-1.5,1.5)) = 0.8
	_DispScrollSpeedX  ("Map Scroll Speed X", Float) = 0
	_DispScrollSpeedY  ("Map Scroll Speed Y", Float) = 0
	_StrengthX  ("Displacement Strength X", Float) = 1
	_StrengthY  ("Displacement Strength Y", Float) = -1
}

Category {
	Tags { "Queue"="Transparent+99" "RenderType"="Transparent" }
	//Blend SrcAlpha OneMinusSrcAlpha
	//AlphaTest Greater .01
 Lighting Off 
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}

	SubShader {
		Pass {
			Name "BASE"
			
			Tags { "LightMode" = "Always" }
			
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float2 texcoord: TEXCOORD0;
	float4 param : TEXCOORD1;
};

struct v2f {
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float2 uvmain : TEXCOORD0;
	float4 param : TEXCOORD1;
	float4 uvgrab : TEXCOORD2;
};

uniform half _StrengthX;
uniform half _StrengthY;

uniform float4 _DispMap_ST;
uniform sampler2D _DispMap;
uniform sampler2D _MaskTex;
uniform half _DispScrollSpeedY;
uniform half _DispScrollSpeedX;

float4 _MainColor;
sampler2D _MainTex;
float _Rate;
float _Height;

v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.zw;
	o.uvmain = TRANSFORM_TEX( v.texcoord, _DispMap );
	o.color = v.color;
	o.param.xy = v.param.xy;
	o.param.z = v.vertex.x + _Height;// + 0.5;// * 1;
	o.param.w = 0;
	return o;
}

sampler2D _grabLastTexture;

half4 frag( v2f i ) : COLOR
{
	i.param.z = saturate(i.param.z);
	//return float4(i.param.z,i.param.z,i.param.z,0);
	//_DispScrollSpeedX *= i.param.z;
	//_DispScrollSpeedY *= i.param.z;
	////_StrengthX *= (1-i.param.z/2);
	//_StrengthY *= (1-i.param.z/2);
	//scroll displacement map.
	half2 mapoft = half2(_Time.y*_DispScrollSpeedX, _Time.y*_DispScrollSpeedY);

	//get displacement color
	half4 offsetColor = tex2D(_DispMap, i.uvmain + mapoft);

	//get offset
	half oftX =  offsetColor.r * _StrengthX;// * i.param.x;
	half oftY =  offsetColor.g * _StrengthY;// * i.param.x;

	i.uvgrab.x += oftX;// * i.param.z;// * 0.5;
	i.uvgrab.y += oftY;// * i.param.z;// * 0.2;

	half4 col = tex2Dproj( _grabLastTexture, UNITY_PROJ_COORD(i.uvgrab));

	//intensity is controlled by particle color.
	col.a = i.color.a;

	//use mask's red channel to determine visibility.
	fixed4 tint = tex2D( _MaskTex, i.uvmain );

	col.a *= tint.r;
	
	
	//col.rgb *= _MainColor.rgb;
	float3 texcolor = tex2D(_MainTex,i.uvmain);
	col.rgb = col.rgb * i.param.z + (1-i.param.z) * texcolor.rgb;
	col.rgb = col.rgb * _Rate  + (1-_Rate) * texcolor.rgb;

	return col;
}
ENDCG
		}
}

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro
	
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture * primary double, texture * primary }
		}
	}
}
}

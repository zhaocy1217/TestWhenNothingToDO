// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "GOE/Grab/Pubu" 
{
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_DistortAmt  ("Distortion", range (0,128)) = 10
	_MainTex ("Tint Color (RGB)", 2D) = "white" {}
	_DistortMap ("DistortMap", 2D) = "white" {}
	_Speed ("speed", Range(0,10)) = 2
}

CGINCLUDE
#pragma fragmentoption ARB_precision_hint_fastest
#pragma fragmentoption ARB_fog_exp2
#include "UnityCG.cginc"

sampler2D _grabLastTexture;
float4 _grabLastTexture_TexelSize;
sampler2D _DistortMap;
sampler2D _MainTex;
fixed4 _Color; 
half _Speed;
struct v2f {
	float4 vertex : POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvbump : TEXCOORD1;
	float2 uvmain : TEXCOORD2;
	float4 vertexcolor : COLOR;
};

uniform float _DistortAmt;


half4 frag( v2f i ) : COLOR
{
	float2 uv_main =  i.uvmain;
	uv_main.y += _Time.x * _Speed;
	fixed4 tint = tex2D( _MainTex, uv_main );

	float2 uv_bump = i.uvbump;
	uv_bump.y += _Time.x * _Speed;
	half2 bump =(tex2D( _DistortMap, uv_bump )).rr; // we could optimize this by just reading the x & y without reconstructing the Z
	float2 offset = bump * _DistortAmt * i.vertexcolor.a * _grabLastTexture_TexelSize.xy;
	i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy ;

	half4 col = tex2D( _grabLastTexture, i.uvgrab.xy );
	fixed4 c =  col;
	//c.rgb = tint.rgb * tint.a + c.rgb * (1 - tint.a) ;
	//c.a = tint.a-0.9;
	return c * (1-bump.r)+  c*_Color * bump.r;
}
ENDCG

	Category {

		// We must be transparent, so other objects are drawn before this one.
		Tags { "Queue"="Transparent-100" "RenderType"="Opaque" }


		SubShader {
		//Blend SrcAlpha OneMinusSrcAlpha
		//Blend one one
			Pass {
				Name "BASE"
				Tags { "LightMode" = "Always" }
				
				ZWrite Off
				//ColorMask RGB
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					fixed4 vertexcolor : COLOR;
				};

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.vertex.xyzw /= o.vertex.w;
					o.uvgrab.xyzw = ComputeScreenPos(o.vertex);
					o.uvgrab.xyzw /= o.uvgrab.w ;
					o.uvbump = MultiplyUV( UNITY_MATRIX_TEXTURE1, v.texcoord );
					o.uvmain = MultiplyUV( UNITY_MATRIX_TEXTURE2, v.texcoord );
					o.vertexcolor = v.vertexcolor;
					return o;
				}
				ENDCG
			}
		}

	}

}
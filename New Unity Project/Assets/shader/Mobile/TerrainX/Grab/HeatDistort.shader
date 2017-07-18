
Shader "GOE/Grab/HeatDistort" {
Properties {
	_DistortAmt  ("Distortion", range (0,128)) = 10
	_MainTex ("Tint Color (RGB)", 2D) = "white" {}
	_DistortMap ("DistortMap", 2D) = "white" {}
}

CGINCLUDE
#pragma fragmentoption ARB_precision_hint_fastest
#pragma fragmentoption ARB_fog_exp2
#include "UnityCG.cginc"

sampler2D _grabLastTexture;
float4 _grabLastTexture_TexelSize;
sampler2D _DistortMap;
sampler2D _MainTex;

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
	fixed4 bumpColor = tex2D( _DistortMap, i.uvbump );
	half2 bump =bumpColor.rg; // we could optimize this by just reading the x & y without reconstructing the Z
	float2 offset = bump * _DistortAmt * i.vertexcolor.a * _grabLastTexture_TexelSize.xy;
	i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
	
	half4 col = tex2D( _grabLastTexture, i.uvgrab.xy );
	half4 tint = tex2D( _MainTex, i.uvmain );
	col *= tint;
	col.a = bumpColor.b;
	return col;
}
ENDCG

	Category {

		// We must be transparent, so other objects are drawn before this one.
		Tags { "Queue"="Transparent-100" "RenderType"="Opaque" }


		SubShader {
		Blend SrcAlpha OneMinussrcalpha
		//Blend One One
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
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
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
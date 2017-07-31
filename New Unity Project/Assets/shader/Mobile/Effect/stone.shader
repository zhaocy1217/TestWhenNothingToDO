// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Stone"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	Subshader
	{
		Tags { "Queue"="Geometry-100" "IgnoreProjector"="True" }
		
		Pass
		{
			cull off
			CGPROGRAM
			
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"

				
				struct v2f
				{
					float4 pos	: SV_POSITION;
					float2 uv 	: TEXCOORD0;
				};
				
				uniform float4 _MainTex_ST;
	
				v2f vert (appdata_base v)
				{
					v2f o;
					
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
	
					return o;
				}
				
				fixed4 _Color;

				uniform sampler2D _MainTex;
	
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 tex = tex2D(_MainTex, i.uv);
					fixed temp = (tex.x + tex.y +tex.z)*0.33;
					tex.xyz = fixed3(temp, temp, temp);
					return tex;//black * _Color *_test;
				}
			ENDCG
		}
	}
	
	Fallback "VertexLit"
}
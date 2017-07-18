Shader "GOE/LinkEffect"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlackTex ("Base (RGB)", 2D) = "white" {}
	}
	
	Subshader
	{
		Tags{ "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
		
		Pass
		{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha One
			Cull Off Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }


		CGPROGRAM
				#pragma multi_compile LIGHTEFFECT_ON LIGHTEFFECT_OFF
				#pragma multi_compile LIGHTFACE_ON LIGHTFACE_OFF
				#pragma multi_compile FOG_ON FOG_OFF
			
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"

				
				struct v2f
				{
					float4 pos	: SV_POSITION;
					float2 uv 	: TEXCOORD0;
					float2 uv2 	: TEXCOORD1;
				};
				
				uniform float4 _MainTex_ST;
				uniform float4 _BlackTex_ST;
				v2f vert (appdata_base v)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.uv2 = TRANSFORM_TEX(v.texcoord, _BlackTex);
					return o;
				}
				
				fixed4 _Color;
				
				uniform sampler2D _MainTex;
				uniform sampler2D _BlackTex;

				fixed4 frag (v2f i) : COLOR
				{
					fixed4 tex = tex2D(_MainTex, i.uv);
					fixed4 black = tex2D(_BlackTex, i.uv2);		

					black.rgb += tex.rgb;

					black.a += max(max(tex.r,tex.g),tex.b);
					return black*_Color;
				}
			ENDCG
		}
	}
	
	Fallback "VertexLit"
}
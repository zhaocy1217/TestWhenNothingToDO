// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "GOE/Scene/GrassStatic" {
	Properties {
	_Color("Diffuse Color", Color) = (1,1,1,1)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_MinSwing("MinSwing",range(0,1)) = 0.3
	_MinSpeed("MinSpeed",range(0,2)) = 0.6
	_RockFactor("RockFactor",range(0,2)) = 2
	_WaveDistance("Wave Distance", Range(0,10)) = 1
	_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
}

Category 
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Cull Off
		Lighting Off

		SubShader {
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON
				#pragma target 3.0
				#include "UnityCG.cginc"
				#include "LightFace.cginc"

				sampler2D _MainTex;
				float _RockFactor;
				fixed _MinSwing;
				fixed _MinSpeed;
				float4 _MainTex_ST;
				fixed4 _Color;
				fixed _Cutoff;

				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
					LIGHTFACE_COORDS(1)
					FOG_COORDS(2)
				};

				v2f vert(appdata_t v)
				{
					v2f o;

					WORLD_POS

					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

					LIGHTFACE_VS
					LIGHTFACE_VS_SHADOW
					FOG_VS

					o.color = v.color;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					half4 tex = tex2D(_MainTex, i.uv);
					tex.rgb *= _Color.rgb;
					clip(tex.a - _Cutoff);

					LIGHTMAP_PS
					LIGHTFACE_PS_SHADOW
					FOG_PS

					return tex;
				}
				ENDCG
			}
		}
	}
}

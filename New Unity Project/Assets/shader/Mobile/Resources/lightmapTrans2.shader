// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Lightmap Transparent2"
{
	Properties
	{
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
		_IllumFactor("Illumin Factor", Range(1,2)) = 1
		_transFactor("Trans Factor", Range(0,1)) = 1
	}


		SubShader{

			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" }

			Pass
			{

				Tags {  "LightMode" = "ForwardBase" "Queue" = "Transparent" }
				ZTEST LEqual
				ZWrite  Off
				Blend srcalpha oneminussrcalpha

				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					//#pragma multi_compile_fwdbase
					#pragma fragmentoption ARB_precision_hint_fastest
					#pragma exclude_renderers d3d11 flash

					#include "UnityCG.cginc"
					#include "Lighting.cginc"
					#include "LightFace.cginc"

					sampler2D _MainTex;
					fixed4 _Color;
					half _IllumFactor;

					fixed _transFactor;

					struct v2f
					{
						float4	pos : SV_POSITION;
						half4	uv : TEXCOORD0;
						float4  depth : TEXCOORD1;
		#ifdef LIGHTMAP_ON
						half2   uv2 : TEXCOORD4;
		#endif
					};

					struct appdata
					{
						float4 vertex : POSITION;
						float4 texcoord : TEXCOORD0;
						float4 texcoord1 : TEXCOORD1;
					};


					v2f vert(appdata v)
					{
						WORLD_POS
						v2f o;
						o.uv = v.texcoord;
						#ifdef LIGHTMAP_ON
						o.uv2 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
						#endif

						o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
		#ifdef LIGHT_FACE_ON
						o.uv.zw = LightFaceUV(worldPos);
		#else
						o.uv.zw = half2(0, 0);
		#endif

						o.depth = SimulateFogVS(o.pos.xyz, worldPos.xyz);

						return o;
					}


					float4 frag(v2f i) : COLOR
					{
						fixed4 tex = tex2D(_MainTex, i.uv.xy);
						tex.rgb *= _Color;
						tex.rgb *= _IllumFactor;

						#ifdef LIGHTMAP_ON
						fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2));
						#else
						fixed3 lm = fixed3(1,1,1);
						#endif

						LightFaceColor(tex.xyzw, i.uv.zw, lm, 1); 

						tex.xyz = SimulateFog(i.depth, tex, 1);

						tex.a *= (0.2 + 0.8 * _transFactor);
						return tex;
					}

				ENDCG
			}
		}
}





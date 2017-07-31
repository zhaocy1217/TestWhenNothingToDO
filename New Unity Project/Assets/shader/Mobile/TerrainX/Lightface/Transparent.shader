// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Depleted/Transparent"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//_MatCap ("MatCap (RGB)", 2D) = "white" {}
		_RefTex ("Ref Tex (RGB)", 2D) = "white" {}
		
		lightTest("不要设置这个东西",Range(1,1)) = 1  
		_LColor("不要设置这个东西", Color) = (1,1,1,1)
	}
	
	Subshader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector"="True" }
		
		Pass
		{
			cull off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
				#pragma multi_compile LIGHTEFFECT_ON LIGHTEFFECT_OFF
				#pragma multi_compile LIGHTFACE_ON LIGHTFACE_OFF
				#pragma multi_compile FOG_ON FOG_OFF
			
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				#include "LightFace.cginc"
				
				struct v2f
				{
					float4 pos	: SV_POSITION;
					float2 uv 	: TEXCOORD0;
					float2 cap	: TEXCOORD1;
					float2 ref	: TEXCOORD2;
					#ifdef LIGHTEFFECT_ON
						half4  uv_Lightface : TEXCOORD3;
						half4  uv_LightfaceShadow : TEXCOORD4;
					#endif
				};
				
				uniform float4 _MainTex_ST;
				float4x4 _texView;
				//fixed4 _LightColor0;
				v2f vert (appdata_base v)
				{
					v2f o;
					WORLD_POS
					float3 viewWorld = -1*normalize(worldPos.xyz - _WorldSpaceCameraPos.xyz);
					
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					
					half2 capCoord;
					capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
					capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
					o.cap = capCoord * 0.5 + 0.5;
					
					float3 worldNormal = UnityObjectToWorldNormal(v.normal.xyz);
					float test = max(0.2,dot(worldNormal, normalize(_WorldSpaceLightPos0.xyz)));
					o.ref.x =test;
					o.ref.y =test;
					
					
					#ifdef LIGHTEFFECT_ON
						o.uv_Lightface = LightFaceUV(worldPos);
						o.uv_LightfaceShadow = LightFaceUV(worldPos);		
					#endif
					return o;
				}
				
				uniform sampler2D _MainTex;
				//uniform sampler2D _MatCap;
				uniform sampler2D _RefTex;
				//fixed light;
				fixed lightTest;
				fixed4 _LColor;

				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 tex = tex2D(_MainTex, i.uv);
					fixed4 ref = tex2D(_RefTex, i.cap);
					
					tex.rgb = tex.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb + tex.rgb * _LightColor0 * i.ref.x  + (ref.rgb)*floor(tex.a*2);//(max(0, tex.a-0.78f)/0.22f);
					
					//LightFaceColor(tex.rgba, i.uv_Lightface.xy, 1, 0);
					

					tex.a = 0.2;
			
					return tex;
				}
			ENDCG
		}
	}
	
	Fallback "VertexLit"
}
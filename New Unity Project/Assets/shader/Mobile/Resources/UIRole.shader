// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/UI Role"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_BackColor ("Back Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RefTex ("Ref Tex (RGB)", 2D) = "black" {}
		_LightDir ("Light Dir", Vector) = (0.6,-1,-0.21,1)
		lightTest("不要设置这个东西",Range(1,1)) = 1  
		_LColor("不要设置这个东西", Color) = (1,1,1,1)
		
		_Stencil ("Stencil Value", Float) = 0
	}
	
	Subshader
	{
		Tags { "Queue"="Geometry-100" "IgnoreProjector"="True" }
		LOD 200
		
		Stencil 
		{
			Ref [_Stencil]
			Comp Equal
		}
		
		Pass
		{
			//Tags { "LightMode" = "Always" }
			cull off
			CGPROGRAM
//				#pragma multi_compile LIGHTEFFECT_ON LIGHTEFFECT_OFF
//				#pragma multi_compile LIGHTFACE_ON LIGHTFACE_OFF
//				#pragma multi_compile FOG_ON FOG_OFF
			
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				//#include "LightFace.cginc"
				
				struct v2f
				{
					float4 pos	: SV_POSITION;
					float2 uv 	: TEXCOORD0;
					float2 cap	: TEXCOORD1;
					float2 ref	: TEXCOORD2;
				};
				
				uniform float4 _MainTex_ST;
				float4x4 _texView;
				fixed4 _LightColor0;
				fixed4 _BackColor;
				float4 _LightDir;
				v2f vert (appdata_base v)
				{
					v2f o;
					
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					
					half2 capCoord;
					capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
					capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
					o.cap = capCoord * 0.5 + 0.5;
					
					float3 worldNormal = UnityObjectToWorldNormal(v.normal.xyz);
					float test = max(0.1,dot(worldNormal, normalize(_LightDir.xyz)));
					o.ref.x =test *test;
					o.ref.y =test * test;
					return o;
				}
				
				uniform sampler2D _MainTex;
				uniform sampler2D _RefTex;
				fixed light;
				fixed lightTest;
				fixed4 _LColor;

				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 tex = tex2D(_MainTex, i.uv);
					fixed4 ref = tex2D(_RefTex, i.cap);
					
					fixed3 c = fixed3(0.8,0.8,0.8) * (1-i.ref.x) + _BackColor.rgb *(i.ref.x)*(_BackColor.a + 1);
					//tex.rgb = tex.rgb * fixed3(0.4,0.4,0.6) * 0.8 + tex.rgb * fixed3(0.8,0.8,0.8) * i.ref.x * 1.75 + (ref.rgb)*floor(tex.a*2);//(max(0, tex.a-0.78f)/0.22f);
					tex.rgb = tex.rgb * fixed3(0.4,0.4,0.6) * 0.8 + c*tex.rgb + (ref.rgb)*floor(tex.a*2);
					
					tex.rgb =  tex.rgb * 0.8f * lightTest * _LColor.rgb;
					tex.a = 1;
			
					return tex;
				}
			ENDCG
		}
	}
	
	Fallback "VertexLit"
}
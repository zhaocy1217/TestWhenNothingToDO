// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "GOE/Lightmap Transparent"
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1
	_bloom ("Bloom Factor", Range(0,1)) = 1
	_transFactor("Trans Factor", Range(0,1)) = 1
}


SubShader {

	Tags {"Queue"="Transparent" "IgnoreProjector"="True" }
	LOD 400
		
		
	Pass 
	{
		ColorMask 0
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers d3d11
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			sampler2D _MainTex;
			struct v2f
			{
				float4	pos : SV_POSITION;
				fixed2 uv : TEXCOORD;
			}; 
			
			struct appdata
			{
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			    float4 texcoord1 : TEXCOORD1;
			};
		

			v2f vert (appdata v)
			{
				v2f o;
				o.pos =  mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
				o.uv.xy = v.texcoord.xy;
				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
			
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				clip(tex.a -= 0.2);
				return fixed4(1,1,1,1);
			}

		ENDCG
	}		
		
		
	Pass 
	{
			
		Tags {  "LightMode" = "ForwardBase" "Queue" = "Transparent" }
		ZTEST LEqual
		ZWrite  Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
			#pragma multi_compile LIGHTEFFECT_ON LIGHTEFFECT_OFF
			#pragma multi_compile LIGHTFACE_ON LIGHTFACE_OFF
			#pragma multi_compile FOG_ON FOG_OFF
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
				
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
			sampler2D _MainTex;
			fixed4 _Color;
			half _IllumFactor;
			
			fixed _bloom;
			fixed _transFactor;
			
			struct v2f
			{
				float4	pos : SV_POSITION;
				half4	uv : TEXCOORD0;
			#ifdef LIGHTMAP_ON
				half2   uv2 : TEXCOORD1;
			#endif
			}; 
			
			struct appdata
			{
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			    float4 texcoord1 : TEXCOORD1;
			};
		

			v2f vert (appdata v)
			{
				v2f o;
				o.uv = v.texcoord;
				#ifdef LIGHTMAP_ON
				o.uv2 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				o.pos =  mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));

				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				#ifdef LIGHTMAP_ON
				fixed3 lm = DecodeLightmap( UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2));
				tex.rgb = tex.rgb * lm;
				#endif

				tex.a = 0.7;
				return tex;
			}

		ENDCG
	}	
}

//FallBack "Diffuse"
}




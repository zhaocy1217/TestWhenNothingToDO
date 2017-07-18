// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable

Shader "GOE/Transparent Object"
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1
	_bloom ("Bloom Factor", Range(0,1)) = 1
}


SubShader {

	Tags {"Queue"="Transparent" "IgnoreProjector"="True" }
	LOD 200
		
		
	Pass 
	{
		ColorMask 0
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
				
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			
			struct v2f
			{
				float4	pos : SV_POSITION;
				half4	uv : TEXCOORD0;
			}; 
			
			struct appdata
			{
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			};
		

			v2f vert (appdata v)
			{
				v2f o;
				o.uv = v.texcoord;
				o.pos =  mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
				return o;
			}

			sampler2D _MainTex;
			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				clip(tex.a - 0.01);
				return fixed4(0,1,1,1);
			}

		ENDCG
	}		
		
		
	Pass 
	{
			
		Tags {  "LightMode" = "ForwardBase" }
		ZTEST LEqual
		ZWrite  off
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
			
			// half4 unity_LightmapST;
			// sampler2D unity_Lightmap;

			fixed light; 
			fixed _bloom;
			
			struct v2f
			{
				float4	pos : SV_POSITION;
				half4	uv : TEXCOORD0;
				float4	ObjectPos : TEXCOORD1;
//			#ifdef LIGHTMAP_ON
//				half2   uv2 : TEXCOORD2;
//			#endif
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
				o.pos =  mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
				o.ObjectPos = o.pos;//v.vertex.xyzw;
				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				tex.a *= (1-_Time.x);
				
				return tex;
			}

		ENDCG
	}	
}
//FallBack "Diffuse"
//FallBack "Diffuse"
}




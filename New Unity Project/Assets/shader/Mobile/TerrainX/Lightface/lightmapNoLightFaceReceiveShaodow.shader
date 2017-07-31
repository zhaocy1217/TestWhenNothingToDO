// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Depleted/Lightmap No Lightmap Receive Shaodow" 
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1
	//_ShadowFactor("Shadow Factor", Range(0,1)) = 0.5
}


SubShader {

	Tags {"Queue"="Geometry+100" "IgnoreProjector"="True"}
	Pass 
	{
		Tags {  "LightMode" = "ForwardBase" }
		Fog { Mode off } 
		cull off
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers d3d11 flash
			#pragma multi_compile LIGHT_FACE_OFF  LIGHT_FACE_ON 
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "LightFace.cginc"
			
			sampler2D _MainTex;
			fixed4 _Color;
			fixed _IllumFactor;
			//fixed _ShadowFactor;
			// half4 unity_LightmapST;
			// sampler2D unity_Lightmap;
			
			float _Cutoff;
			fixed _bloom;
			struct v2f
			{
				float4	pos : SV_POSITION;
				half4	uv : TEXCOORD0;
				half2   uv2 : TEXCOORD1;
				float4  depth : TEXCOORD2;//depth:x  nor.y:y(用于光照计算)
	
			}; 
			
			struct appdata
			{
			    float4 vertex : POSITION;
				float4 nor : NORMAL;
			    float4 texcoord : TEXCOORD0;
			    float4 texcoord1 : TEXCOORD1;
			};
		

			v2f vert (appdata v)
			{
				WORLD_POS
				v2f o;
				o.uv = v.texcoord;
				
				o.pos =  UnityObjectToClipPos(float4(v.vertex.xyz, 1));
				o.uv.zw = half2(0,0);

				o.depth = SimulateFogVS(o.pos.xyz, worldPos.xyz);

#ifdef LIGHTMAP_ON
				o.uv2 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#else
				o.uv2 = half2(0,0);
//			// 计算世界坐标系下的法线,用于光照细节的计算,存于depth的y通道中,充分利用现有通道 //
//			//------------------------------------------------------------------------------------//
//				float3 worldNormal = UnityObjectToWorldNormal(v.nor).xyz;
//				o.depth.y = max(_ShadowFactor, dot(worldNormal, normalize(_WorldSpaceLightPos0.xyz)));
//			//------------------------------------------------------------------------------------//
//
#endif

				return o;
			}
				

			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				clip(tex.a -= _Cutoff);
				tex.rgb *= _Color;	
				tex.rgb *= _IllumFactor;
				
#ifdef LIGHTMAP_ON
					fixed3 lm = DecodeLightmap( UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2) );	
#else
				fixed3 lm = fixed3(1,1,1);
//				tex.xyz *= i.depth.y;
#endif

				tex.rgb = tex.rgb * lm;// *UNITY_LIGHTMODEL_AMBIENT.rgb;
				tex.xyz = SimulateFog(i.depth, tex, 1);
				

				return tex;
			}

		ENDCG
	}	
}
}




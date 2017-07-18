// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "GOE/Other/Grass RunTime" 
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_IllumFactor ("Illumin Factor", Range(0,2)) = 1
	_WindSpeed ("Wind Speed", Range(0,1)) = 0.5
	_RockPower ("Rock Power", Range(0,1)) = 0.3
	_RockSpeed ("Rock Speed", Range(1,10)) = 3
}


SubShader {

	Tags {"Queue"="Geometry" "IgnoreProjector"="True"}
	
		
	Pass 
	{
			
		Tags {  "LightMode" = "ForwardBase" }
		Fog { Mode Off }  
		//Blend SrcAlpha OneMinusSrcAlpha 
		//ZWrite off
		//ZTEST Always
		//alphatest Greater [_Cutoff]
		Cull off
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
				
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			#include "Lighting.cginc"
			
			float _RockPower;
			fixed _RockSpeed;
			float _WindSpeed;
			
			sampler2D _MainTex;
			fixed4 _Color;
			fixed _IllumFactor;
			float _Cutoff;
			 // half4 unity_LightmapST;
			// sampler2D unity_Lightmap;

			struct v2f
			{
				fixed4  col : COLOR;
				float4	pos : SV_POSITION;
				half2	uv : TEXCOORD0;
				half2   uv2 : TEXCOORD1;
				//float2	brightness : TEXCOORD1;
			}; 
		
			float4 GrassRock(float4 pos, float3 vectorPos,  float2 uv1, float dir)
			{
				
//				if (uv.y > 0.99)
//				{					
//					
//					float windx = 0;
//					float windz = 0;
//
//					if (uv1.y > 0.5f)
//					{
//						windx = sin( _Time.y * _WindSpeed)* 0.3;
//					}
//					else
//					{
//						windz = cos( _Time.y * _WindSpeed)* 0.3;
//					}
//					if (uv1.x > 0)
//					{
//						float percentage = 1f - uv1.x;
//						if (uv1.y < 0.5f)
//						{
//							windx += sin( uv1.x * 5 * _RockSpeed)* percentage;
//							windz += cos( uv1.x * 5 * _RockSpeed)* percentage;
//						}
//						else
//						{
//							windx += -sin( uv1.x * 5 * _RockSpeed)* percentage;
//							windz += -cos( uv1.x * 5 *_RockSpeed)* percentage;
//						}
//					}
//
//					pos.x += windx;
//					pos.z += windz;
//					//pos.y += windz;
//				}


				float windx = 0;
				float windz = 0;
				float windxOffst = 0;
				float windzOffst = 0;
//				if (uv1.y > 0.5f)
//				{
					windx = sin( _Time.y * _WindSpeed)* 0.08;
//				}
//				else
//				{
//					windz = cos( _Time.y * _WindSpeed)* 0.3;
//				}

				//uv1.x = clamp(uv1.x, 0.001, 1);
				
				//if (uv1.x > 0)
				{
					
					float percentage = 1 - uv1.x;
					//if (uv1.y < 0.5f)
//					{
						windxOffst = sin( uv1.x * 5 * _RockSpeed)* percentage;
						//windzOffst = cos( uv1.x * 5 * _RockSpeed)* percentage;
//					}
//					else
//					{
//						windx += -sin( uv1.x * 5 * _RockSpeed)* percentage;
//						windz += -cos( uv1.x * 5 *_RockSpeed)* percentage;
//					}
				}

				pos.x += (windx+windxOffst ) * vectorPos.y;
				//pos.z += (windz+windzOffst*0.5 ) * uv.y ;

				return pos;
			}

			v2f vert (appdata_full v)
			{
				v2f o;
				o.uv.xy = v.texcoord.xy;
				o.uv2 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.col = v.color;
				float4 newPos = mul(unity_ObjectToWorld, v.vertex);
			    newPos = GrassRock(newPos,  v.vertex.xyz, v.color.xy, v.tangent.x);
			    
				o.pos =  mul(UNITY_MATRIX_VP, newPos);
				return o;
			}
				

			float4 frag(v2f i) : COLOR
			{

				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				clip(tex.a - _Cutoff);
				tex = _Color * tex;
				tex.xyz = tex.xyz;// * _IllumFactor;// * i.col.x;
//				#ifdef LIGHTMAP_ON
//				fixed3 lm = DecodeLightmap( tex2D(unity_Lightmap, i.uv2) );	
//			
//		#else
//				fixed3 lm = fixed3(1,1,1);
//#endif		
				
//				tex.xyz;
			
				return tex;
			}

		ENDCG
	}	
}
//FallBack "Diffuse"
//FallBack "Diffuse"
}




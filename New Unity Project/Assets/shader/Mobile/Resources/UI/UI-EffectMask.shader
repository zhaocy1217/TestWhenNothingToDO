// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Particles/Additive Mask" {
	Properties {
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		 
		//-------------------add----------------------
		_MinX ("Min X", Float) = -10
		_MaxX ("Max X", Float) = 10
		_MinY ("Min Y", Float) = -10
		_MaxY ("Max Y", Float) = 10
		//-------------------add----------------------
		 
	}
	 
	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
		Blend SrcAlpha One
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off
		 
		SubShader {
			Pass {
			 
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				 
				#include "UnityCG.cginc"
				 
				sampler2D _MainTex;
				fixed4 _TintColor;
				//-------------------add----------------------
				float _MinX;
				float _MaxX;
				float _MinY;
				float _MaxY;
				//-------------------add----------------------


				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					//-------------------add----------------------
						float4 vpos : TEXCOORD1;
						//-------------------add----------------------
				};


				
				float4 _MainTex_ST;
				 
				v2f vert (appdata_t v)
				{
					v2f o;
					//-------------------add----------------------
					o.vpos = mul(unity_ObjectToWorld,v.vertex);
					//-------------------add----------------------
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
					o.projPos = ComputeScreenPos (o.vertex);
					COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}
				 
				sampler2D_float _CameraDepthTexture;
				float _InvFade;
				 
				fixed4 frag (v2f i) : SV_Target
				{
					#ifdef SOFTPARTICLES_ON
					float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
					float partZ = i.projPos.z;
					float fade = saturate (_InvFade * (sceneZ-partZ));
					i.color.a *= fade;
					#endif
					
					fixed4 c = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
					UNITY_APPLY_FOG_COLOR(i.fogCoord, c, fixed4(0,0,0,0));
					if (i.vpos.x >= _MinX&&i.vpos.x <= _MaxX&&i.vpos.y >= _MinY&&i.vpos.y <= _MaxY){
						
					}else{
						c.a=0;
					}
					//c.rgb *= c.a;
					return c;
					//-------------------add----------------------
				}
				ENDCG
			}
		}
	}
}
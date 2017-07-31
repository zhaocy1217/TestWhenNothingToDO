// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "GOE/Scene/Soul Tree" {
	Properties {
	_Color("Diffuse Color", Color) = (1,1,1,1)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_MinSwing("MinSwing",range(0,1)) = 0.2
	_MinSpeed("MinSpeed",range(0,2)) = 1
	_RockFactor("RockFactor",range(0,2)) = 0.5	
	_FogFactor("Fog Factor", Range(0,1)) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	Cull Off Lighting Off 
	Blend One One
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			#pragma multi_compile_fog
#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
			#include "UnityCG.cginc"
			#include "LightFace.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			float _RockFactor;
			fixed _MinSwing;
			fixed _MinSpeed;
			fixed _FogFactor;
			float4 _MainTex_ST;
			fixed4 _Color;

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				LIGHTFACE_COORDS(1)
					FOG_COORDS(2)
			};
			
			v2f vert (appdata_t v)
			{
				v2f o;
	
				float percentage =  1 - v.color.r;
				v.vertex.x += (v.color.g - 0.5)*(v.texcoord.y * cos(_Time.y * _MinSpeed) * _MinSwing + v.texcoord.y * sin(v.color.r * UNITY_PI * 2) * _RockFactor * percentage);
				v.vertex.z += (v.color.g - 0.5)*(v.texcoord.y * sin(_Time.y * _MinSpeed) * _MinSwing + v.texcoord.y * sin(v.color.r * UNITY_PI * 2) * _RockFactor * percentage);
				
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

				WORLD_POS
				
				LIGHTFACE_VS
					FOG_VS

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 tex = tex2D(_MainTex, i.uv);
				tex.rgb *= _Color.rgb;
				clip(tex.a -0.2);
				
				LIGHTMAP_PS
				LIGHTFACE_PS
					FOG_PS
			
				return tex;
			}
			ENDCG 
		}
	}
}
}

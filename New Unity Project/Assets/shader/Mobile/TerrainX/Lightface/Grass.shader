
Shader "GOE/Scene/Grass" {
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
				#pragma multi_compile SM_3_OFF SM_3_ON
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

	#if defined(SM_3_ON)
				sampler2D _GrassShader_ObstacleTex;
	#else
				float3 _GrassShader_ObstaclePos;
				float _GrassShader_WaveDistance;
				float _GrassShader_WaveTime;
	#endif
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
	#if defined(SM_3_ON)
					float4 obstacleColor = tex2Dlod(_GrassShader_ObstacleTex, float4(v.texcoord1.xy, 0, 0));
					v.color.r = obstacleColor.a;//for test
					//obstacleColor.a = 0;
					v.vertex.x += (v.color.g - 0.5)*(v.color.b * cos(_Time.y * _MinSpeed) * _MinSwing + v.color.b * cos(_Time.y * 10 * (v.texcoord1.x*0.2 + 0.3))  * obstacleColor.a * _RockFactor * 0.5);
					v.vertex.z += (v.color.g - 0.5)*(v.color.b * sin(_Time.y * _MinSpeed) * _MinSwing + v.color.b * cos(_Time.y * 20 * (v.texcoord1.x*0.2 + 0.3))  * obstacleColor.a * _RockFactor * 0.5);
	#else
					//刺客还没有刷顶点色，暂时先注释
					//float3 waveDir = normalize(worldPos.xyz - _GrassShader_ObstaclePos);
					//float distMulti = (_GrassShader_WaveDistance - min(_GrassShader_WaveDistance, distance(worldPos.xyz, _GrassShader_ObstaclePos))) / _GrassShader_WaveDistance;
					//v.vertex.x += (v.color.g - 0.5)*(v.color.b * cos(_Time.y * _MinSpeed) * _MinSwing + waveDir.x * distMulti * v.color.b * _RockFactor);
					//v.vertex.z += (v.color.g - 0.5)*(v.color.b * sin(_Time.y * _MinSpeed) * _MinSwing + waveDir.z * distMulti * v.color.b * _RockFactor);
	#endif
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
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

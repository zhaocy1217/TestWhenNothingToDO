Shader "GOE/Scene/Tree" {
	Properties
	{
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_MainTex("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
		_IllumFactor("Illumin Factor", Range(0,2)) = 1
		_WindSpeed("Wind Speed", Range(0,1)) = 0.1
		_WaveSize("Wave Size", Range(0,1)) = 0.5
		_WindAmount("Wind Amount", Range(0,1)) = 1

		_Lux_SnowAlbedoNormal("Snow: Albedo(RG) Normal(BA)", vector) = (1,1,1,1)
	}


		SubShader
		{

		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True"}

		cull off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
			#pragma multi_compile SM_3_OFF SM_3_ON
			#pragma multi_compile SNOW_OFF SNOW_ON
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			#include "Lighting.cginc"
			#include "LightFace.cginc"
			#include "Weather.cginc"

			float _WindSpeed;
			float _WaveSize;
			float _WindAmount;
			sampler2D _MainTex;
			fixed4 _Color;
			fixed _Shininess;
			fixed _IllumFactor;
			float  _MulColor;
			fixed _Cutoff;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				LIGHTMAP_COORDS(1)
				float4 color :COLOR;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4	pos : SV_POSITION;
				float4	uv : TEXCOORD0;
				LIGHTFACE_COORDS(1)
				FOG_COORDS(2)
				LOD_DITHER_COORDS(3)
				NORMAL_COORDS(4, 5, 6, 7)
			};

			float4 GrassWave(float4 vertex, fixed alpha)
			{
				float4 _WaveAndDistance = float4(_WindSpeed * _Time.x , _WaveSize, _WindAmount, 1);	// wind speed, wave size, wind amount, max sqr distance
				float waveAmount = 0.5 * _WindAmount;
				float4 _waveXSize = float4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y;
				float4 _waveZSize = float4 (0.006, .02, 0.02, 0.05) * _WaveAndDistance.y;
				float4 waveSpeed = float4 (1.2, 2.0, 1.6, 4.8);

				float4 _waveXmove = float4(0.024, 0.04, -0.12, 0.096);
				float4 _waveZmove = float4 (0.006, .02, -0.02, 0.1);

				float4 waves;
				waves = vertex.x * _waveXSize;
				waves += vertex.z * _waveZSize;

				// Add in time to model them over time
				waves += _WaveAndDistance.x * waveSpeed;

				float4 s, c;
				waves = frac(waves);
				FastSinCos(waves, s,c);

				s = s * s;

				s = s * s;

				s = s * waveAmount;

				float3 waveMove = float3 (0,0,0);
				waveMove.x = dot(s, _waveXmove);
				waveMove.z = dot(s, _waveZmove);

				vertex.xz -= waveMove.xz * _WaveAndDistance.z;
				return vertex;
			}

			v2f vert(appdata v)
			{
				v2f o;
				o.uv.xy = v.texcoord.xy;
				WORLD_POS
				float4 newPos = GrassWave(worldPos, v.color.w);
				o.pos = mul(UNITY_MATRIX_VP, newPos);

				LIGHTMAP_VS
				LIGHTFACE_VS
				FOG_VS
				NORMAL_VS
				LOD_DITHER_VS
				
				return o;

			}

			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				clip(tex.a-_Cutoff);
				tex = _Color * tex;
				tex.xyz = tex.xyz *  _IllumFactor;

				LIGHTMAP_PS
				LIGHTFACE_PS
				SNOW_PS_TREE
				FOG_PS
				LOD_DITHER_PS
				
				return tex;
			}

			ENDCG
		}
	}

}



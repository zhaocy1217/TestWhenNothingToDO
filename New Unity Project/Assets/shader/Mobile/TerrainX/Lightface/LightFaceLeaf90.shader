Shader "GOE/LightFace/LightFaceLeaf90" 
{
Properties 
{
	_ShadowFactor("Shadow LightFace Factor", Range(0,1)) = 0.4
	_TintColor("Tint Color", Color) = (1,1,1,1)
	_NightColor("NightColor", Color) = (1,1,1,1)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_WindSpeed ("Wind Speed", Range(0,5)) = 0.1
	_WaveSize ("Wave Size", Range(0,5)) = 0.5
	_WindAmount ("Wind Amount", Range(0,5)) = 1
}

SubShader {

	Tags {"Queue"="Geometry" "IgnoreProjector"="True"}

	ColorMask RGB
	Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }
	Pass 
	{
		Blend  srcalpha oneminussrcalpha, zero one
		CGPROGRAM
			#pragma exclude_renderers d3d11 flash gles3 metal
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "TerrainEngine.cginc"
			#include "Lighting.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			fixed _ShadowFactor;
			float _WindSpeed;
			float _WaveSize;
			float _WindAmount;
			fixed4 _TintColor;
			fixed4 _NightColor;
			float _GameTime;
			
			struct v2f
			{
				float4	pos : SV_POSITION;
				fixed4 color : COLOR;
				half2	uv : TEXCOORD0;
			}; 
			
			float4 GrassWave(float4 vertex, fixed alpha)
			{
				float4 _WaveAndDistance = float4(_WindSpeed * _Time.x , _WaveSize, _WindAmount, 1);	// wind speed, wave size, wind amount, max sqr distance
				float waveAmount = 0.5 * _WindAmount;
				float4 _waveXSize = float4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y;
				float4 _waveZSize = float4 (0.006, .02, 0.02, 0.05) * _WaveAndDistance.y;
				float4 waveSpeed = float4 (0.3, .5, .4, 1.2) * 4;
				
				float4 _waveXmove = float4(0.012, 0.02, -0.06, 0.048) * 2;
				float4 _waveZmove = float4 (0.006, .02, -0.02, 0.1);
				
				float4 waves;
				waves = vertex.x * _waveXSize;
				waves += vertex.z * _waveZSize;
				
				// Add in time to model them over time
				waves += _WaveAndDistance.x * waveSpeed;
				
				float4 s, c;
				waves = frac (waves);
				FastSinCos (waves, s,c);
				
				s = s * s;
					
				s = s * s;
				
				s = s * waveAmount;
				
				float3 waveMove = float3 (0,0,0);
				waveMove.x = dot (s, _waveXmove);
				waveMove.z = dot (s, _waveZmove);
				
				vertex.xz -= waveMove.xz * _WaveAndDistance.z;
				
				return vertex;
			}
			
			struct appdata
			{
			    float4 vertex : POSITION;
			    fixed4 color : COLOR;
			    float4 texcoord : TEXCOORD0;
			};
			

			v2f vert (appdata v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.color = v.color;
				float4  viewPos = mul(UNITY_MATRIX_MV, v.vertex);
				viewPos.x = GrassWave(viewPos, v.color.w);  //sin(_Time.y);//
				o.pos =  mul(UNITY_MATRIX_P, viewPos);
				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				//c.a *= c.r;
				c.rgb = _ShadowFactor * lerp(_NightColor, _TintColor, _GameTime);
				return  c;
			}

		ENDCG
	}	

}
}




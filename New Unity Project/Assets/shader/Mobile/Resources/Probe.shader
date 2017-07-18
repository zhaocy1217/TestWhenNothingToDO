Shader "GOE/Effect/Probe"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_dark("_dark Factor", Range(-1,1)) = 0
		_Light("_Light Factor", Range(0,4)) = 1
		//_GameTime("GameTime", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			Blend Dstcolor  SrcColor
			ColorMask RGB
			ZTest Always
			cull off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			//fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Light;
			fixed _dark;
			float _GameTime;
			v2f vert (appdata v)
			{
				v2f o;
				//o.vertex =   mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex = float4(v.vertex.xyz, 1);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col.r += _dark;
				col.r = min(col.r, 1);
				col.r = max(col.r, -1);
				//col.rgb = col.r * _Light;
				col.rgb = lerp(col.r * _Light, 0.5, _GameTime);
				return col;
			}
			ENDCG
		}
	}
}

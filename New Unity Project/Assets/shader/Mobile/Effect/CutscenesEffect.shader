Shader "GOE/ImageEffect/CutscenesEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_2ndTex("2nd Texture", 2D) = "white" {}
		_lerp("Lerp", Range(0,1))=0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			sampler2D _MainTex;
			sampler2D _2ndTex;
			fixed _lerp;


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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}


			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 secondCol = tex2D(_2ndTex, i.uv);

				fixed4 finalCol = lerp(secondCol, col, _lerp);
				return finalCol;
			}
			ENDCG
		}
	}
}

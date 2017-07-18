Shader "Hidden/Blend"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainTex2 ("Texture", 2D) = "white" {}
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

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;


			fixed4 frag (v2f IN) : SV_Target
			{
				float distance = 0.005;
                fixed4 computedColor = tex2D(_MainTex, IN.uv)  ;
								
				float rightOffset = min(IN.uv.x + distance, 1 - distance);
				float downOffset = min(IN.uv.y + distance, 1 - distance);
				float leftOffset = max(IN.uv.x - distance, distance);
				float upOffset = max(IN.uv.y - distance, distance);

				computedColor += tex2D(_MainTex, half2( rightOffset,   downOffset)) ;
				computedColor += tex2D(_MainTex, half2( rightOffset ,  IN.uv.y)) ;
				computedColor += tex2D(_MainTex, half2(IN.uv.x , downOffset )) ;
				computedColor += tex2D(_MainTex, half2( leftOffset,    upOffset ));
				computedColor += tex2D(_MainTex, half2(rightOffset , upOffset )) ;
				computedColor += tex2D(_MainTex, half2(leftOffset , downOffset ));
				computedColor += tex2D(_MainTex, half2(leftOffset , IN.uv.y)) ;
				computedColor += tex2D(_MainTex, half2(IN.uv.x , upOffset ));
				
                computedColor = computedColor / 9;


                return computedColor;
			}
			ENDCG
		}
	}
}

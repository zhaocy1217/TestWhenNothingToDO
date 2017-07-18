Shader "GOE/ImageEffect/LowHP"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//_Mask ("mask (RGB)", 2D) = "white" {}
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
			
			sampler2D _MainTex;
			//sampler2D _Mask;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				//fixed4 mask = tex2D(_Mask,  i.uv);
				//col.rgb = col.rgb*(1-mask.a) + mask.rgb*(mask.a);

				fixed temp = (0.2990*col.r  + 0.587*col.g +0.114*col.b);
				fixed3 bw = fixed3(temp, temp, temp);
				fixed3 red = fixed3(col.r*1, col.g*0.8 , col.b*0.8 );

				//col.rgb = lerp(bw, red, sin(_Time.y*1));

				float t=  abs(sin(_Time.y*2));
				//max(0, abs(sin(_Time.y*2)) - 0.5f);
				//t*=10;
				//t = min(t, 1);
				col.rgb = bw*t + red*(1-t);
				return col;
			}
			ENDCG
		}
	}
}

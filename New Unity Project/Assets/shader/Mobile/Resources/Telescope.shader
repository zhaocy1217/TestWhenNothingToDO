// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/ImageEffect/Telescope" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BackgroundColor("Background Color", Color) = (0, 0, 0, 1)
		_Parameters("Circle Parameters", Vector) = (0.5, 0.5, 300, 1) // Center: (x, y), Radius: z  
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert      
			#pragma fragment frag      
			#pragma fragmentoption ARB_precision_hint_fastest       

			#include "UnityCG.cginc"     
			#pragma target 3.0      

			sampler2D _MainTex;
			float4 _Parameters;
			float4 _BackgroundColor;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 srcPos : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};

			//precision highp float;  
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.srcPos = ComputeScreenPos(o.pos);
				o.uv = v.texcoord;
				return o;
			}

			float4 circle(float2 pos, float2 center, float radius, float3 color, float antialias)
			{
				float d = length(pos - center) - radius;
				float t = smoothstep(0, antialias, d);
				return float4(color, 1.0 - t);
			}

			fixed4 frag(v2f _iParam) : COLOR0
			{
				float4 tex = tex2D(_MainTex, _iParam.uv);
				//屏幕中的坐标，以pixel为单位  
				float2 pos = (_iParam.srcPos.xy / _iParam.srcPos.w)*_ScreenParams.xy;
				float4 layer1 = float4(_BackgroundColor.rgb, 1.0);
				float4 layer2 = circle(pos, _Parameters.xy * _ScreenParams.xy, _Parameters.z, tex.rgb, _Parameters.w);
				return lerp(layer1, layer2, layer2.a);
			}


			ENDCG
		}
	}
	FallBack Off
}
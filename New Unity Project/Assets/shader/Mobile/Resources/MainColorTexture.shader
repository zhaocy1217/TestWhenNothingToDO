// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/MainColorTexture" 
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "Queue" = "Geometry-50" }
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			fixed4 _Color;
			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
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
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			//sampler2D _MainTex;
			//sampler2D _Mask;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c =  tex2D(_MainTex, i.uv);
				fixed4 temp = fixed4(_Color.r, _Color.g, _Color.b, c.a);
				return temp;
			}
			ENDCG
		}
	}
}






// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Grid" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SampleOffset("SampleOffset", Float) = 0.01
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 300
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Always
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv_MainTex : TEXCOORD0;
	};

	float4 _MainTex_ST;
	float4 _Color;
	half _SampleOffset;
	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	sampler2D _MainTex;

	float4 frag(v2f IN) : COLOR{
		half4 left = tex2D(_MainTex, IN.uv_MainTex+ half2(-_SampleOffset, 0));
		half4 right = tex2D(_MainTex, IN.uv_MainTex + half2(_SampleOffset, 0));
		half4 up = tex2D(_MainTex, IN.uv_MainTex + half2(0, _SampleOffset));
		half4 down = tex2D(_MainTex, IN.uv_MainTex + half2(0, -_SampleOffset));

		half4 leftup = tex2D(_MainTex, IN.uv_MainTex + half2(-_SampleOffset, _SampleOffset));
		half4 rightdown = tex2D(_MainTex, IN.uv_MainTex + half2(_SampleOffset, -_SampleOffset));
		half4 rightup = tex2D(_MainTex, IN.uv_MainTex + half2(_SampleOffset, _SampleOffset));
		half4 leftdown = tex2D(_MainTex, IN.uv_MainTex + half2(-_SampleOffset, -_SampleOffset));

		half4 c = tex2D(_MainTex, IN.uv_MainTex);
		c = (left + right + up + down + c + leftup+ rightdown+ rightup + leftdown) / 8 * _Color;
		return c;
	}
		ENDCG
	}
	}
}

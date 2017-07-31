// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Effect/LightfaceWaterFlare"
{
	Properties
	{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
		_Noise("Noise", 2D) = "bump" {}
		_bloom("Bloom Factor", Range(0,10)) = 2
	}

		SubShader{

		Tags{ "Queue" = "Geometry+500" "IgnoreProjector" = "True" }


		LOD 200
		Pass
	{

		cull off
		Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }
		Blend  DstAlpha OneMinusSrcColor,zero one
		ColorMask RGB
		CGPROGRAM
		//#pragma exclude_renderers d3d11 flash gles3 metal
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest

		#include "UnityCG.cginc"


		sampler2D _MainTex;
		fixed4 _TintColor;
		float4 _MainTex_ST;
		sampler2D _Noise;
		float4 _Noise_ST;
		fixed _bloom;
		fixed _NightLightScale;
		struct v2f
		{
			float4	pos : SV_POSITION;
			fixed4 color : COLOR;
			half2	uv : TEXCOORD0;
			half2	uv2 : TEXCOORD1;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float4 texcoord : TEXCOORD0;
		};


		v2f vert(appdata v)
		{
			v2f o;
			o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);//v.texcoord.xy;
			o.uv2 = v.texcoord;
			o.color = v.color;
			o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
			return o;
		}


		float4 frag(v2f i) : COLOR
		{
			//				fixed4 tex = tex2D(_MainTex, i.uv);
			//				tex.rgb *= _TintColor;
			float2 uv = i.uv2.xy;
			uv.x += _Time.x;
			fixed4 noise = tex2D(_Noise, TRANSFORM_TEX(uv, _Noise));
			noise.xy -= 0.5f;
			noise.xy *= 2;
			uv.xy += i.uv.xy + noise.xy/5;
			fixed4 c = (i.color.r)*_TintColor * tex2D(_MainTex, uv);
			
			//fixed4 c = i.color * _TintColor * tex2D(_MainTex, i.uv);
			_NightLightScale = 1;
			c *= (_bloom * _NightLightScale);
			return  c;
		}

		ENDCG
	}
	}
}

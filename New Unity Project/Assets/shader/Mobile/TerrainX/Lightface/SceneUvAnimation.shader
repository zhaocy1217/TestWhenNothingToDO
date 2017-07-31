// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Scene/SceneUvAnimation" 
{
	Properties
	{
		_TintColor("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
		Speed("纹理滚动速度", Range(0.01, 50.0)) = 1
		Direction("纹理滚动方向", Range(-1, 1)) = 1
		FogScale("雾的强度", Range(0, 4)) = 1
	}


		SubShader{

		Tags{ "Queue" = "Geometry+1000" "IgnoreProjector" = "True" }
		LOD 200
		ColorMask RGB
		Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }
		Pass
	{

		Tags{ "LightMode" = "ForwardBase" }

		Blend One One
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"
#include "LightFace.cginc"

		sampler2D _MainTex;
	fixed4 _TintColor;
	float4 _MainTex_ST;
	float Speed;
	float Direction;
	half FogScale;
	struct v2f
	{
		float4	pos : SV_POSITION;
		fixed4 color : COLOR;
		half2	uv : TEXCOORD0;
		FOG_COORDS(1)
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
		o.color = v.color;
		o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
		WORLD_POS
		FOG_VS
		return o;
	}


	float4 frag(v2f i) : COLOR
	{
		i.uv.x += _Time.x * Speed * Direction;
	fixed4 tex = i.color * _TintColor * tex2D(_MainTex, i.uv);
	FOG_PS
		tex.a = 1;
	return tex;
	}

		ENDCG
	}
	}
}





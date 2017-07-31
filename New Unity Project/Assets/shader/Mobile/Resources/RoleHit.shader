// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Role Hit"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	_RefTex("Ref Tex (RGB)", 2D) = "black" {}
	_HitTex("Hit Tex (RGB)", 2D) = ""{}
	_Emission("Emission", Range(0,1)) = 0.3
	}

		Subshader
	{
		Tags{ "Queue" = "Geometry+200" "IgnoreProjector" = "True" }
		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }
		cull off
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma exclude_renderers d3d11 flash
#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
#include "UnityCG.cginc"
#include "LightFace.cginc"

	struct v2f
	{
		float4 pos	: SV_POSITION;
		float4 uv 	: TEXCOORD0;
		float4 uv_ref_lightface	: TEXCOORD1;
	};

	uniform float4 _MainTex_ST;
	float4x4 _texView;
	float _Emission;

	v2f vert(appdata_base v)
	{
		v2f o;
		WORLD_POS

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex).xy;

		half2 capCoord;
		capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
		capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
		o.uv.zw = capCoord * 0.5f + 0.5f;

		float3 worldNormal = UnityObjectToWorldNormal(v.normal.xyz);
		o.uv_ref_lightface.x = max(0.2,dot(worldNormal, normalize(_WorldSpaceLightPos0.xyz)));
		o.uv_ref_lightface.y = 0;
#ifdef LIGHT_FACE_ON
		o.uv_ref_lightface.zw = LightFaceUV(worldPos).xy;
#else
		o.uv_ref_lightface.zw = half2(0,0);
#endif
		return o;
	}

	uniform sampler2D _MainTex;
	uniform sampler2D _RefTex;
	uniform sampler2D _HitTex;

	fixed4 _Color;

	fixed4 frag(v2f i) : COLOR
	{
		fixed4 tex = tex2D(_MainTex, i.uv.xy);
	fixed4 ref = tex2D(_RefTex, i.uv.zw);
	fixed4 hit = tex2D(_HitTex, i.uv.zw);
	tex.rgb *= _Color;

	tex.rgb = tex.rgb + tex.rgb * _LightColor0 * i.uv_ref_lightface.x + ref.rgb * tex.a * 2;

	LightFaceColorRole(tex.rgba, i.uv_ref_lightface.zw, 1, 1);

	//tex.rgb =  tex.rgb * light * lightTest * _LColor.rgb;
	//fixed temp = (tex.x + tex.y +tex.z)* 0.35f;
	//tex.a = temp;

	tex.rgb += ((1 - hit.a) * fixed3(0.529, 0.749, 0.843));
	tex.rgb +=  0.5 * fixed3(0.529, 0.749, 0.843);
	//tex.rgb += _Emission;
	return tex;
	}
		ENDCG
	}
	}
}
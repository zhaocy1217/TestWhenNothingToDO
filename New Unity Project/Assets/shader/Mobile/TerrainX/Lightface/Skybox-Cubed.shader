// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "GOE/Scene/Skybox-Cubemap" {
Properties {
	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
	[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
	_Rotation ("Rotation", Range(0, 360)) = 0
	[NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
}

SubShader {
	Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" }
	Cull off 
	ZWrite On

	Pass {
		
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
		#include "LightFace.cginc"

		#include "UnityCG.cginc"

		samplerCUBE _Tex;
		half4 _Tex_HDR;
		half4 _Tint;
		half _Exposure;
		float _Rotation;

		float4 RotateAroundYInDegrees (float4 vertex, float degrees)
		{
			float alpha = degrees * UNITY_PI / 180.0;
			float sina, cosa;
			sincos(alpha, sina, cosa);
			float2x2 m = float2x2(cosa, -sina, sina, cosa);
			return float4(mul(m, vertex.xz), vertex.yw).xzyw;
		}
		
		struct appdata_t {
			float4 vertex : POSITION;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			float3 uv : TEXCOORD0;
			LIGHTFACE_COORDS(1)
				FOG_COORDS(2)
		};

		v2f vert (appdata_t v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, RotateAroundYInDegrees(v.vertex, _Rotation));
			o.uv = v.vertex.xyz;

			float4 worldPos = mul(unity_ObjectToWorld, RotateAroundYInDegrees(v.vertex, _Rotation));
			
			LIGHTFACE_VS
				FOG_VS

			return o;
		}

		fixed4 frag (v2f i) : SV_Target
		{
			half4 tex = texCUBE (_Tex, i.uv);

			tex.rgb *= _Tint.rgb;
			tex *= _Exposure;

			LIGHTMAP_PS
			LIGHTFACE_PS
			FOG_PS

			return tex;
		}
		ENDCG 
	}
} 	


}

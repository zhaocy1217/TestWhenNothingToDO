Shader "GOE/Scene/Lightmap Cubmap Receive Shadow"
{
	Properties
	{
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
		_IllumFactor("Illumin Factor", Range(1,2)) = 1
		_Cubemap("CubeMap", CUBE) = ""{}					    	
		_ReflAmount("CubeMap Reflection Amount", Range(0.01, 2)) = 1
		_MaskTex("Mask (RGB)", 2D) = "white" {}

		_Lux_RainRippleWater("Rain: Ripple(RG) Water(BA)", vector) = (0.6, 1, 1, 1)
	}


		SubShader{

		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" }
		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }
		Fog{ Mode off }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		//#pragma multi_compile_fwdbase
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma exclude_renderers d3d11 flash			
		#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
		#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
		#pragma multi_compile RAIN_OFF RAIN_ON
		#pragma multi_compile SM_3_OFF SM_3_ON
		#pragma target 3.0

		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "LightFace.cginc"
		#include "UnityLightingCommon.cginc"
		#include "Weather.cginc"

		sampler2D _MainTex;
		half4 _MainTex_ST;
		fixed4 _Color;
		sampler2D _MaskTex;
		samplerCUBE _Cubemap;
		float _ReflAmount;
		half _IllumFactor;

		struct v2f
		{
			float4	pos : SV_POSITION;
			half4	uv : TEXCOORD0;
			LIGHTFACE_COORDS(1)
			FOG_COORDS(2)
			float3	reflect : TEXCOORD3;
			NORMAL_COORDS(4, 5, 6, 7)
		};


		struct appdata
		{
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
			LIGHTMAP_COORDS(1)
			float4 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		v2f vert(appdata v)
		{
			WORLD_POS
			v2f o;
			o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

			float3 I = worldPos.xyz - _WorldSpaceCameraPos.xyz;
			I = normalize(I);
			float3 N = UnityObjectToWorldNormal(v.normal);
			N = normalize(N);
			o.reflect = reflect(I, N);

			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);


			LIGHTMAP_VS
			LIGHTFACE_VS
			LIGHTFACE_VS_SHADOW
			FOG_VS
			NORMAL_VS

			return o;
		}


		float4 frag(v2f i) : COLOR
		{
			RAIN_PS_UV

			fixed4 tex = tex2D(_MainTex, i.uv.xy);
			tex.rgb *= _Color;
			tex.rgb *= _IllumFactor;

			fixed4 mask = tex2D(_MaskTex, i.uv.xy);
			tex.rgb += mask.r * tex.rgb * texCUBE(_Cubemap, i.reflect).rgb * _ReflAmount;   //叠加反射效果
		
			LIGHTMAP_PS
			LIGHTFACE_PS_SHADOW
			FOG_PS

			return tex;
		}

		ENDCG
	}
		}
}





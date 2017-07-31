// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "GOE/Role/RoleOutlineSpecDissolve" 
{
    Properties 
	{
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_FalloffSampler("Falloff Control", 2D) = "white" {}
		_RimLightSampler("RimLight Control", 2D) = "white" {}

		_IlluminIntensity("Illumin Intensity", Range(0, 2)) = 1
		_SpecIntensity("Specular Intensity", Range(0, 1)) = 0

		_EdgeThickness("Outline Thickness", Float) = 1
		_EdgeColor("Outline Color", Color) = (1,1,1,1)
    }

    SubShader 
	{
		Tags
		{
			"RenderType"="Opaque"
			"Queue"="Geometry"
			"LightMode"="ForwardBase"
		}

		Pass
		{
			Cull Front
			ZTest Less
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#define INV_EDGE_THICKNESS_DIVISOR 0.00185

			float _EdgeThickness;
			float4 _EdgeColor;

			struct v2f
			{
				float4 pos : SV_POSITION; 
				float2 uv : TEXCOORD0;
			};


			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				float2 offset = TransformViewToProjection(norm.xy);
				o.pos.xy += offset * o.pos.z * _EdgeThickness * INV_EDGE_THICKNESS_DIVISOR;
				o.uv = v.texcoord;

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				half4 tex = _EdgeColor;
				return tex;
			}
			ENDCG
		}
		
        Pass 
		{
			Cull Off
			ZTest LEqual
            CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#define FALLOFF_POWER 1.0
			#define float_t  half
			#define float2_t half2
			#define float3_t half3
			#define float4_t half4

			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform sampler2D _FalloffSampler;
			uniform sampler2D _RimLightSampler;

			uniform float _IlluminIntensity;
			uniform float _SpecIntensity;
			uniform float4 _LightColor0;

			struct v2f
			{
				float4 pos    : SV_POSITION;
				float3 normal : TEXCOORD1;
				float2 uv     : TEXCOORD2;
				float3 eyeDir : TEXCOORD3;
				float3 lightDir : TEXCOORD4;
			};

			v2f vert (appdata_base v) 
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.normal = normalize(mul(unity_ObjectToWorld, float4_t(v.normal, 0)).xyz);

				// Eye direction vector
				float4_t worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.eyeDir = normalize(_WorldSpaceCameraPos - worldPos);

				o.lightDir = WorldSpaceLightDir(v.vertex);

				return o;
			}

			fixed4 frag(v2f i) : COLOR 
			{
				float4_t diffSamplerColor = tex2D(_MainTex, i.uv);

				// Falloff. Convert the angle between the normal and the camera direction into a lookup for the gradient
				float_t normalDotEye = dot(i.normal, i.eyeDir);
				float_t falloffU = clamp(1 - abs(normalDotEye), 0.02, 0.98);
				float4_t falloffSamplerColor = FALLOFF_POWER * tex2D(_FalloffSampler, float2(falloffU, 0.25f));
				float3_t combinedColor = lerp(diffSamplerColor.rgb, falloffSamplerColor.rgb * diffSamplerColor.rgb, falloffSamplerColor.a);

				// Rimlight
				float_t rimlightDot = saturate(0.5 * (dot(i.normal, i.lightDir) + 1.0));
				falloffU = saturate(rimlightDot * falloffU);
				//falloffU = saturate( ( rimlightDot * falloffU - 0.5 ) * 32.0 );
				falloffU = tex2D(_RimLightSampler, float2(falloffU, 0.25f)).r;
				float3_t lightColor = diffSamplerColor.rgb * 0.5; // * 2.0;
				combinedColor += falloffU * lightColor;

				return float4_t(combinedColor, diffSamplerColor.a) * _Color * _LightColor0 * _IlluminIntensity;
			}
			
            ENDCG
        }
    }
}
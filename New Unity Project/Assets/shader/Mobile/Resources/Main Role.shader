// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Main Role"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OcclusionTex("Occlusion Tex (RGB)", 2D) = "black" {}
        _EmissionIntensity ("EmissionIntensity", Range(0, 2)) = 1
        _FresnelPow ("FresnelPow", Range(1, 11)) = 3
        _BackColor ("BackColor", Color) = (0.1843137,0.4627451,0.9176471,1)
        _RimColor ("RimColor", Color) = (0.9333333,0.682353,0.3647059,1)
		_SpecularColor("SpecularColor", Color)= (1,1,1,1)
		_SpecularPow("SpecularPow", Range(0.1, 1)) = 1
        _CubeMap ("CubeMap", Cube) = "_Skybox" {}
        _CubeMapIntensity ("CubeMapIntensity", Range(0, 4)) = 0
	}
	
	Subshader
	{
		Tags { "Queue"="Geometry" "IgnoreProjector"="True" }
		// 先画被遮挡部分
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			zwrite off
			ZTest Greater
			Blend Srcalpha oneminusSrcalpha
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
			struct v2f
			{
				float4 pos	: SV_POSITION;
				float2 uv 	: TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				half2 capCoord;
				capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz, v.normal);
				capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz, v.normal);
				o.uv.xy = capCoord * 0.5f + 0.5f;
				return o;
			}
			uniform sampler2D _OcclusionTex;
			fixed4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_OcclusionTex, i.uv.xy);
				return tex;
			}
			ENDCG
		}
		
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

			#pragma target 2.0

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float _EmissionIntensity;
			uniform float _FresnelPow;
			uniform float4 _BackColor;
			uniform float4 _RimColor;
			uniform float4 _SpecularColor;
			uniform float _SpecularPow;
			uniform samplerCUBE _CubeMap;
			uniform float _CubeMapIntensity;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				LIGHTFACE_COORDS(3)
				FOG_COORDS(4)
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				WORLD_POS
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex).xy;
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.posWorld = worldPos;

				LIGHTFACE_VS
				FOG_VS
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed3 normalDirection = normalize(i.normalDir);
				fixed3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				fixed3 viewReflectDirection = reflect(-viewDirection, normalDirection);
				fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				fixed diff = max(0, dot(lightDirection, normalDirection));

				fixed4 tex = tex2D(_MainTex,TRANSFORM_TEX(i.uv.xy, _MainTex));
				tex.rgb *= _Color.rgb;
				fixed3 emissive = lerp(tex.rgb* _EmissionIntensity, texCUBE(_CubeMap, viewReflectDirection).rgb*_CubeMapIntensity, tex.a);

				tex.rgb = emissive
					+ tex.rgb * diff * _LightColor0.rgb
					+ tex.rgb * max(0,dot((-1 * lightDirection),normalDirection)) * _BackColor.rgb
					+ pow(1.0 - max(0,dot(normalDirection, viewDirection)),_FresnelPow) * diff * _RimColor.rgb
					+ pow(max(0, dot(normalDirection, viewDirection)), lerp(1, 200, _SpecularPow))* _SpecularColor.rgb * tex.a * 2;

				LIGHTFACE_PS_ROLE
				FOG_PS

				return tex;
			}
			ENDCG
		}
	}
	
}

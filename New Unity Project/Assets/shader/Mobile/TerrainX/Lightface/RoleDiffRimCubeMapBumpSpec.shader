// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "GOE/Role/RoleDiffRimCubeMapBumpSpec" {
    Properties 
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _EmissionIntensity ("EmissionIntensity", Range(0, 2)) = 1
        _FresnelPow ("FresnelPow", Range(1, 11)) = 3
        _BackColor ("BackColor", Color) = (0.1843137,0.4627451,0.9176471,1)
        _RimColor ("RimColor", Color) = (0.9333333,0.682353,0.3647059,1)
		_SpecularColor("SpecularColor", Color)= (1,1,1,1)
		_SpecularPow("SpecularPow", Range(0.1, 1)) = 1
        _CubeMap ("CubeMap", Cube) = "_Skybox" {}
        _CubeMapIntensity ("CubeMapIntensity", Range(0, 4)) = 0
		_BumpTex("Bump Tex", 2D) = "bump"{}
		_BumpIntensity("BumpIntensity", Range(0,1)) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			cull off
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma exclude_renderers d3d11 flash
				#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
				#pragma multi_compile NORMAL_ON NORMAL_OFF 
				#include "UnityCG.cginc"
				#include "LightFace.cginc"
				
				#pragma target 3.0
				
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
				uniform sampler2D _BumpTex;
				uniform float4 _BumpTex_ST;
				uniform float _BumpIntensity;
			
				struct VertexInput {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					LIGHTFACE_COORDS(1)
					FOG_COORDS(2)
					NORMAL_COORDS(3, 4, 5, 6)
				};
				
				v2f vert (VertexInput v)
				{
					v2f o;
					WORLD_POS
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex).xy;
					//o.normalDir = UnityObjectToWorldNormal(v.normal);
					//o.posWorld = worldPos;

					LIGHTFACE_VS
					FOG_VS
					NORMAL_VS
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{ 
					float3 normalDirection = normalize(i.normalDir);
#if ((defined (SM_3_ON)) && (defined (NORMAL_ON))) 
					normalDirection = NORMAL_PS_Normal(_BumpTex, i.uv.xy, normalDirection, i.tangentDir, i.bitangentDir, _BumpIntensity);				
#endif
					fixed3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
					fixed3 viewReflectDirection = reflect( -viewDirection, normalDirection );
					fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					fixed diff = max(0, dot(lightDirection, normalDirection));
					fixed spec = max(0, dot(normalDirection, viewDirection));
					fixed backDiff = max(0, dot((-1 * lightDirection), normalDirection));

					fixed4 tex = tex2D(_MainTex,TRANSFORM_TEX(i.uv.xy, _MainTex));
					tex.rgb *= _Color.rgb;
					fixed3 emissive = lerp(tex.rgb * _EmissionIntensity, texCUBE(_CubeMap, viewReflectDirection).rgb * _CubeMapIntensity, tex.a);

					tex.rgb = emissive
					+ tex.rgb * diff * _LightColor0.rgb
					+ tex.rgb * backDiff * _BackColor.rgb
					+ pow(1.0 - spec, _FresnelPow) * diff * _RimColor.rgb
					+ pow(spec, lerp(1, 200, _SpecularPow)) * _SpecularColor.rgb * tex.a * 2;

					LIGHTFACE_PS_ROLE
					FOG_PS
					
					return tex;
				}
			ENDCG
		}
    }
    FallBack "Diffuse"
}
Shader "GOE/AlphaFactor" 
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_AlphaFactor ("Alpha Factor", Vector) = (1,0,1)	
		_MainTex ("Base (RGB)", 2D) = "white" {}
	_RefTex("Ref Tex (RGB)", 2D) = "black" {}
	_RefIntensity("Ref Intensity", Range(0,3)) = 2
		_CapTex("Cap Tex (RGB)", 2D) = "white" {}
	_CapColor("Cap Color", Color) = (1,1,1,1)
		_CapIntensity("Cap Intensity", Range(0,3)) = 0
		_EmissionIntensity("Emission Intensity", Range(0,2)) = 1
		_LFFactor("Light Factor", Range(0,1)) = 0.35
	}

SubShader {

	Tags {
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType" = "Transparent"
		}		
	Pass 
	{
		Blend SrcAlpha OneMinusSrcAlpha	
		Tags {  "LightMode" = "ForwardBase" }

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers d3d11 flash d3d11_9x
#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "LightFace.cginc"
			
		uniform float4 _MainTex_ST;
	float _RefIntensity;
	float _CapIntensity;
	uniform sampler2D _MainTex;
	uniform sampler2D _RefTex;
	uniform sampler2D _CapTex;
	fixed4 _Color;
	fixed4 _CapColor;
	float _EmissionIntensity;
	float _LFFactor;
	float4 _AlphaFactor;

			struct v2f
			{
				float4	pos : SV_POSITION;
				half4	uv : TEXCOORD0;
				LIGHTFACE_COORDS(1)
					FOG_COORDS(2)
					float diff : TEXCOORD3;
			}; 
		

			v2f vert (appdata_full v)
			{
				v2f o;
				WORLD_POS

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex).xy;

				float3 worldNormal = UnityObjectToWorldNormal(v.normal.xyz);
				half3 capCoord;
				capCoord = mul(UNITY_MATRIX_V, float4(worldNormal, 0));
				o.uv.zw = capCoord.xy * 0.5f + 0.5;

				o.diff = max(0, dot(worldNormal, normalize(_WorldSpaceLightPos0.xyz)));
				o.diff = pow(o.diff, 2);

				LIGHTFACE_VS
					FOG_VS

					return o;
			}
			
			fixed4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
			fixed4 ref = tex2D(_RefTex, i.uv.zw);
			fixed4 cap = tex2D(_CapTex, i.uv.zw);
			tex.rgb *= _Color.rgb;
			tex.rgb = tex.rgb * _EmissionIntensity + tex.rgb * _LightColor0 * i.diff + ref.rgb * tex.a * _RefIntensity + cap.rgb * _CapIntensity * _CapColor;

			LIGHTMAP_PS
				//LIGHTFACE_PS_ROLE_F
				//FOG_PS

				fixed dt = 3;
				fixed tf = saturate((_Time.y - _AlphaFactor.z) / dt);
				fixed4 c;
				c.rgb = tex.rgb;
				c.a = lerp(_AlphaFactor.x, _AlphaFactor.y, tf);
				//_Color.a = c.a;
				return c;
			}

		ENDCG
	}	
}

FallBack "Diffuse"
}






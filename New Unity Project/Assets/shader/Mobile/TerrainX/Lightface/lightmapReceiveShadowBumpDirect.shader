Shader "GOE/Scene/Lightmap Receive Shadow Bump Diret" 
{
Properties 
{
	_Color("Main Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1

	_BumpTex("Bump Tex", 2D) = "bump"{}
	_BumpIntensity("BumpIntensity", Range(0,1)) = 1

	_MyDiffColor("Diff Color", Color) = (0.8, 0.8, 0.8, 0.8)
	_MySpecColor("Spec Color", Color) = (0.8, 0.8, 0.8, 0.8)
	_GlossPower("Gloss Power", Range(0,1)) = 1

	//weather
	_Lux_RainRippleWater("Rain: Ripple(RG) Water(BA)", vector) = (0.6, 1, 1, 1)
	_Lux_SnowAlbedoNormal("Snow: Albedo(RG) Normal(BA)", vector) = (1,1,1,1)
}

SubShader {

	Tags {"Queue"="Geometry" "IgnoreProjector"="True"}
	Pass 
	{	
		Tags {  "LightMode" = "ForwardBase" }
		Fog { Mode off } 
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers d3d11 flash
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma multi_compile SM_3_OFF SM_3_ON
			#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON
			#pragma multi_compile NORMAL_ON NORMAL_OFF
			#pragma multi_compile RAIN_OFF RAIN_ON
			#pragma multi_compile SNOW_OFF SNOW_ON
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			#include "LightFace.cginc"
			#include "Weather.cginc"
			
			sampler2D _MainTex;
			half4 _MainTex_ST;
			fixed4 _Color;
			half _IllumFactor;

			sampler2D _BumpTex;
			half4 _BumpTex_ST;
			float _BumpIntensity;

			fixed4 _MySpecColor;
			fixed4 _MyDiffColor;
			float _GlossPower;

			struct appdata
			{
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
				LIGHTMAP_COORDS(1)
				float4 tangent : TANGENT; 
				float3 normal : NORMAL;
			};
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				half4 uv : TEXCOORD0;
				LIGHTFACE_COORDS(1)
				FOG_COORDS(2)
				NORMAL_COORDS(3,4,5,6)
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex); 

				WORLD_POS
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
				float4 tex = tex2D(_MainTex, i.uv.xy);
				tex.rgb *= _Color * _IllumFactor;

				float3 normalDirection = normalize(i.normalDir);
#if ((defined (SM_3_ON)) && (defined (NORMAL_ON))) 
				normalDirection = NORMAL_PS_Normal(_BumpTex, i.uv.xy, normalDirection, i.tangentDir, i.bitangentDir, _BumpIntensity);
#endif
				float3 viewDirection = normalize(UnityWorldSpaceViewDir(i.posWorld.xyz));
				float3 lightDirection = normalize(UnityWorldSpaceLightDir(i.posWorld.xyz));
				float3 halfDirection = normalize(viewDirection + lightDirection);
				float diff = dot(normalDirection, lightDirection) * 0.5 + 0.5;
				float spec = dot(normalDirection, halfDirection) * 0.5 + 0.5;

				float3 diffColor = tex.rgb * diff *  _MyDiffColor;
				float3 specColor = pow(spec, lerp(1, 200, _GlossPower)) * _MySpecColor.rgb * tex.a * 2;
				tex.rgb = /*tex.rgb +*/ diffColor + specColor;

				LIGHTMAP_PS
				LIGHTFACE_PS_SHADOW
				SNOW_PS_BUMP
				FOG_PS

				return tex;
			}

		ENDCG
	}	
}
}




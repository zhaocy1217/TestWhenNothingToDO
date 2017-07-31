// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Scene/Lightmap Reflect Receive Shadow" 
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_RefTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_RefColor("Ref Color", Color) = (1,1,1,1)
	_EnvironmentPower ("Environment Factor", Range(0,1)) = 0
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1

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
			#pragma multi_compile RAIN_OFF RAIN_ON
			#pragma multi_compile SNOW_OFF SNOW_ON
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "LightFace.cginc"
			#include "Weather.cginc"
			
			sampler2D _MainTex;
			sampler2D _RefTex;
			half4 _MainTex_ST;

			fixed4 _Color;
			fixed4 _RefColor;
			fixed _EnvironmentPower;
			half _IllumFactor;

			struct v2f
			{
				float4 pos : SV_POSITION;
				half4 uv : TEXCOORD0;
				half4 reflectUV :TEXCOORD1;
				LIGHTFACE_COORDS(2)
				FOG_COORDS(3)
				NORMAL_COORDS(4, 5, 6, 7)
			}; 
		
			
			struct appdata
			{
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
				LIGHTMAP_COORDS(1)
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
			};
			
			float3 reflect(float3 I,float3 N)
            {
                return I - 2.0*N *dot(N,I);
            }
			
			v2f vert (appdata v)
			{
				WORLD_POS
				v2f o;
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pos =  UnityObjectToClipPos(float4(v.vertex.xyz, 1));
				o.reflectUV = ComputeScreenPos(o.pos);

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
				half4 reflectiveColor = tex2D(_RefTex, (i.reflectUV.xy/i.reflectUV.w));

				tex.rgb *= _Color;
				tex.rgb *= _IllumFactor;

				_EnvironmentPower *= (tex.a);
				tex.rgb = tex.rgb + reflectiveColor.rgb * _EnvironmentPower;

				LIGHTMAP_PS
				LIGHTFACE_PS_SHADOW
				SNOW_PS
				FOG_PS
				
				return tex;
			}

		ENDCG
	}	
}
}




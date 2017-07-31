// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Scene/Lightmap Receive Shadow" 
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
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
			#pragma multi_compile NORMAL_ON NORMAL_OFF
			#pragma multi_compile RAIN_OFF RAIN_ON
			#pragma multi_compile SNOW_OFF SNOW_ON
			#pragma multi_compile LOD_FADE_CROSSFADE _
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "LightFace.cginc"
			#include "Weather.cginc"
			
			sampler2D _MainTex;
			half4 _MainTex_ST;
			fixed4 _Color;
			half _IllumFactor;

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
				LOD_DITHER_COORDS(3)
				NORMAL_COORDS(4, 5, 6, 7)

			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex); 

				WORLD_POS
				LIGHTMAP_VS
				LIGHTFACE_VS
				LIGHTFACE_VS_SHADOW
				FOG_VS
				LOD_DITHER_VS
				NORMAL_VS
				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				RAIN_PS_UV_TILE

				float4 tex = tex2D(_MainTex, i.uv.xy);
				tex.rgb *= _Color * _IllumFactor;

				//RAIN_PS_Lighting
				LIGHTMAP_PS
				LIGHTFACE_PS_SHADOW
				SNOW_PS
				FOG_PS
				LOD_DITHER_PS
				
				return tex;
			}

		ENDCG
	}	
}
}




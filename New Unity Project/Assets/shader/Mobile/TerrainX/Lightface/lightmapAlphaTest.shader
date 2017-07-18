Shader "GOE/Scene/LightmapAlphaTest" 
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1
}

SubShader {

	Tags {"Queue"="Geometry" "IgnoreProjector"="True"}
	Pass 
	{
		Tags {  "LightMode" = "ForwardBase" }
		Fog { Mode off } 
		cull off
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers d3d11 flash
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma multi_compile LIGHT_FACE_OFF  LIGHT_FACE_ON 
			#pragma multi_compile SM_3_OFF SM_3_ON
			#pragma multi_compile NORMAL_ON NORMAL_OFF
			#pragma multi_compile SNOW_OFF SNOW_ON
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "LightFace.cginc"
			#include "Weather.cginc"	
			
			sampler2D _MainTex;
			fixed4 _Color;
			fixed _IllumFactor;
			float _Cutoff;

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
				float4	pos : SV_POSITION;
				half4	uv : TEXCOORD0;

				LIGHTFACE_COORDS(1)
				FOG_COORDS(2)
				NORMAL_COORDS(3, 4, 5, 6)
			};

			v2f vert (appdata v)
			{
				WORLD_POS
				v2f o;
				o.uv = v.texcoord;
				
				o.pos =  mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));

				LIGHTMAP_VS
				LIGHTFACE_VS
				FOG_VS
				NORMAL_VS
				return o;
			}
				

			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				clip(tex.a -= _Cutoff);
				tex.rgb *= _Color;	
				tex.rgb *= _IllumFactor;

				LIGHTMAP_PS
				LIGHTFACE_PS
				SNOW_PS_TREE
				FOG_PS

				return tex;
			}

		ENDCG
	}	
}
}




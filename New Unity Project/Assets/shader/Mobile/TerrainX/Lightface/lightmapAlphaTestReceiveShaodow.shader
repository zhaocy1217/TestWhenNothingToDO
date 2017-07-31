// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Scene/LightmapAlphaTest Receive Shadow" 
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
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers d3d11 flash
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "LightFace.cginc"
			
			sampler2D _MainTex;
			half4 _MainTex_ST;
			fixed4 _Color;
			fixed _IllumFactor;
			float _Cutoff;

			struct v2f
			{
				float4	pos : SV_POSITION;
				half4	uv : TEXCOORD0;
				LIGHTFACE_COORDS(1)
					FOG_COORDS(2)
			}; 
		
			struct appdata
			{
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
				LIGHTMAP_COORDS(1)
			};
		

			v2f vert (appdata v)
			{
				WORLD_POS
				v2f o;
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.pos =  UnityObjectToClipPos(float4(v.vertex.xyz, 1));

				LIGHTMAP_VS
				LIGHTFACE_VS
				LIGHTFACE_VS_SHADOW
				FOG_VS

				return o;
			}
				

			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				clip(tex.a -= _Cutoff);
				tex.rgb *= _Color;
				tex.rgb *= _IllumFactor;
	
				LIGHTMAP_PS
				LIGHTFACE_PS_SHADOW
				FOG_PS

				return tex;
			}

		ENDCG
	}	
}

}




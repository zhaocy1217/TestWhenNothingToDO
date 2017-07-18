Shader "GOE/Lightmap Transparent3"
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1
	_Alpha("Alpha", Range(0,1)) = 1
}


SubShader 
{

	Tags {"Queue"="Transparent" "IgnoreProjector"="True" }
	Pass 
	{
		Blend SrcAlpha OneMinusSrcAlpha
		Fog { Mode off }  


		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers d3d11 flash
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "LightFace.cginc"
			
			sampler2D _MainTex;
			fixed4 _Color;
			half _IllumFactor;
			fixed _Alpha;

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
				o.uv = v.texcoord;

				o.pos =  mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));

				LIGHTMAP_VS
				LIGHTFACE_VS
				FOG_VS

				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				tex.rgb *= _Color;
				tex.rgb *= _IllumFactor;

				LIGHTMAP_PS
				LIGHTFACE_PS_SHADOW
				FOG_PS
				LOD_ALPHA_PS
				
				tex.a *= _Alpha;
								
				return tex;
			}

		ENDCG
	}	
}
}




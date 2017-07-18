Shader "GOE/Wing/Additive" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	[HideInInspector]
	_Alpha("Alpha", Range(0,1)) = 1
}

Category {
	Tags { "Queue"="Transparent+100" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	ColorMask RGB
	Cull Off 
	Lighting Off 
	ZWrite Off
	Fog {Mode Off}
            
	
	SubShader 
	{
		Pass 
		{
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "../Lightface/LightFace.cginc"
            #pragma exclude_renderers d3d11 flash
            #pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON
			
            #pragma target 2.0

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _TintColor;
			float _Alpha;

			struct appdata_t 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				LIGHTFACE_COORDS(1)
				FOG_COORDS(2)
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

				WORLD_POS
				LIGHTFACE_VS
				FOG_VS

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 tex = tex2D(_MainTex, i.uv) * _TintColor * 2.0f;
				
#ifdef LIGHT_FACE_ON
				LIGHTFACE_PS_ROLE
#endif
				FOG_PS

				tex.rgb *= _Alpha;
				return tex;
			}
			ENDCG 
		}
	}	
}
}

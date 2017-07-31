// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Wing/Fire Wing" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Main Texture", 2D) = "white" {}
	_AnimTex ("Anim Texture", 2D) = "white" {}
	_SizeX ("SizeX", Float) = 4
    _SizeY ("SizeY", Float) = 4
    _Speed ("Speed", Float) = 200
	[HideInInspector]
	_Alpha("Alpha", Range(0,1)) = 1
}

Category {
	Tags { "Queue"="Transparent+100" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off 
	Lighting Off 
	ZWrite Off 
	Fog {Mode Off}
	
	SubShader {
		Pass {
		
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "../Lightface/LightFace.cginc"
            #pragma exclude_renderers d3d11 flash
            #pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 

            #pragma target 2.0

			sampler2D _MainTex;
			sampler2D _AnimTex;
			fixed4 _TintColor;
			fixed _SizeX;
			fixed _SizeY;
			fixed _Speed;
			float _Alpha;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 _uv0 : TEXCOORD0;
				float2 _uv1 : TEXCOORD1;
				float2 uv_ref_lightface	: TEXCOORD2;
				float4  depth : TEXCOORD3;
			};
			
			float4 _MainTex_ST;
			float4 _AnimTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o._uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
				o._uv1 = TRANSFORM_TEX(v.texcoord,_AnimTex);


				WORLD_POS
#ifdef LIGHT_FACE_ON	
				o.uv_ref_lightface.xy = LightFaceUV(worldPos).xy;
#else
				o.uv_ref_lightface.xy = half2(0,0);			
#endif
				o.depth = SimulateFogVS(o.vertex.xyz, worldPos.xyz);

				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 mainColor = tex2D(_MainTex, i._uv0);

				int indexX=fmod (_Time.x*_Speed,_SizeX);
		 		int indexY=fmod ((_Time.x*_Speed)/_SizeX,_SizeY);

				fixed2 seqUV = float2((i._uv1.x) /_SizeX, (i._uv1.y)/_SizeY);
				seqUV.x += indexX/_SizeX;
				seqUV.y -= indexY/_SizeY;
				fixed4 seqColor = tex2D(_AnimTex, seqUV);
				
				fixed4 finalColor = fixed4(mainColor.rgb * mainColor.a + seqColor.rgb, (mainColor.a + seqColor.a) * i.color.r);

#ifdef LIGHT_FACE_ON	
				LightFaceColorRole(finalColor.rgba, i.uv_ref_lightface.xy, fixed3(1,1,1), 1);
#endif
				finalColor.rgb = SimulateFog(i.depth, finalColor, 1);
				finalColor.a *= _Alpha;

				return finalColor;
			}
			ENDCG 
		}
	}	
}
}

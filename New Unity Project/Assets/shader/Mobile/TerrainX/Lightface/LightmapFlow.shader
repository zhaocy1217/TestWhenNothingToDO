Shader "GOE/Scene/LightmapFlow" 
{

Properties {
	_MainTex ("Base", 2D) = "white" {}
//	_BumpMap("Normal", 2D) = "bump" {}
	_Noise ("Noise", 2D) = "bump" {}
	_RefColor ("Ref Color", Color) = (1,1,1,1)
	_ReflectionTex("_ReflectionTex", 2D) = "black" {}
	_ReflectionPower("_ReflectionTex Power", Range(0, 2)) = 1
	_DirectionUv("Wet scroll direction (2 samples)", Vector) = (1.0,1.0, -0.2,-0.2)
	_TexAtlasTiling("Tex atlas tiling", Vector) = (8.0,8.0, 4.0,4.0)
	_Speed ("Speed", Range (0, 10)) = 1
}

		




SubShader {
	Tags { "Queue" = "Geometry" "IgnoreProjector" = "True" }
	Pass 
	{
		Tags{ "LightMode" = "ForwardBase" }
		cull off
		
		CGPROGRAM


		#pragma vertex vert
		#pragma fragment frag
		//#pragma multi_compile_fwdbase
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma exclude_renderers d3d11 flash
		#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
		#include "UnityCG.cginc"		
		#include "Lighting.cginc"
		#include "LightFace.cginc"

		struct v2f_full
		{
			half4 pos : SV_POSITION;
			fixed4 color : COLOR;
			half4 uv : TEXCOORD0;
			half4 noiseScrollUv : TEXCOORD1;
			float4  depth : TEXCOORD2;//depth:x  nor.y:y(用于光照计算)
	#ifdef LIGHTMAP_ON
			half2   uv2 : TEXCOORD3;
	#endif

		};


		half4 _DirectionUv;
		half4 _TexAtlasTiling;
		half4 _ReflectionTexTiling;
		fixed4 _RefColor;
		half _ReflectionPower;
		half _Speed;
		sampler2D _MainTex;
		sampler2D _Noise;
		sampler2D _ReflectionTex;
		sampler2D _ReflectionTex2;





		half4 _MainTex_ST;
		half4 _BumpMap_ST;

//		float4 unity_LightmapST;	
//		sampler2D unity_Lightmap;
		float4 _ReflectionTex2_ST;
		v2f_full vert (appdata_full v) 
		{
			WORLD_POS
			v2f_full o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv.xy = TRANSFORM_TEX(v.texcoord ,_MainTex);
			o.color = v.color;
#ifdef LIGHT_FACE_ON
			o.uv.zw = LightFaceUV(worldPos);
#else
			o.uv.zw = half2(0, 0);
#endif

			o.noiseScrollUv.xyzw = v.texcoord.xyxy;
#ifdef LIGHTMAP_ON
			o.uv2 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif			
			o.depth = SimulateFogVS(o.pos.xyz, worldPos.xyz);
			return o; 
		}
		
		fixed4 frag (v2f_full i) : COLOR0 
		{
			float2 uv = i.noiseScrollUv.xy;
			i.noiseScrollUv.xyzw = i.noiseScrollUv.xyzw *_TexAtlasTiling + _Time.xxxx * _Speed * _DirectionUv;
			fixed4 tex = tex2D (_MainTex, i.uv.xy);
				
			
			half3 nrml = tex2D(_Noise, i.noiseScrollUv.xy);
			nrml += tex2D(_Noise, i.noiseScrollUv.zw);
			
			nrml.xy *= 0.25;
										
			fixed4 rtRefl = tex2D (_ReflectionTex,  nrml.xy);			
			rtRefl *= _RefColor; 
		
			tex.xyz  += (rtRefl.xyz * _ReflectionPower * i.color.a);
			
#ifdef LIGHTMAP_ON
			fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2));
#else
			fixed3 lm = fixed3(1, 1, 1);
#endif

			LightFaceColor(tex.xyzw, i.uv.zw, lm,  1);
			
			tex.xyz =  SimulateFog(i.depth, tex, 1);

			return tex;	
		}
		
		ENDCG
	}
} 

//FallBack "Transparent/IceFire/CullOut/Wing"

}

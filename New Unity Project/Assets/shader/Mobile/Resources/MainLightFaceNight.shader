// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Main LightFace Night" 
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_NightLightScale ("scale", Range(0,1)) = 1
}

	SubShader 
	{

	Tags {"Queue"="Transparent+150" "IgnoreProjector"="True"}
		//Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" }
	LOD 200
		

		Pass
		{
			cull off
			//Blend  srcalpha  srccolor//oneminussrcalpha
			blend dstcolor zero
			ColorMask RGB
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			fixed _NightLightScale;
			struct v2f
			{
				float4	pos : SV_POSITION;
				half2	uv : TEXCOORD0;
			};


			v2f vert(appdata_full v)
			{
				v2f o;
				o.uv = v.texcoord.xy;
				o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
				//o.pos = float4(v.vertex.xyz, 1);//
				//o.pos.z = 0.5f;
				return o;
			}


			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				//fixed3 no = fixed3(0.5, 0.5, 0.5);
				//tex.rgb = tex.rgb*_NightLightScale + (1-_NightLightScale)*no;
				//tex.rgb = tex.rgb*_NightLightScale + (1 - _NightLightScale)*_Color.rgb;

				//tex.rgb *= _Color.rgb;
				//tex.a *= 10;
				tex.rgb = 0.8f;
				return tex;
			}

			ENDCG
		}




		Pass 
		{
			cull off
			Blend  srcalpha  oneminussrcalpha
			//blend one zero
			//blend dstcolor zero//oneminussrcalpha
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				//#pragma multi_compile_fwdbase
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#include "UnityCG.cginc"
			
				sampler2D _MainTex;
				fixed4 _Color;
				fixed _NightLightScale;
				struct v2f
				{
					float4	pos : SV_POSITION;
					half2	uv : TEXCOORD0;
				}; 
		

				v2f vert (appdata_full v)
				{
					v2f o;
					o.uv = v.texcoord.xy;
					o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
					//o.pos = float4(v.vertex.xyz, 1);//
					//o.pos.z = 0.5f;
					return o;
				}
				

				float4 frag(v2f i) : COLOR
				{
					fixed4 tex = tex2D(_MainTex, i.uv);
					//fixed3 no = fixed3(0.5, 0.5, 0.5);
					//tex.rgb = tex.rgb*_NightLightScale + (1-_NightLightScale)*no;
					//tex.rgb = tex.rgb*_NightLightScale + (1 - _NightLightScale)*_Color.rgb;
			
					//tex.rgb *= _Color.rgb;
					//tex.a *= 10;
					tex.a = tex.r + 0.2;
					tex.rgb = _Color.rgb;
					
					return tex;
				}

			ENDCG
		}	



		//Pass
		//{
		//	cull off
		//	//Blend  zero one
		//	//blend one zero
		//	//blend dstcolor srcalpha
		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag
		//	#pragma multi_compile_fwdbase
		//	#pragma fragmentoption ARB_precision_hint_fastest

		//	#include "UnityCG.cginc"

		//	sampler2D _MainTex;
		//	sampler2D _MaskTex;
		//	fixed4 _Color;
		//	fixed _NightLightScale;
		//	struct v2f
		//	{
		//		float4	pos : SV_POSITION;
		//		half2	uv : TEXCOORD0;
		//	};


		//	v2f vert(appdata_full v)
		//	{
		//		v2f o;
		//		o.uv = v.texcoord.xy;
		//		o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
		//		//o.pos = float4(v.vertex.xyz, 1);//
		//		//o.pos.z = 0.5f;
		//		return o;
		//	}


		//	float4 frag(v2f i) : COLOR
		//	{
		//		fixed4 tex = tex2D(_MainTex, i.uv);
		//		fixed4 mask = tex2D(_MaskTex, i.uv);
		//		//fixed3 no = fixed3(0.5, 0.5, 0.5);
		//		//tex.rgb = tex.rgb*_NightLightScale + (1-_NightLightScale)*no;
		//		//tex.rgb = tex.rgb*_NightLightScale + (1 - _NightLightScale)*_Color.rgb;
		//		//tex.a *= tex.r;
		//		tex.rgb = _Color * (tex.r + 0.8);
		//		//tex.rgb = tex.rgb*_NightLightScale + (1 - _NightLightScale)*_Color.rgb;
		//		tex.a = mask.a;
		//	    return tex;
		//	}

		//		ENDCG
		//}

}
}




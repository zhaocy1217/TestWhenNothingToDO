
Shader "GOE/Grab/Glass" 
{
Properties {
	//_DistortAmt  ("Distortion", range (0,128)) = 10
	//_DistortMap ("DistortMap", 2D) = "white" {}

	_Color("Main Color", Color) = (1,1,1,1)
	_MainTex ("Tint Color (RGB)", 2D) = "white" {}
	_EnvironmentPower ("Environment Factor", Range(0,5)) = 0
	_Alpha ("alpha", Range(0,1)) = 0.5

}

	
SubShader 
{
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" }
	Blend  srcalpha oneminussrcalpha
	Pass 
	{
		Tags { "LightMode" = "ForwardBase" }
		Fog { Mode off }  
		ZWrite on
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		//#pragma multi_compile_fwdbase
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma exclude_renderers d3d11 flash

		#include "UnityCG.cginc"

		struct appdata_t 
		{
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f 
		{
			float4 vertex : POSITION;
			float4 uvgrab : TEXCOORD0;
			//float2 uvbump : TEXCOORD1;
		};

		fixed4 _Color;
		//sampler2D _grabLastTexture;
		//float4 _grabLastTexture_TexelSize;
		//sampler2D _DistortMap;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		//uniform float _DistortAmt;
		half _EnvironmentPower;
		fixed _Alpha;

		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			//o.vertex.xyzw /= o.vertex.w;
			o.uvgrab.xyzw = ComputeScreenPos(o.vertex);
			o.uvgrab.xyzw /= o.uvgrab.w ;
			//o.uvbump = v.texcoord;
			return o;
		}

		half4 frag( v2f i ) : COLOR
		{
			//half2 bump =(tex2D( _DistortMap, i.uvbump )).rg;
			//float2 offset = bump * _DistortAmt * _grabLastTexture_TexelSize.xy;
			fixed2 origin = i.uvgrab.xy;
			origin = TRANSFORM_TEX(origin, _MainTex);
			//i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
			
			//half4 col = tex2D( _grabLastTexture, origin );
			half4 reflectiveColor = tex2D( _MainTex, origin );
			//col.rgb = col.rgb*(1-_EnvironmentPower) + reflectiveColor.rgb * _EnvironmentPower;
			reflectiveColor.rgb *= _EnvironmentPower;
			reflectiveColor.rgb *= _Color;
			reflectiveColor.a = _Alpha;
			return reflectiveColor;
		}


		ENDCG
	}
}

	

}
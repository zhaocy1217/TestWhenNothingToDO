Shader "GOE/Other/FakeShadow" 
{
Properties {
	_MainTex ("Base", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.1
}

SubShader {
		LOD 200
	Pass {
		cull off
		ZTest Always 

		//Blend  one zero
		ColorMask A
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "unitycg.cginc"
		#include "UnityLightingCommon.cginc"
		sampler2D _MainTex;
		fixed4 _Color;
		
		struct v2f 
		{
			half4 pos : SV_POSITION;
		};	
		v2f vert (appdata_full v) 
		{
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			return o; 
		}		
		
		fixed4 frag (v2f i) : COLOR
		{	
			fixed4 c = 0.5;
			return c;		
		}	

		ENDCG
	}
} 
}
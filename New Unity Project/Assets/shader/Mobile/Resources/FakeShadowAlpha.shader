Shader "GOE/Other/FakeShadowAlpha" 
{
Properties {
	_MainTex ("Base", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.1
}

SubShader {

	Pass {

		ZTest Always 

		Blend  DstAlpha zero
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
			half4 texcoord : TEXCOORD0;
		};	
		v2f vert (appdata_full v) 
		{
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.texcoord = v.texcoord;
			return o; 
		}		
		
		fixed4 frag (v2f i) : COLOR
		{	
			fixed4 c = tex2D(_MainTex, i.texcoord);
		//c.rgb = c.rgb * c.a;
		//c.a = (c.r + c.g + c.b)/3;
			//c.a = 1 - c.a * 0.5;
		//c.a *= 1.3;
			return c;		
		}	

		ENDCG
	}
} 
}
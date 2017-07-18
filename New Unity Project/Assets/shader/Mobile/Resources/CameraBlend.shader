
Shader "GOE/CameraBlend" 
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BGTexture("Bloom (RGB)", 2D) = "black" {}
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _BGTexture;
		//float4x4 _Rotation;
		float4 uvOffset;
	

		struct v2f_simple 
		{
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		v2f_simple vertBlend (appdata_img v)
		{
			v2f_simple o;
			
			o.pos = mul (UNITY_MATRIX_MVP,   v.vertex);
			
        	o.uv = v.texcoord;		
        	    	        	
			return o; 
		}
		

						
		fixed4 fragBlend ( v2f_simple i ) : COLOR
		{	
			//fixed4 tex = tex2D(_MainTex, i.uv);
			float2 uv;
			uv.x = lerp(uvOffset.x, uvOffset.y, i.uv.x);
			uv.y = lerp(uvOffset.z, uvOffset.w, i.uv.y);
			fixed4 bg = tex2D(_BGTexture, uv);
			//fixed4 c = bg * (1- tex.a) + tex.rgba * tex.a;
			//c.a = 1;
 			return bg;
		} 
			
	ENDCG
	
	SubShader {
	  ZTest Off Cull Off ZWrite Off Blend Off
	  Fog { Mode off }  
	  
	// 0
	Pass {
	
		CGPROGRAM
		#pragma vertex vertBlend
		#pragma fragment fragBlend
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}		
	}
	FallBack Off
}

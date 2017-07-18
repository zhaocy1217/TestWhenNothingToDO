
Shader "GOE/Depth" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	CGINCLUDE
		#include "UnityCG.cginc"


		sampler2D _MainTex;
		float _DofNear;
		float _DofFar;
		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f_simple 
		{
			half4 pos : SV_POSITION;
			float4 normal_depth : TEXCOORD0;
			//half2 depth : TEXCOORD0;
		};
				
		v2f_simple vertDepth (appdata v)
		{
			v2f_simple o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
        	o.normal_depth.xyz = UnityObjectToWorldNormal(v.normal.xyz);
			o.normal_depth.xyz = normalize(o.normal_depth.xyz);
        	o.normal_depth.w = o.pos.z/o.pos.w;
        	        	
			return o; 
		}

							
		fixed4 fragDepth ( v2f_simple i ) : COLOR
		{	
			fixed4 c = fixed4(0,0,0,0);
			c.rgb = i.normal_depth.xyz;
			c.a = i.normal_depth.w*0.5/_DofFar + max(0,_DofNear - i.normal_depth.w*1.5);
			return c;
		} 

			
	ENDCG
	
	SubShader 
	{
	  Fog { Mode off }  
	  
	// 0
	Pass {
	
		CGPROGRAM
		#pragma vertex vertDepth
		#pragma fragment fragDepth
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}
	// 1
	/*Pass { 
	
		CGPROGRAM
		
		#pragma vertex vertMax
		#pragma fragment fragMax
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}	
	// 2
	Pass {
	
		CGPROGRAM
		
		#pragma vertex vertBlur
		#pragma fragment fragBlur
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}	*/		
	}
	FallBack Off
}

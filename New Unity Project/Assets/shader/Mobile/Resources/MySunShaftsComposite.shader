Shader "Hidden/MySunShaftsComposite" {
	Properties {
		_MainTex ("Base", 2D) = "" {}
		_ColorBuffer ("Color", 2D) = "" {}
	}
	
	CGINCLUDE
				
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		#if SHADER_API_D3D9
		float2 uv1 : TEXCOORD1;
		#endif		
	};
		
	struct v2f_radial {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float2 blurVector : TEXCOORD1;
	};
		
	sampler2D _MainTex;
	sampler2D _ColorBuffer;
	//sampler2D _CameraDepthTexture;
	sampler2D _depthNormalTexture;
	
	uniform half _NoSkyBoxMask;
		
	uniform half4 _SunColor;
	uniform half4 _BlurRadius4;
	uniform half4 _SunPosition;
	uniform half4 _MainTex_TexelSize;	

	#define SAMPLES_FLOAT 3.0f
	#define SAMPLES_INT 2
			
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		
		#if SHADER_API_D3D9
		o.uv1 = v.texcoord.xy;
		//if (_MainTex_TexelSize.y < 0)
			//o.uv1.y = 1-o.uv1.y;
		#endif				
		
		return o;
	}
		
	half4 fragScreen(v2f i) : COLOR { 
		half4 colorA = tex2D (_MainTex, i.uv.xy);
		#if SHADER_API_D3D9
		half4 colorB = tex2D (_ColorBuffer, i.uv1.xy);
		#else
		half4 colorB = tex2D (_ColorBuffer, i.uv.xy);
		#endif
		half4 depthMask = saturate (colorB * _SunColor);
		//return depthMask;	
		return 1.0f - (1.0f-colorA) * (1.0f-depthMask);	
	}

	
	v2f_radial vert_radial( appdata_img v ) {
		v2f_radial o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		
		o.uv.xy =  v.texcoord.xy;
		o.blurVector = (_SunPosition.xy - v.texcoord.xy) * _BlurRadius4.xy * 1.5;	
		
		return o; 
	}
	
	half4 frag_radial(v2f_radial i) : COLOR 
	{	
		half4 color = half4(0,0,0,0);
		
		half4 tmpColor = tex2D(_MainTex, i.uv.xy);
		color += tmpColor;
		i.uv.xy += i.blurVector; 
		tmpColor = tex2D(_MainTex, i.uv.xy);
		color += tmpColor;
		i.uv.xy += i.blurVector; 
		tmpColor = tex2D(_MainTex, i.uv.xy);
		color += tmpColor;
		i.uv.xy += i.blurVector; 
			
			
		//for(int j = 0; j < SAMPLES_INT; j++)   
		//{	
			//half4 tmpColor = tex2D(_MainTex, i.uv.xy);
			//color += tmpColor;
			//i.uv.xy += i.blurVector; 	
		//}
		return color / SAMPLES_FLOAT;
	}	
	
	half TransformColor (half4 skyboxValue) {
		return max (skyboxValue.a, _NoSkyBoxMask * dot (skyboxValue.rgb, float3 (0.59,0.3,0.11))); 		
	}
	
	half4 frag_depth (v2f i) : COLOR {
		#if SHADER_API_D3D9
		//float depthSample = UNITY_SAMPLE_DEPTH(tex2D (_CameraDepthTexture, i.uv1.xy));
		float depthSample = tex2D (_depthNormalTexture, i.uv1.xy).a;
		#else
		//float depthSample = UNITY_SAMPLE_DEPTH(tex2D (_CameraDepthTexture, i.uv.xy));
		float depthSample = tex2D (_depthNormalTexture, i.uv.xy).a;
		#endif
		
		half4 tex = tex2D (_MainTex, i.uv.xy);
		
		//depthSample = Linear01Depth (depthSample);
		 depthSample = depthSample * 0.5 + 0.5;
		 
		// consider maximum radius
		#if SHADER_API_D3D9
		half2 vec = _SunPosition.xy - i.uv1.xy;
		#else
		half2 vec = _SunPosition.xy - i.uv.xy;		
		#endif
		half dist = saturate (_SunPosition.w - length (vec.xy));		
		
		half4 outColor = 0;
		
		// consider shafts blockers
		//if (depthSample > 0.99)
		half temp = sign(depthSample - 0.99);
			outColor = TransformColor (tex) * dist * temp;
			
			
		return outColor;
	}
	
	

	

	ENDCG
	
Subshader {
  
 Pass {
 	  Blend Off
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest 
      #pragma vertex vert
      #pragma fragment fragScreen
      
      ENDCG
  }
  
 Pass {
	  Blend One Zero
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert_radial
      #pragma fragment frag_radial
      
      ENDCG
  }
  
  Pass {
 	  Blend Off  	
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest      
      #pragma vertex vert
      #pragma fragment frag_depth
      
      ENDCG
  }
  
}

Fallback off
	
} // shader
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/WaterTransition"
{
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
        _MainTex ("image", 2D) = "white" {}
        _NoiseTex("noise", 2D) = "bump" {}
		_NoiseFactor("Noise Param", Range(-1, 1)) = 0
    }  
      
    CGINCLUDE  
        #include "UnityCG.cginc"             
        
        sampler2D _MainTex;
        sampler2D _NoiseTex;
		fixed _NoiseFactor;
		fixed4 _Color;
        
        struct v2f {
            half4 pos:SV_POSITION;
            half4 uv : TEXCOORD0;
        };
    
        v2f vert(appdata_full v) {
            v2f o;
            o.pos = UnityObjectToClipPos (v.vertex);
            o.uv.xy = v.texcoord.xy;
            o.uv.z = v.texcoord.x - _Time.x * 4 ;
			o.uv.w = v.texcoord.y - _Time.x;
            return o;    
        }    
    
        fixed4 frag(v2f i) : COLOR0 {  
			float2 uv2 = i.uv.xy;
#if UNITY_UV_STARTS_AT_TOP
			//uv2.y = 1 - i.uv.y;
#endif
            half3 noise = tex2D(_NoiseTex, i.uv.zw );
			noise -= 0.45;
            fixed4 tex1 = tex2D(_MainTex, uv2 + noise.xy * _NoiseFactor);
			//tex1.rgb = noise;
			//tex1.rgb = fixed3(1, 0, 0);
			return tex1;
        }    
    ENDCG      
    
    SubShader {     
        Tags {"Queue" = "Transparent"}       
        ZWrite Off    
		blend one zero   
        //Blend SrcAlpha OneMinusSrcAlpha       
        Pass {      
            CGPROGRAM      
            #pragma vertex vert      
            #pragma fragment frag      
            #pragma fragmentoption ARB_precision_hint_fastest       
    
            ENDCG      
        }  
    }  
    FallBack Off 
}


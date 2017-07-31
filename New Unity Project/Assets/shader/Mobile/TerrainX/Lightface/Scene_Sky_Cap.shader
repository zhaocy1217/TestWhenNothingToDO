// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Scene/Sky_Cap" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ScrollX ("Base layer Scroll speed X", Float) = 1.0
        _ScrollY ("Base layer Scroll speed Y", Float) = 0.0
        _ChangeTex ("Base (RGB)", 2D) = "white" {}
        _blendFactor("blend factor",range(0,1)) = 0
    }
    SubShader {
        Tags { "Queue" = "Geometry+1" "RenderType"="Opaque" "Ignoreprojector" = "True"}
        LOD 200
        lighting off
        fog{mode off}
        pass{
        CGPROGRAM
        #pragma multi_compile CHANGE_TEX_OFF CHANGE_TEX_ON
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
			#include "LightFace.cginc"
        
        sampler2D _MainTex;
        float4 _MainTex_ST;
        float _ScrollX;
        float _ScrollY;
        
        sampler2D _ChangeTex;
        fixed _blendFactor;
        float _globalDarkFactor;
        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
			FOG_COORDS(2)
        };
        
        v2f vert (appdata_full v) 
        {
			WORLD_POS
            v2f o;
            o.pos = UnityObjectToClipPos (v.vertex);
            o.uv = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);

			FOG_VS
            return o;
        }
        
        
        fixed4 frag(v2f i) : color 
        {
            fixed4 c = tex2D(_MainTex, i.uv);
            #ifdef CHANGE_TEX_ON
            fixed4 c2 = tex2D(_ChangeTex, i.uv);
            c = lerp(c,c2,_blendFactor);
            #endif
            
            c.rgb = lerp(c.rgb,fixed3(0,0,0),_globalDarkFactor);

			fixed4 tex = c;
			FOG_PS

            return fixed4(tex.rgb,0);
        }
        
        ENDCG
        
        }
    } 
    FallBack "Diffuse"
}


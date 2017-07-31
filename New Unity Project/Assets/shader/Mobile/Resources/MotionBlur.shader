// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/ImageEffect/MotionBlur" {
    Properties {
        _MainTex ("main tex", 2D) = "" {}
        _LastTex ("mask tex", 2D) = "" {}
        _MaskTex ("mask tex", 2D) = "" {}
    }

    SubShader {
        Pass {
            ZTest Always
CGPROGRAM

#pragma vertex vert
#pragma fragment frag
            uniform float _Alpha;
            uniform sampler2D _MainTex;
            uniform sampler2D _LastTex;
            uniform sampler2D _MaskTex;
            struct appdata 
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };
            struct v2f 
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }

            half4 frag(v2f i) : COLOR {
                fixed4 c;
                fixed4 base = tex2D(_MainTex, i.uv);
                fixed4 last = tex2D(_LastTex, i.uv);
   
                fixed4 mask = tex2D(_MaskTex, i.uv);
				
				c.rgb  = base.rgb * 0.1 + last.rgb * 0.9;

                c.rgb = base.rgb* (mask.a) + last.rgb*(1-mask.a);
                c.a = 1;
                return c;
            }
ENDCG
        }
    }
}

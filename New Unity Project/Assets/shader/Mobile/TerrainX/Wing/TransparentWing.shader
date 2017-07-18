Shader "GOE/Wing/Transparent Wing" {
    Properties {
    	_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _RefTex ("RefTex", 2D) = "white" {}
		[HideInInspector]
		_Alpha("Alpha", Range(0,1)) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent+100"
            "RenderType"="Transparent"
        }
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Fog {Mode Off}
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "../Lightface/LightFace.cginc"
            #pragma exclude_renderers d3d11 flash
            #pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 

            #pragma target 2.0

            uniform float4 _Color;
            uniform sampler2D _RefTex; uniform float4 _RefTex_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			float _Alpha;

            struct VertexInput 
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput 
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv_ref_lightface	: TEXCOORD1;
				float4  depth : TEXCOORD2;
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );

				WORLD_POS
#ifdef LIGHT_FACE_ON	
				o.uv_ref_lightface.xy = LightFaceUV(worldPos).xy;
#else
				o.uv_ref_lightface.xy = half2(0,0);			
#endif
				o.depth = SimulateFogVS(o.pos.xyz, worldPos.xyz);

                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                fixed4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                fixed4 _RefTex_var = tex2D(_RefTex,TRANSFORM_TEX(i.uv0+_Time.y*float2(1,1), _RefTex));
                fixed3 emissive = (_MainTex_var.rgb+(_MainTex_var.a*_RefTex_var.rgb));

                fixed4 finalColor = fixed4(emissive *_Color.rgb, _MainTex_var.a*_Color.a);

#ifdef LIGHT_FACE_ON
				LightFaceColorRole(finalColor.rgba, i.uv_ref_lightface.xy, fixed3(1,1,1), 1);
#endif
				finalColor.rgb = SimulateFog(i.depth, finalColor, 1);
				finalColor.a *= _Alpha;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

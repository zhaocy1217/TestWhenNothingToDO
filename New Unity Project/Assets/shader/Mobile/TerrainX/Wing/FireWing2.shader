Shader "GOE/Wing/FireWing2" {
    Properties {
    	_Color("Color", Color) = (1, 0.4, 0, 1)
        _NoiseTex1 ("NoiseTex1", 2D) = "white" {}
        _NoiseTex2 ("NoiseTex2", 2D) = "white" {}
        _MainTex ("MainTex", 2D) = "white" {}
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

            #pragma target 2.0

            uniform sampler2D _MainTex; 
			uniform float4 _MainTex_ST;
			float _Alpha;

            struct VertexInput 
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            struct VertexOutput 
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float2 uv_ref_lightface	: TEXCOORD1;
				float4  depth : TEXCOORD2;
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }

            float4 frag(VertexOutput i, float facing : VFACE) : COLOR 
            {
                fixed4 tex = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                return fixed4(tex.rgb, tex.a * i.vertexColor.r * _Alpha);
            }
            ENDCG
        }

        Pass {
          	Blend SrcAlpha OneMinusSrcAlpha
            ZWrite on
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

            uniform sampler2D _NoiseTex1; 
			uniform float4 _NoiseTex1_ST;
            uniform sampler2D _NoiseTex2; 
			uniform float4 _NoiseTex2_ST;
            uniform sampler2D _MainTex; 
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			float _Alpha;

            struct VertexInput 
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            struct VertexOutput 
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
				LIGHTFACE_COORDS(1)
				FOG_COORDS(2)
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );

				WORLD_POS
				LIGHTFACE_VS
				FOG_VS

                return o;
            }

            float4 frag(VertexOutput i, float facing : VFACE) : COLOR 
            {
                fixed4 tex = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));

                fixed4 _NoiseTex1_var = tex2D(_NoiseTex1,TRANSFORM_TEX(((1.0*i.uv0)+_Time.g*float2(-0.1,0)), _NoiseTex1));
                fixed4 _NoiseTex2_var = tex2D(_NoiseTex2,TRANSFORM_TEX(((0.8*i.uv0)+_Time.g*float2(-0.2,-0.2)), _NoiseTex2));

                fixed3 fireColor = _NoiseTex1_var.rgb * _NoiseTex2_var.rgb * i.vertexColor.r * _Color.xyz * 5;
                fixed3 emissive = tex.rgb + fireColor;
				tex = fixed4(emissive, (tex.a+((tex.a + fireColor.r) * emissive.r)));

#ifdef LIGHT_FACE_ON
				LIGHTFACE_PS_ROLE
#endif
				FOG_PS

				tex.a *= _Alpha;

                return tex;
            }
            ENDCG
        }
    }
}

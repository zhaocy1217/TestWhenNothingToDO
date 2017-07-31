// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Wing/SmokeWing" 
{
    Properties {
        _Color ("Color", Color) = (0.3676471,0.3676471,0.3676471,1)
        _Tex1 ("Tex1", 2D) = "white" {}
        _Tex2 ("Tex2", 2D) = "white" {}
        _UVSpeed1("UV speed1", Vector) = (2,1,-0.1,-0.05)
        _UVSpeed2("UV speed2", Vector) = (2,1,-0.2,0)
        _MainTex ("Main texture", 2D) = "white" {}
		[HideInInspector]
        _Alpha("Alpha", Range(0,1)) = 1
    }
    SubShader 
    {
        Tags 
        {
            "IgnoreProjector"="True"
            "Queue"="Transparent+100"
            "RenderType"="Transparent"
        }

        Pass 
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform float4 _Color;
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
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }

            fixed4 frag(VertexOutput i, float facing : VFACE) : COLOR 
            {
                fixed4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                return fixed4(_MainTex_var.rgb, _MainTex_var.a * i.vertexColor.r * _Alpha);
            }
            ENDCG
        }

        Pass 
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            //Blend SrcAlpha One
            ZWrite on
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "../Lightface/LightFace.cginc"
            #pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON

            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform float4 _Color;
            uniform sampler2D _Tex1; 
            uniform float4 _Tex1_ST;
            uniform sampler2D _Tex2; 
            uniform float4 _Tex2_ST;
            uniform float4 _UVSpeed1;
            uniform float4 _UVSpeed2;
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
				LIGHTFACE_COORDS(1)
				FOG_COORDS(2)
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );

				WORLD_POS
				LIGHTFACE_VS
				FOG_VS

                return o;
            }

            fixed4 frag(VertexOutput i, float facing : VFACE) : COLOR 
            {
                fixed4 tex = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                fixed2 uv1 = ((i.uv0*_UVSpeed1.xy)+_Time.g*_UVSpeed1.zw);
                fixed4 _Tex1_var = tex2D(_Tex1,TRANSFORM_TEX(uv1, _Tex1));
                fixed2 uv2 = ((i.uv0*_UVSpeed2.xy)+_Time.g*_UVSpeed2.zw);
                fixed4 _Tex2_var = tex2D(_Tex2,TRANSFORM_TEX(uv2, _Tex2));
                fixed3 col = (_Tex1_var.rgb * _Tex2_var.rgb);
                tex = fixed4((tex.rgb*tex.a)+(_Color.rgb*col), (tex.a + col.r)*i.vertexColor.r);

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
    FallBack "Diffuse"
}

Shader "GOE/Wing/Transparent Wing2" 
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
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

            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
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

        Pass 
        {
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

            uniform fixed4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			float _Alpha;

            struct VertexInput 
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
				LIGHTFACE_COORDS(1)
				FOG_COORDS(2)
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );

				WORLD_POS
				LIGHTFACE_VS
				FOG_VS

                return o;
            }

            float4 frag(VertexOutput i, float facing : VFACE) : COLOR 
            {
                fixed4 tex = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                fixed3 emissive = (tex.rgb*_Color.rgb);

                float alpha = pow(tex.a, 1) * _Color.a;
				alpha += alpha * sin(_Time.z * 2) * 0.2;
                tex = fixed4(emissive, alpha);

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

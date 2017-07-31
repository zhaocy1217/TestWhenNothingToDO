// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Wing/UVFlowWing" 
{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main texture", 2D) = "white" {}
        _FlowTex ("Flow Tex", 2D) = "white" {}
        _USpeed("U speed", Range(-2, 2)) = -1
        _VSpeed("V speed", Range(-2, 2)) = 0
		[HideInInspector]
		_Alpha("Alpha", Range(0,1)) = 1
    }

    SubShader 
    {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent+100"
            "RenderType"="Transparent"
        }

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite on
		Cull Off
		Fog {Mode Off}

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "../Lightface/LightFace.cginc"
            #pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 

            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0

            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _FlowTex; uniform float4 _FlowTex_ST;
            float _Alpha;
            uniform float _USpeed;
            uniform float _VSpeed;

            struct VertexInput
             {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput 
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
				LIGHTFACE_COORDS(1)
				FOG_COORDS(2)
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );

				WORLD_POS
				LIGHTFACE_VS
				FOG_VS

                return o;
            }

            float4 frag(VertexOutput i) : COLOR 
            {
                fixed4 tex = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                fixed2 uv = (i.uv0 + _Time.g * float2(_USpeed,_VSpeed));
                fixed4 _FlowTex_var = tex2D(_FlowTex,TRANSFORM_TEX(uv, _FlowTex));
                fixed3 emissive = (_Color.rgb*(tex.rgb+_FlowTex_var.rgb));
				tex = fixed4(emissive, 1);

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

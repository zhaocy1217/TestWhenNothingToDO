// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Blood" 
{
    Properties 
    {
        _Color1 ("Color1", Color) = (0,1,0,1)
        _Color2 ("Color2", Color) = (1,0,0,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _BeingTime("Begin time", float) = 9999
        _Speed("UV Speed", Range(0,1)) = 0.1
        _FadeIn("Fade in", Range(0.01,1)) = 0.1
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
		cull off
		Lighting Off

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 

            uniform float4 _Color1;
            uniform float4 _Color2;
            uniform sampler2D _MainTex; 
			uniform float4 _MainTex_ST;
            float _BeingTime;
            fixed _FadeIn;
            fixed _Speed;

            struct VertexInput 
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput 
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR 
            {
////// Lighting:
////// Emissive:
				float4 _MainTex_var0 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                fixed2 uv = (i.uv0+ float2(_Time.g * -1 *_Speed, 0));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(uv, _MainTex));

                fixed uvX = (_Time.y-_BeingTime -_MainTex_var.g) * _Speed;
                fixed fadeMulti = min(uvX - i.uv0.x, _FadeIn) /  _FadeIn;
                fixed3 emissive = (_MainTex_var.rgb + _MainTex_var0) * lerp(_Color1, _Color2, fadeMulti * step(_BeingTime, _Time.y) * step(i.uv0.x, uvX));
                fixed3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
}

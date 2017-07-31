// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "CustomUI/disappear"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
 
		_ColorMask ("Color Mask", Float) = 15
        //--------------add------------------
        _Distance ("Distance", Float) = 0.015
        //--------------add------------------
        _StartTime("StartTime",Float) = 0  
        _NoiseTex ("NoiseTex (R)",2D) = "white"{}  
        _DissolveSpeed ("DissolveSpeed (Second)",Float) = 1 
        _EdgeColor("EdgeColor",Color) =  (1,1,1,1)  
	}
 
	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
 
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]
 
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};
 
			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
 
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				OUT.color = IN.color * _Color;
				return OUT;
			}
 
			sampler2D _MainTex;
            //--------------add------------------
            float _Distance;
            uniform float _StartTime;  
            uniform float _DissolveSpeed;  
             uniform sampler2D _NoiseTex;  
             uniform float4 _EdgeColor;
            //--------------add------------------
			fixed4 frag(v2f IN) : SV_Target
			{

				float DissolveFactor = saturate((_Time.y - _StartTime) / _DissolveSpeed);  
                float noiseValue = tex2D(_NoiseTex,IN.texcoord).r;              
                if(noiseValue <= DissolveFactor)  
                {  
                    discard;  
                }  
                  
                float4 texColor = tex2D(_MainTex,IN.texcoord)* IN.color;  
                float3 texColorrgb=float3(texColor.r,texColor.g,texColor.b);
                float EdgeFactor = saturate((noiseValue - DissolveFactor)/(0.1*DissolveFactor));  
                float3 BlendColor = texColorrgb * _EdgeColor;  
                                  
                return float4(lerp(texColorrgb,BlendColor,1 - EdgeFactor),texColor.a);  

                //--------------add------------------
                //float distance = _Distance;
                //fixed4 computedColor = tex2D(_MainTex, IN.texcoord);

                //return computedColor;
                //--------------add------------------
			}
		ENDCG
		}
	}
}
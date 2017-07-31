// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/UI/UI Flash" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_FlashTex ("Base (RGB) Trans(A)", 2D) = "white" {}
		_DirectionUv("Scroll direction (X Y)  Scroll speed (Z W)", Vector) = (1.0,1.0, 2, 0)
	//	_Speed ("Speed", Range (0, 50)) = 1
	}
SubShader 
{
			Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
	

	Pass 
	{
		
		Blend SrcAlpha OneMinusSrcAlpha
		zwrite off
		CGPROGRAM

//#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
				
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _FlashTex;
			fixed4 _Color;
			half4 _DirectionUv;
		//	float _Speed;
			struct v2f
			{
				float4	pos : SV_POSITION;
				float2	uv_MainTex : TEXCOORD0;
				float2	uv_FlashTex : TEXCOORD1;
			};  
			
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.uv_MainTex = v.texcoord.xy;
				o.uv_FlashTex = v.texcoord.xy;
				o.uv_FlashTex -= _Time.xx * _DirectionUv.zw * _DirectionUv.xy; 
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			float4 frag(v2f i) : COLOR
			{
				fixed4 texColor = tex2D(_MainTex, i.uv_MainTex);
				//float2 flashUV = i.uv_MainTex;
				//flashUV.x = frac(_Time.y * 0.01); 
				fixed4 flashColor = tex2D(_FlashTex, i.uv_FlashTex);
				
				texColor.rgb += (_Color.rgb * flashColor.a * texColor.a);
				
				return texColor;
			}

		ENDCG
	}	
}
	FallBack "Diffuse"
}

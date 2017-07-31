// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "GOE/Scene/Mirror" {
	Properties {
		_Distortion ("Distortion", range (0,30)) = 10
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
		_BlendLevel("Main Material Blend Level",Range(0,1))=1
		_Ref ("For Mirror reflection,don't set it!", 2D) = "white" {}
	}

	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Opaque" }

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma debug
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Ref;
			fixed4 _Color;
			half _BlendLevel;
			half _Distortion;

	
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				half3 worldNormal : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.screenPos = ComputeScreenPos (o.pos);
				return o;
			}


			fixed4 frag (v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv0);
				
				float4 screenUV = i.screenPos;
				float2 offset = (_Distortion * i.worldNormal) ;
			    screenUV.xy  += offset ;
			    float3 ref = tex2Dproj(_Ref, screenUV);
				fixed3 col = lerp(ref.rgb, tex.rgb, _BlendLevel);
				fixed4 finalColor = fixed4(col * _Color.rgb, tex.a * _Color.a);
				return finalColor;
			}
			ENDCG
		}
	}
}
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/uv animation" {
	Properties {
		Speed ("纹理滚动速度", Range (0.01, 50.0)) = 1
		Direction ("纹理滚动方向", Range (-1, 1)) = 1
		_MainTex ("纹理贴图", 2D) = "white" {}
	}
	SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True"}
		Pass {
			Blend srcalpha oneminussrcalpha
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float Speed;
			float Direction;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			struct VertexIn {
				float4 position : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			
			struct VertexToSurf {
				float4 oPosition : POSITION;
				float2 uvPosition : TEXCOORD0;
			};
			
			void vert (
				in VertexIn posIn,
				out VertexToSurf posOut
				) {
				posOut.oPosition = UnityObjectToClipPos(posIn.position);
				posOut.uvPosition = TRANSFORM_TEX(posIn.texcoord, _MainTex);
			}
			
			void frag(
				in VertexToSurf posOut,
				out float4 color : COLOR
				) {
				float2 uv;
				uv.x = posOut.uvPosition.x + _Time.x * Speed * Direction;
				uv.y = posOut.uvPosition.y;
				color = tex2D(_MainTex, uv);
			}
			
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}

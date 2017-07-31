// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Role/Sunder" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OutLineTex ("Out Line (RGB)", 2D) = "" {TexGen SphereMap}
	}
SubShader {
	Tags {"Queue"="Geometry"}
	Pass 
	{
		CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
				
			#include "UnityCG.cginc"
			#include "LightFace.cginc"

			sampler2D _MainTex;
			sampler2D _OutLineTex;
			fixed4 _Color;
			struct v2f
			{
				float4	pos : SV_POSITION;
				float2	uv_MainTex : TEXCOORD0;
				float2	uv_OutLineTex : TEXCOORD1;
			};  
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv_MainTex = v.texcoord.xy;
				
				o.uv_OutLineTex.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
				o.uv_OutLineTex.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
				o.uv_OutLineTex.xy = o.uv_OutLineTex.xy * 0.5 + 0.5;
				return o;

			}
			
			float4 frag(v2f i) : COLOR
			{
				fixed4 diff = tex2D(_MainTex, i.uv_MainTex);
				fixed4 outline = tex2D(_OutLineTex, i.uv_OutLineTex);
				float alpha = abs ((1-outline.a) * sin( _Time.y));
				fixed4 lastColor = _Color * alpha;

				diff.rgb = diff.rgb + lastColor.rgb;
				return diff;
			}

		ENDCG
	}	
}
	FallBack "Diffuse"
}

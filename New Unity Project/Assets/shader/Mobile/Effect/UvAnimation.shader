// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Particles/Add UV Animation" {
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AlphaTex ("Alpha Texture", 2D) = "white" {}
	}
	// normal //
	//-------------------------------------------------------------------------------//
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True"}
		LOD 50
		
		Pass 
		{
			Blend SrcAlpha one 
			ZWrite off
			cull off
			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
					
				#include "UnityCG.cginc"
				
				half4 _MainTex_ST;
				half4 _AlphaTex_ST;
				fixed4 _Color;
				sampler2D _MainTex;
				sampler2D _AlphaTex;
          		//half _IllumFactor;
				struct v2f
				{
					float4	pos : SV_POSITION;
					
                	float2	uv_MainTex : TEXCOORD0;
					float2	uv_AlphaTex : TEXCOORD1;
				};  
				
				
				
				v2f vert(appdata_full  v)
				{
					v2f o;
					half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.pos = UnityObjectToClipPos(v.vertex);
            
					o.uv_MainTex =  TRANSFORM_TEX( v.texcoord, _MainTex);
					o.uv_AlphaTex = TRANSFORM_TEX( v.texcoord, _AlphaTex);

					return o;
				}

				
				float4 frag(v2f i) : COLOR
				{
					float4 tex = tex2D(_MainTex, i.uv_MainTex);
					//float4 tex1 = tex2D(_MainTex, i.uv_AlphaTex);
					
					float4 alphaTex = tex2D(_AlphaTex, i.uv_AlphaTex);
					
					float4 lastColor = _Color;// = tex;
					if (alphaTex.a < 0.2)
					{
						//lastColor = tex1;
						lastColor.a = 0;
						lastColor.rgb = float3(0,0,0);
					}
					else
					{
						lastColor.rgb *= tex.rgb;
						lastColor.rgb *= alphaTex.a;
						lastColor.a = alphaTex.a;
					}
					
					
	
					return lastColor ;
				}
	
			ENDCG
		}	
	} 

	
	FallBack "Diffuse"
}

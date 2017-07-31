// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Ice" 
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)	
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_IceTex ("Ice (RGB)", 2D) = "white" {}
       // _RealColor ("Real Color", Color) = (0.0,0.0,0.0,1)
       // _IllumFactor ("Illumin Factor", Range(1,10)) = 1
	}

SubShader {

	Tags {"Queue"="Geometry" "IgnoreProjector"="True"}
	LOD 200
		
	Pass 
	{
			
		Tags {  "LightMode" = "ForwardBase" }
		//blend srcAlpha oneminusSrcAlpha
	
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
				
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
			sampler2D _MainTex;
			sampler2D _IceTex;
			//fixed4 _RealColor;
			fixed4 _Color;
			//float _IllumFactor;

			struct v2f
			{
				float4	pos : SV_POSITION;
				half2	uv : TEXCOORD0;
			}; 
		

			v2f vert (appdata_full v)
			{
				v2f o;
				o.uv = v.texcoord.xy;
				o.pos =  UnityObjectToClipPos(float4(v.vertex.xyz, 1));
				return o;
			}
				

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 ice = tex2D(_IceTex, i.uv*3);
//				fixed4 c;
//				//fixed3 iceColor = ;
//				c.rgb = temp * _Color.rgb * 2;
				tex.rgb =tex.rgb*0.3 + 0.7* ice.rgb;
				tex.rgb *= _Color.rgb;
				//tex.a += 0.4;
				
				return tex;
			}

		ENDCG
	}	
}
//FallBack "Diffuse"
//FallBack "Diffuse"
}






// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Scene/Background" 
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1
	_Speed ("speed", Range(0,1)) = 0
}


SubShader 
{
	Tags {"Queue"="Geometry" "IgnoreProjector"="True" }
		
	Pass 
	{
			
		Tags {  "LightMode" = "ForwardBase" }
		
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers d3d11
	
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "LightFace.cginc"
			
			sampler2D _MainTex;
			fixed4 _Color;
			half _IllumFactor;
			half _Speed;
			
			struct v2f
			{
				float4	pos : SV_POSITION;
				half2	uv : TEXCOORD0;
				FOG_COORDS(1)
			}; 
			
			struct appdata
			{
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			};
		

			v2f vert (appdata v)
			{
				WORLD_POS
				v2f o;
				o.uv.xy = v.texcoord.xy;
				o.pos =  UnityObjectToClipPos(float4(v.vertex.xyz, 1));

				FOG_VS
				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
				half2 uv =  i.uv.xy;
				uv.x += _Time.x*_Speed*0.1;
				fixed4 tex = tex2D(_MainTex, uv);
				tex.rgb *= _Color;
				tex.rgb *= _IllumFactor;
				FOG_PS
				return tex;
			}

		ENDCG
	}	
}
}




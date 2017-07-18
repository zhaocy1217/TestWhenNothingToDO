Shader "GOE/Effect/Fog" 
{
Properties 
{
	_Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_AlphaMaskTex ("ALPHA (RGB) MASK (A)", 2D) = "white" {}
	_bloom ("Bloom Factor", Range(0,2)) = 2
	_speed ("speed Factor", Range(0,2)) = 1
}

SubShader {

	Tags {"Queue"="Geometry+199" "IgnoreProjector"="True"}
	LOD 200
	cull off
	//Ztest Always
	Lighting Off ZWrite Off Fog { Color (0,0,0,0) }

	Pass 
	{
		Blend  One One
		ColorMask RGB
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _AlphaMaskTex;
			fixed4 _Color;
			float4 _MainTex_ST;
			fixed _bloom;
			half _speed;
			struct v2f
			{
				float4	pos : SV_POSITION;
				half2	uv : TEXCOORD0;
			}; 
			
			struct appdata
			{
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			};
			

			v2f vert (appdata v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.pos =  mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
				float2 uv = i.uv;
				uv.x  += _Time.x * _speed;
				fixed4 c =  _Color * tex2D(_MainTex, uv);
				c *= _bloom;
				c.rgb *= tex2D(_AlphaMaskTex, i.uv).rgb;
				return  c;
			}

		ENDCG
	}		
}
}




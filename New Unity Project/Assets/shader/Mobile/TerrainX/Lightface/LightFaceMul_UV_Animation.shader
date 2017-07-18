Shader "GOE/LightFace/LightFaceMul UV Animation" 
{
Properties 
{
	_ShadowFactor("Shadow LightFace Factor", Range(0,1)) = 0.4
	_TintColor("Tint Color", Color) = (1,1,1,1)
	_MainTex("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_Speed ("纹理滚动速度", Range (0.01, 8)) = 1
	_Direction ("纹理滚动方向", Range (-1, 1)) = 1
	_MaskTex("Mask (RGB)", 2D) = "white" {}
}


SubShader {

	Tags {"Queue"="Geometry" "IgnoreProjector"="True"}
	ColorMask RGB
	Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	Pass 
	{
			
		Tags {  "LightMode" = "ForwardBase" }

		Blend  srcalpha oneminussrcalpha, zero one
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
				
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _ShadowFactor;
			fixed4 _TintColor;
			float _Speed;
			float _Direction;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;

			struct v2f
			{
				float4	pos : SV_POSITION;
				fixed4 color : COLOR;
				half2	uv : TEXCOORD0;
			}; 
			
			struct appdata
			{
			    float4 vertex : POSITION;
			    fixed4 color : COLOR;
			    float4 texcoord : TEXCOORD0;
			};
			

			v2f vert (appdata v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);//v.texcoord.xy;
				o.color = v.color;
				o.pos =  mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
				fixed4 mask = tex2D(_MaskTex, i.uv);
				i.uv.x += _Time.x * _Speed * _Direction;
				fixed4 tex = tex2D(_MainTex, i.uv);
				tex.rgb = _ShadowFactor * _TintColor;
				tex.a *= mask.r;
				return tex;
			}


		ENDCG
	}	
}
}




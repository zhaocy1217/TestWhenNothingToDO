// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/LightFace/LightFaceAdd" 
{
Properties 
{
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_NightColor("NightColor", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_bloom ("Bloom Factor", Range(0,4)) =2
}

SubShader {

	Tags {"Queue"="Transparent" "IgnoreProjector"="True"}
	
	Pass 
	{
		cull off
		Lighting Off 
		//ZWrite Off 
		Fog { Color (0,0,0,0) }
		Blend  One OneMinusSrcColor,zero one
		//blend one zero
		ColorMask RGB
		CGPROGRAM
			#pragma exclude_renderers d3d11 flash gles3 metal
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			fixed4 _TintColor;
			float4 _MainTex_ST;
			fixed _bloom;
			fixed4 _NightColor;
			float _GameTime;

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
				o.pos =  UnityObjectToClipPos(float4(v.vertex.xyz, 1));
				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				tex *= _bloom * lerp(_NightColor, _TintColor, _GameTime);
				//c.r = 1;
				return  tex;
			}

		ENDCG
	}		
}

}




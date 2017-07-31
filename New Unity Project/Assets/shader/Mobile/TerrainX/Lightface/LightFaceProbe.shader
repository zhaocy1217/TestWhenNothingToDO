// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/LightFace/LightFaceProbe" 
{
Properties 
{
	_LampColor("Lamp Color", Color) = (1,1,1,1)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_LampFactor ("Lamp Factor", Range(0,3)) = 1
}

SubShader {

	Tags {"Queue"="Transparent+120" "IgnoreProjector"="True"}
	

	LOD 200
	Pass 
	{
		
		cull off
		Lighting Off 
		ZWrite On 
		Fog { Color (0,0,0,0) }
		//blend oneminusdstalpha one
		// 3.第三步写探照光，点光有的地方，则探照灯不写，所以用oneminusdstalpha， //
		//blend oneminusdstalpha oneminussrcalpha
		//Blend  srcalpha oneminussrcalpha
		blend oneminusdstalpha srcalpha
		ColorMask RGBA
		CGPROGRAM
			#pragma exclude_renderers d3d11 flash
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			
			sampler2D _MainTex;
			fixed4 _LampColor;
			float4 _MainTex_ST;
			fixed _LampFactor;
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
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.color = v.color;
				o.pos =  UnityObjectToClipPos(float4(v.vertex.xyz, 1));
				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				//c.rgb *= c.a;
				//c.rgb = c.rgb * _LampColor +  _LampColor * _LampFactor;
				c.rgb = 0.3;//*= _LampColor * _LampFactor;
				//c.rgb = c.rgb * 0.5 + 0.5;
				c.rgb *= c.a;
				//c.rgb += _LampColor * _LampFactor;
				//c.rgb *= 1 - _LightColor0.rgb;
				return  c;
			}

		ENDCG
	}	

}
}
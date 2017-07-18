Shader "GOE/Effect/Sun Shine" 
{
Properties 
{
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_sunshineLight ("sun shine Factor", Range(0,2)) = 2
}

SubShader {

	Tags {"Queue"="Transparent" "IgnoreProjector"="True"}
	


	Pass 
	{
		cull off
		Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		Blend  one one
		ColorMask RGB
		CGPROGRAM
			#pragma exclude_renderers d3d11 flash gles3
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
			
			
			sampler2D _MainTex;
			fixed4 _TintColor;
			float4 _MainTex_ST;
			fixed _sunshineLight;
			struct v2f
			{
				float4	pos : SV_POSITION;
				fixed4 color : COLOR;
				half3	uv : TEXCOORD0;
				//float3 worldPos : TEXCOORD1;
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
				//o.worldPos = mul(_Object2World, float4(v.vertex.xyz, 1)).xyz;
				
				o.uv.xy = TRANSFORM_TEX(v.texcoord,_MainTex).xy;
				o.color = v.color;
				o.pos =  mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
				o.uv.z = (o.pos.z);
				return o;
			}

			
			float4 frag(v2f i) : COLOR
			{
				float2 uv = i.uv;
				//uv.y += _Time.x*2;
				fixed4 mask0 = tex2D(_MainTex, uv);
				float2 maskUV = i.uv;
				maskUV.x += _Time.x*0.2;
				fixed4 mask1 = tex2D(_MainTex, maskUV);
				
				maskUV = i.uv;
				maskUV.x -= _Time.x*0.3;
				fixed4 mask2 =	tex2D(_MainTex, maskUV);
				
				fixed4 mask = (mask0 + mask1 + mask2) * 0.33;
				
				fixed4 c = i.color * _TintColor * mask;

			

			
				c *= _sunshineLight;


			//	float dis = distance(_WorldSpaceCameraPos, i.worldPos);
				 
				c.rgb *= min(1,(max(0, i.uv.z - 5)/5));
				//c.a -= (2*_sunshineLight);
				return  c;
			}

		ENDCG
	}		
}
}




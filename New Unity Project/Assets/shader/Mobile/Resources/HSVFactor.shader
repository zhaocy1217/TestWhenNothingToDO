// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/HSVFactor" 
{
	Properties
	{
		_HSVFactor ("HSV Factor", Vector) = (1,0.5,0.5,1)	
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

SubShader {

	Tags {"Queue"="Geometry" "IgnoreProjector"="True"}
	LOD 200
		
	Pass 
	{
			
		Tags {  "LightMode" = "ForwardBase" }

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
				
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
			sampler2D _MainTex;
			fixed4 _HSVFactor;
			
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
			fixed3 rgb2hsv(fixed3 c)
			{
				fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				fixed4 p = lerp(fixed4(c.bg, K.wz), fixed4(c.gb, K.xy), step(c.b, c.g));
				fixed4 q = lerp(fixed4(p.xyw, c.r), fixed4(c.r, p.yzx), step(p.x, c.r));

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}	
			
			fixed3 hsv2rgb(fixed3 c)
			{
				fixed4 K = fixed4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				fixed3 p = c.xxx + K.xyz;
				p = abs((p - floor(p)) * 6.0 - K.www);
				return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed3 hsv = rgb2hsv(tex.rgb);
				fixed3 one = fixed3(1,1,1);
				fixed dt = 1;
				fixed tf = saturate((_Time.y - _HSVFactor.a) / dt);
				fixed3 factor = lerp(_HSVFactor.rgb, one, 1 - tf);
				hsv = hsv * factor;
				fixed4 c;
				c.rgb = hsv2rgb(hsv);
				c.a = 1;
				
				return c;
			}

		ENDCG
	}	
}
//FallBack "Diffuse"
//FallBack "Diffuse"
}






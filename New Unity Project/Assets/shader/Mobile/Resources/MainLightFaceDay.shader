Shader "GOE/Main LightFace Day" 
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
}

SubShader {

	Tags {"Queue"="Transparent+150" "IgnoreProjector"="True"}
	LOD 200
		
	Pass 
	{
		cull off
		
		// 4.第四步写遮罩图，防止出现边缘采样问题，同时清空alpha通道，为阴影做准备 //
		blend SrcAlpha OneMinusSrcAlpha, zero zero
		Colormask RGBA
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
				
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			
			sampler2D _MainTex;
			fixed4 _Color;
			
			struct v2f
			{
				float4	pos : SV_POSITION;
				half2	uv : TEXCOORD0;
			}; 
		

			v2f vert (appdata_full v)
			{
				v2f o;
				o.uv = v.texcoord.xy;
				o.pos = float4(v.vertex.xyz, 1);
				o.pos.z = 0.5f;
				return o;
			}
				

			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				// 忽略贴图颜色，用lightcolor的“底色”作为边缘颜色过度 //
				// tex.rgb = _LightColor0 * UNITY_LIGHTMODEL_AMBIENT * 0.5 * 0;
				// 新的方式底图就是0.5 //
				tex.rgb = 0.5;
				//tex.a = 0;
				return tex;
			}

		ENDCG
	}
}
}




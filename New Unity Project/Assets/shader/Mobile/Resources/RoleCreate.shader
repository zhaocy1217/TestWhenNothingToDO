// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/RoleCreate"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RefTex ("Ref Tex (RGB)", 2D) = "black" {}
		_BackColorFactor("BackColorFactor", Range(0,8)) = 5
		_BackColor("Back Color", Color) = (1,1,1,1)
		_LightDir("Light Dir", Vector) = (0.6,-1,-0.21,1)
	}
	
	Subshader
	{
		Tags { "Queue"="Geometry+200" "IgnoreProjector"="True" }
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			cull off
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma exclude_renderers d3d11 flash
				#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
				#include "UnityCG.cginc"
				#include "LightFace.cginc"
				
				struct v2f
				{
					float4 pos	: SV_POSITION;
					float4 uv 	: TEXCOORD0;
					float4 uv_ref_lightface	: TEXCOORD1;
					float4  depth : TEXCOORD2;
				};
				
				uniform float4 _MainTex_ST;
				float4x4 _texView;
				float _BackColorFactor;
				float4 _BackColor;
				float4 _LightDir;
				v2f vert (appdata_base v)
				{
					v2f o;
					WORLD_POS
					
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex).xy;
					
					half2 capCoord;
					capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
					capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
					o.uv.zw = capCoord * 0.5f + 0.5f;
					
					float3 worldNormal = UnityObjectToWorldNormal(v.normal.xyz);
					o.uv_ref_lightface.x = max(0.0,dot(worldNormal, normalize(_WorldSpaceLightPos0.xyz)));
					float3 camNormal = normalize(mul(UNITY_MATRIX_MV, float4(v.normal.xyz, 0)));
					o.uv_ref_lightface.y = max( 0, dot(camNormal, normalize(_LightDir.xyz)));
#ifdef LIGHT_FACE_ON	
					o.uv_ref_lightface.zw = LightFaceUV(worldPos).xy;
#else
					o.uv_ref_lightface.zw = half2(0,0);			
#endif
					o.depth = SimulateFogVS(o.pos.xyz, worldPos.xyz);
					return o;
				}
				
				uniform sampler2D _MainTex;
				uniform sampler2D _RefTex;
				fixed4 _Color;
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 tex = tex2D(_MainTex, i.uv.xy);
					fixed4 ref = tex2D(_RefTex, i.uv.zw);
					
					tex.rgb = tex.rgb + tex.rgb * _LightColor0 * i.uv_ref_lightface.x + tex.rgb * _BackColor * i.uv_ref_lightface.y * _BackColorFactor + (ref.rgb)*(tex.a*2);

					LightFaceColorRole(tex.rgba, i.uv_ref_lightface.zw, fixed3(1,1,1), 0.15);

					tex.rgb *= _Color.rgb;
					tex.xyz = SimulateFog(i.depth, tex, 1);

					//fixed temp = (tex.x + tex.y +tex.z)* 0.35f;
					return tex;
				}
			ENDCG
		}
	}
	
}
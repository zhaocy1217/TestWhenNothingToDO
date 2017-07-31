// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Scene/Lightmap Reflect" 
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_RefTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_EnvironmentPower ("Environment Factor", Range(0,2)) = 0
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1
}


SubShader {

	Tags {"Queue"="Geometry" "IgnoreProjector"="True"}		
	Pass 
	{
		Tags {  "LightMode" = "ForwardBase" }
		Fog { Mode off } 
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers d3d11 flash
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "LightFace.cginc"
			
			sampler2D _MainTex;
			sampler2D _RefTex;
			half4 _MainTex_ST;
			half4 _RefTex_ST;
			fixed4 _Color;
			fixed _EnvironmentPower;
			half _IllumFactor;

			struct v2f
			{
				float4	pos : SV_POSITION;
				half4	uv : TEXCOORD0;
				half4  reflectUV :TEXCOORD1;
				LIGHTFACE_COORDS(2)
				FOG_COORDS(3)
			}; 
		
			
			struct appdata
			{
			    float4 vertex : POSITION;
			    float4 normal : NORMAL;
			    float4 texcoord : TEXCOORD0;
				LIGHTMAP_COORDS(1)
			};
			
			float3 reflect(float3 I,float3 N)
            {
                return I - 2.0*N *dot(N,I);
            }
			
			v2f vert (appdata v)
			{
				WORLD_POS
				v2f o;
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
							
				o.pos =  UnityObjectToClipPos(float4(v.vertex.xyz, 1));
				o.reflectUV = ComputeScreenPos(o.pos);
				o.reflectUV.xy = TRANSFORM_TEX(o.reflectUV.xy, _RefTex)/o.reflectUV.w;
				half2 capCoord;
				capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
				capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
				o.reflectUV.zw = capCoord * 0.5f + 0.5f;

				LIGHTMAP_VS
				LIGHTFACE_VS
				FOG_VS
				return o;
			}
				

			float4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				half4 reflectiveColor = tex2D(_RefTex, (i.reflectUV.xy/i.reflectUV.w)) +  tex2D(_RefTex, i.reflectUV.zw)*2;

				tex.a = tex.a;
				tex.rgb *= _Color;
				tex.rgb *= _IllumFactor;
				
				LIGHTMAP_PS

				_EnvironmentPower *= (tex.a);
				tex.rgb = tex.rgb + reflectiveColor.rgb * _EnvironmentPower;

				LIGHTFACE_PS
				FOG_PS
				
				return tex;
			}

		ENDCG
	}	
}
}




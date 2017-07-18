Shader "GOE/Scene/LightLine"
{
	Properties 
	{
		_MainTex ("Base texture", 2D) = "white" {}
		_MoveDistance("Move Distance", float) = 1
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend srcalpha one
		cull off
		zwrite off
		
		
		Pass 
		{
			CGPROGRAM	
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest		
			#include "UnityCG.cginc"
#include "LightFace/LightFace.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _MoveDistance;

			
			struct v2f 
			{
				float4	pos	: SV_POSITION;
				float2	uv	: TEXCOORD0;
				float randAlpha: TEXCOORD1;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				
							
				WORLD_POS
				o.randAlpha = frac(worldPos.x + worldPos.y + worldPos.z);
				
				float3 worldNormal = UnityObjectToWorldNormal(v.normal.xyz);
				worldPos.xyz += worldNormal * sin(_Time.y) * _MoveDistance;
				
				o.pos = mul(UNITY_MATRIX_VP, worldPos);
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{		
				fixed4 col = tex2D(_MainTex, i.uv);
				col.a =  sin( 2.5*_Time.y) + 1.5;
				return col;
			}
			ENDCG
		}
	}
}
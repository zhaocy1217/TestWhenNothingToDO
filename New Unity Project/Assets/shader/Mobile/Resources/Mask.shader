Shader "GOE/Mask" 
{
	SubShader 
	{
		Tags { "Queue"="Geometry-150" "RenderType"="Opaque" }

		CGINCLUDE
		struct appdata 
		{
			float4 vertex : POSITION;
		};
		struct v2f 
		{
			float4 pos : SV_POSITION;
		};
		v2f vert(appdata v) 
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			return o;
		}
		half4 frag(v2f i) : SV_Target 
		{
			return half4(1,1,0,1);
		}
		ENDCG

		Pass 
		{
			ColorMask 0
			ZWrite Off
			Stencil
			{
				Ref 1
				Comp Always
				Pass Replace
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
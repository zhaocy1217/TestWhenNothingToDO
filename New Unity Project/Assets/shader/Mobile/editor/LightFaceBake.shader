Shader "GOE/Editor/LightFaceBake" 
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf GOELambert  alphatest:_Cutoff

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};
		
		inline fixed4 LightingGOELambert (SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed diff = max (0, dot (s.Normal, lightDir));
			diff = min(dot (float3(0,1,0), lightDir), diff);
			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * diff* atten;
			c.a = s.Alpha;
			return c;
		}
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo.rgb = half3(0.5,0.5,0.5);
			//o.Albedo = c.rgb;
			o.Alpha = tex.a;
		}
		ENDCG
	} 
	Fallback "Transparent/Cutout/VertexLit"
}

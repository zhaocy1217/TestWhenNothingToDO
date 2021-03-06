// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Const/DiffuseTemp" 
{
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_IllumFactor ("Illumin Factor", Range(1,2)) = 1
}

SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True" }
	LOD 200
	
CGPROGRAM
#pragma surface surf BlinnPhong
sampler2D _MainTex;
fixed4 _Color;
half _Shininess;
half _IllumFactor;

struct Input {
	float2 uv_MainTex;
};




void surf (Input IN, inout SurfaceOutput o) 
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb * _IllumFactor;
		
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
}
ENDCG
}

Fallback "VertexLit"
}
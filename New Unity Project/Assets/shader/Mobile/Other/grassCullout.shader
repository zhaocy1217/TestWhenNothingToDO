Shader "GOE/Other/Grass"
{
Properties 
{
	_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_MainTex ("Diffuse (RGB) Transparent (A)", 2D) = "white" {}
	_IllumFactor ("Illumin Factor", Range(0,2)) = 1
	_WindSpeed ("Wind Speed", Range(0,1)) = 0.5
	_RockPower ("Rock Power", Range(0,1)) = 0.3
	_RockSpeed ("Rock Speed", Range(1,10)) = 3
}


SubShader {

	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True"}
	
	cull off
	
	CGPROGRAM
	#pragma surface surf Lambert  alphatest:_Cutoff  vertex:vert	
	

	float _RockPower;
	fixed _RockSpeed;
	float _WindSpeed;
	
	sampler2D _MainTex;
	fixed4 _Color;
	fixed _IllumFactor;
	
	sampler2D _LightFace;
	float4x4 _texViewProj;
	
	struct Input 
	{
		float2 uv_MainTex;
	//	float4 color : COLOR;
		float3 worldPos;
	};
	
//	float4 GrassRock(float4 pos, float2 uv,  float2 uv1, float dir)
//	{
//		
//		if (uv.y > 0.99)
//		{					
//			
//			float windx = 0;
//			float windz = 0;
//
//			if (dir > 0)
//			{
//				windx = sin( _Time.y * _WindSpeed)* 0.3;
//			}
//			else
//			{
//				windz = cos( _Time.y * _WindSpeed)* 0.3;
//			}
//			if (uv1.x > 0)
//			{
//				float percentage = 1f - uv1.x/(3.1415926);
//				if (dir < 0)
//				{
//					windx += sin( uv1.x * _RockSpeed)* _RockPower * percentage;
//					windz += cos( uv1.x * _RockSpeed)* _RockPower * percentage;
//				}
//				else
//				{
//					windx += -sin( uv1.x * _RockSpeed)* _RockPower * percentage;
//					windz += -cos( uv1.x * _RockSpeed)* _RockPower * percentage;
//				}
//			}
//
//			pos.x += windx;
//			pos.z += windz;
//		}
//		return pos;
//	}
	
	
	
	
	void vert(inout appdata_full v)
	{
		//v.vertex = GrassRock(v.vertex, v.texcoord.xy, v.texcoord1.xy, v.tangent.x);
		v.vertex = v.vertex;
	}
	
	void surf (Input IN, inout SurfaceOutput o) 
	{
		float4 lightfaceUV = mul(_texViewProj, float4(IN.worldPos, 1));
		lightfaceUV = lightfaceUV/lightfaceUV.w;
		lightfaceUV.x = (lightfaceUV.x +1.0f)/2.0;
		lightfaceUV.y = (lightfaceUV.y +1.0f)/2.0;
		float4 lightfaceColor = tex2D (_LightFace, lightfaceUV.xy).rgba;
	
	
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = tex.rgb * _Color.rgb * lightfaceColor.rgb;
		o.Emission = tex.rgb * _IllumFactor;
		o.Alpha = tex.a * _Color.a;
		o.Normal = half3(0,1,0);
	}
	ENDCG
}
//FallBack "Transparent/Cutout/VertexLit"
}



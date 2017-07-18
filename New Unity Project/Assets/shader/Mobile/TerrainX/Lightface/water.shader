// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "GOE/Scene/Water" 
{ 
Properties 
{	
	_MainTex ("Normals", 2D) = "black" {}	
	_BaseColor ("Base color", COLOR)  = ( .54, .95, .99, 0.5)	
	_ReflectionColor ("Reflection color", COLOR)  = ( .54, .95, .99, 0.5)
	_RefractionTex ("ref", 2D) = "black" {}		
				
	_BumpTiling ("Bump Tiling", Vector) = (1 ,1, 1, 1)
	_BumpDirection ("Direction and Speed", Vector) = (0 ,1, 1,0)
	_BumpWaves ("Bump Waves", Range (0, 1)) = 0.5	
	
	
	_SunColor ("Sun color", COLOR)  = ( .54, .95, .99, 0.5)
	_SunDirection("Sun direction", Vector) = (1.0,1.0, -0.2,-0.2)
	_Shininess ("Shininess", Range (2.0, 500.0)) = 200.0	
	_alpha ("alpha", Range (0, 1.0)) = 0.4	
} 


CGINCLUDE
	
	#include "UnityCG.cginc"
	#include "Lighting.cginc"
	#include "LightFace.cginc"
	struct appdata 
	{
		float4 vertex : POSITION;
		float4 col : COLOR;
		float3 normal : NORMAL;
	};

	// interpolator structs
	
	struct v2f 
	{
		float4 pos : SV_POSITION;
		float4 col : COLOR;
		float4 uv : TEXCOORD0; 
		float3 viewDir : TEXCOORD1;
		float3 LightDir : TEXCOORD2; 	
		//half4 bumpCoords : TEXCOORD3;
		float4  depth : TEXCOORD3;
		float2 uvLightface : TEXCOORD4;
	};

	// textures
	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;
	sampler2D _RefractionTex;
	uniform fixed4 _BaseColor;
	uniform fixed4 _ReflectionColor;
	
	uniform half _EdgeBlend;
	uniform float _Shininess;

	uniform float4 _BumpTiling;
	uniform float4 _BumpDirection;
	uniform fixed4 _SunColor;
	uniform float4 _SunDirection;
	float4 _DistortParams;
	uniform fixed _alpha;
	fixed _BumpWaves;

    // half4 unity_LightmapST;
	// sampler2D unity_Lightmap;
	//fixed light; 
	inline half3 PerPixelNormal(sampler2D bumpMap, half4 coords, half3 vertexNormal, half bumpStrength) 
	{
		//fixed4 normalColor = tex2D(bumpMap, coords.xy) + tex2D(bumpMap, coords.zw);
		//normalColor.rgb *= 0.5f;
		//half3 bump = UnpackNormal(normalColor);
		half3 bump = UnpackNormal(tex2D(bumpMap, coords.xy))+ UnpackNormal(tex2D(bumpMap, coords.zw));
		//bump.rgb *= 0.5f;
		//bump.xy = bump.xy - half2(1.0, 1.0);
		//bump.xyz = bump.xyz  half3(0.3, 1.0, 0.3);
		//bump.xyz = bump.xyz - half3(1.0, 1.0, 1.0);
		//bump.xyz = UnpackNormal(bump);
		//bump = bump *2 -1;
		
		//bump = bump * 2 -1;
		//half3 worldNormal = vertexNormal + bump.xyz* bumpStrength * half3(1,1,1);// * bumpStrength * half3(1,0,1);
		float temp =  bump.y;
		bump.y = bump.z;
		bump.z = temp;
		half3 worldNormal = vertexNormal.xyz * (1-bumpStrength)+ bump.xyz*(bumpStrength);// * half3(0.6,1,0.6);
		return normalize(worldNormal);
	} 
	
//	inline void ComputeScreenAndGrabPassPos (float4 pos, out float4 screenPos, out float4 grabPassPos) 
//	{
////		#if UNITY_UV_STARTS_AT_TOP
////			float scale = -1.0;
////		#else
//			float scale = 1.0f;
////		#endif
//		
//		screenPos = ComputeScreenPos(pos); 
//		grabPassPos.xy = ( float2( pos.x, pos.y*scale ) + pos.w ) * 0.5;
//		grabPassPos.zw = pos.zw;
//	}
	
	
	v2f vert(appdata_full v)
	{
		v2f o;
		o.col = v.color;
		o.uv = v.texcoord;
		//o.uv.x += _Time.x*0.1;
		float4 worldSpaceVertex = mul(unity_ObjectToWorld,(v.vertex)).xyzw;
		//worldSpaceVertex.xyz /= worldSpaceVertex.w;					
		//half2 tileableUv = worldSpaceVertex.xz;		
				
		//o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;	
		//o.bumpCoords.xyzw = tileableUv.xyxy;// (tileableUv.xyxy + _BumpDirection.xyzw) * _BumpTiling.xyzw;	

		o.viewDir.xyz = normalize(worldSpaceVertex - _WorldSpaceCameraPos);
		o.LightDir.xyz = normalize(worldSpaceVertex - _SunDirection.xyz);
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		
		o.depth = SimulateFogVS(o.pos.xyz, worldSpaceVertex.xyz);

#ifdef LIGHT_FACE_ON
		o.uvLightface.xy = LightFaceUV(worldSpaceVertex);
#else
		o.uvLightface.xy = half2(0, 0);
#endif
		return o;
	}

	half4 frag( v2f i ) : COLOR
	{	
		half3 normalInterpolator = half3(0,1,0);

#ifdef LIGHT_FACE_ON
		half3 worldNormal = PerPixelNormal(_MainTex, (i.uv.xyxy + _BumpDirection.xyzw*_Time.xxxx)*_BumpTiling.xyzw, normalInterpolator, _BumpWaves);
#else
		half3 worldNormal = UnpackNormal(tex2D(_MainTex, (i.uv.xy + _BumpDirection.xy*_Time.xx)*_BumpTiling.xy));
#endif
		
		float3 viewDir = i.viewDir.xyz;// normalize(i.viewDir.xyz);
		float3 lightDir = i.LightDir.xyz;// normalize(i.LightDir.xyz);
		

#ifdef LIGHT_FACE_ON
		half3 h = normalize (lightDir + viewDir);
		float nh = max (0, dot ((worldNormal), -h));
		float spec = max(0, pow (nh, _Shininess));
#endif
		
		half4 distortOffset = half4(worldNormal.xz * 0.05, 0, 0);
		half4 grabWithOffset = i.uv + distortOffset;
		half4 uv = grabWithOffset.xyzw;

		half4 rtRefractions = tex2D(_RefractionTex,uv.xy);
		half refl2Refr = worldNormal.y *0.2;

		half4 baseColor = _BaseColor;

		
		//half4 reflectionColor = _ReflectionColor;
		baseColor = lerp (lerp (rtRefractions, baseColor, baseColor.a), _ReflectionColor, refl2Refr);

#ifdef LIGHT_FACE_ON
		baseColor = baseColor + spec *_SunColor * 4;
#endif

#ifdef LIGHT_FACE_ON
		// 这里没有调用LightFaceColor（），因为指令数已经快接近上限了，这里做一个简单的处理
		fixed4 lightColor = tex2D(_LightFace, i.uvLightface.xy).rgba;
		baseColor.rgb = baseColor.rgb * lightColor.rgb * 2;
#endif	

		baseColor.a = _alpha * i.col.a;
		baseColor.xyz = SimulateFog(i.depth, baseColor, 1);
		return baseColor;
	}
	
	
	
	
ENDCG

SubShader 
{ 
	Tags {"Queue"="Geometry"}
	
	ColorMask RGB
	
	Pass {
	Tags { "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Cull Off
			Fog { Mode off }  
			CGPROGRAM
			
			#pragma target 3.0 
			
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase		
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
			#pragma exclude_renderers d3d11						  			
			ENDCG
	}
}

}

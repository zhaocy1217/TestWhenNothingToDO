// Upgrade NOTE: replaced 'defined RAIN_ON' with 'defined (RAIN_ON)'
// Upgrade NOTE: replaced 'defined SM_3_ON' with 'defined (SM_3_ON)'

// Upgrade NOTE: replaced 'defined RAIN_ON' with 'defined (RAIN_ON)'
// Upgrade NOTE: replaced 'defined SM_3_ON' with 'defined (SM_3_ON)'

#ifndef WEATHER_INCLUDED
#define WEATHER_INCLUDED
#include "UnityShaderVariables.cginc"
#include "UnityLightingCommon.cginc"
#include "LightFace.cginc"

//Lux Rain
sampler2D _WaterNormalTex;
float4 _WaterNormalTex_ST;
float _Lux_RainIntensity;
float4 _Lux_RainRippleWater;
sampler2D _Lux_RainRipples;
float4 _Lux_RainRipples_ST;
sampler2D _Lux_RainWetness;
float4 _Lux_RainWetness_ST;
samplerCUBE _Lux_CubeMap;
float _Lux_CubeMapIntensity;
float _RainRippleIntensity;


#if ((defined (SM_3_ON)) && (defined (RAIN_ON))) 
#define RAIN_PS_UV \
		int showRipple = step(0.7, i.normalDir.y);\
		i.uv.xy += i.normalDir.y * RAIN_PS_UVPerturbation(i.uv.xy, 1);\
		fixed3 rippleNormal = AddWaterRipples(1, i.posWorld, 1, 1);\
		i.uv.xy += showRipple * _Lux_RainRippleWater.y * rippleNormal.xy;

#define RAIN_PS_UV_TILE \
		int showRipple = step(0.7, i.normalDir.y);\
		i.uv.xy += i.normalDir.y * RAIN_PS_UVPerturbation(i.uv.xy, _Lux_RainRippleWater);\
		fixed3 rippleNormal = AddWaterRipples(1, i.posWorld, 1, 1);\
		i.uv.xy += showRipple * _Lux_RainRippleWater.y * rippleNormal.xy;

#define RAIN_PS_Lighting \
		float3 worldNormal = NORMAL_PS_Local2World(rippleNormal, i.normalDir, i.tangentDir, i.bitangentDir, 1);\
		fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.posWorld));\
		fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.posWorld));\
		float3 worldReflect = reflect(-worldViewDir, worldNormal);\
		float3 halfDirection = normalize(worldViewDir + lightDir);\
		tex.rgb = lerp(tex.rgb, texCUBE(_Lux_CubeMap, worldReflect).rgb * _Lux_CubeMapIntensity, 0.3 * _Lux_RainIntensity * showRipple);\
		tex.rgb += pow(max(0, dot(worldNormal, halfDirection)), 100) * 0.2 * _Lux_RainIntensity * showRipple;
#else
#define RAIN_PS_UV 
#define RAIN_PS_UV_TILE
#define RAIN_PS_Lighting
#endif

//	Based on the work of S¨¦bastien Lagarde
//  http://seblagarde.wordpress.com/2013/04/14/water-drop-3b-physically-based-wet-surfaces/
inline float2 ComputeRipple(float4 UV, float CurrentTime, float Weight)
{
	float4 Ripple = tex2D(_Lux_RainRipples, UV);
	// We use multi sampling here in order to improve Sharpness due to the lack of Anisotropic Filtering when using tex2Dlod
	//Ripple += tex2D(_Lux_RainRipples, float4(UV.xy, UV.zw * 0.5));
	//Ripple *= 0.5;

	Ripple.yz = Ripple.yz * 2 - 1; // Decompress Normal
	float DropFrac = frac(Ripple.w + CurrentTime); // Apply time shift
	float TimeFrac = DropFrac - 1.0f + Ripple.x;
	float DropFactor = saturate(0.2f + Weight * 0.8f - DropFrac);
	float FinalFactor = DropFactor * Ripple.x * sin(clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.141592653589793);
	return Ripple.yz * FinalFactor * 0.35f;
}

//	Add animated Ripples to areas where the Water Accumulation is high enough
//  Returns the tweaked and adjusted Ripple Normal
inline float3 AddWaterRipples(float2 i_wetFactor, float3 i_worldPos, float2 lambda, float fadeOutWaterBumps)
{
	float4 _Lux_RippleWindSpeed = float4(0, 0, 0, 0);
	float _Lux_RippleAnimSpeed = 1;

	float4 Weights = _RainRippleIntensity;////_Lux_RainIntensity - float4(0, 0.25, 0.5, 0.75);
	Weights = saturate(Weights * 4);
	float animSpeed = _Time.y * _Lux_RippleAnimSpeed;
	float2 Ripple1 = ComputeRipple (float4(i_worldPos.xz * _Lux_RainRippleWater.x + float2(0.25f, 0.0f) + _Lux_RippleWindSpeed.xy * _Time.y, lambda), animSpeed, Weights.x);
	//float2 Ripple2 = ComputeRipple(float4(i_worldPos.xz * _Lux_RainRippleWater.x + float2(-0.55f, 0.3f) + _Lux_RippleWindSpeed.zw * _Time.y, lambda), animSpeed * 0.71, Weights.y);
	float3 rippleNormal = float3(Weights.x * Ripple1.xy + Weights.y * Ripple1.xy, 1);
	// Blend and fade out Ripples
	return lerp(float3(0, 0, 1), rippleNormal, i_wetFactor.y * i_wetFactor.y * fadeOutWaterBumps);
}
//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#ifdef RAIN_ON
//#define RAIN_TANGENT float4 tangent : TANGENT; float3 normal : NORMAL;
//#else
//#define RAIN_TANGENT
//#endif
//
//#ifdef RAIN_ON
//#define RAIN_COORDS(idx1, idx2, idx3, idx4)\
//		float4 posWorld : TEXCOORD##idx1;\
//		float3 normalDir : TEXCOORD##idx2;\
//		float3 tangentDir : TEXCOORD##idx3;\
//		float3 bitangentDir : TEXCOORD##idx4;
//#elif defined(RAIN_CUBE)
//#define RAIN_COORDS(idx1, idx2, idx3, idx4)\
//		float4 posWorld : TEXCOORD##idx1;\
//		float3 normalDir : TEXCOORD##idx2;
//#else
//	#define RAIN_COORDS(idx1, idx2, idx3, idx4) 
//#endif
//
//#ifdef RAIN_ON
//#define RAIN_VS	\
//		o.posWorld = mul(_Object2World, v.vertex);\
//		o.normalDir = UnityObjectToWorldNormal(v.normal); \
//		o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );\
//		o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
//#elif defined(RAIN_CUBE)
//#define RAIN_VS\
//		o.posWorld = mul(_Object2World, v.vertex);\
//		o.normalDir = UnityObjectToWorldNormal(v.normal);
//#else
//	#define RAIN_VS
//#endif
//
//#ifdef RAIN_ON
//#define RAIN_PS_UV \
//		float3 normalLocal;\
//		i.uv.xy = RAIN_PS_UVPerturbation(i.uv.xy, 1, normalLocal);
//#define RAIN_PS_UV_TILE \
//		float3 normalLocal;\
//		i.uv.xy = RAIN_PS_UVPerturbation(i.uv.xy, _WaterTilling, normalLocal);
//#define RAIN_PS \
//		//tex = RAIN_PS_CUBE(tex, i.posWorld, i.normalDir, normalLocal);
//		//tex = RAIN_PS_Lighting(tex, i.posWorld, i.normalDir, i.tangentDir, i.bitangentDir, normalLocal);
//#else
//#define RAIN_PS_UV
//#define RAIN_PS_UV_TILE
//#define RAIN_PS
//#endif
//	
inline float2 RAIN_PS_UVPerturbation(float2 uv, float4 tile)
{
#ifdef RAIN_ON
	//return 0;
	uv.xy *= _Lux_RainRippleWater.z;
	float speed = (_Time.g*(1*0.03));
	float2 uv1 = (uv.xy * 1 + speed*float2(0.5, -0.2));
	float3 normal1 = UnpackNormal(tex2D(_WaterNormalTex, TRANSFORM_TEX(uv1, _WaterNormalTex)));
	float2 uv2 = (uv.xy * 1 + speed*float2(0.2, 1));
	float3 normal2 = UnpackNormal(tex2D(_WaterNormalTex, TRANSFORM_TEX(uv2, _WaterNormalTex)));
	float3 normal = (normal1.rgb + normal2.rgb);
	return (normal.xy - 0.5)*(_Lux_RainIntensity *0.003 * _Lux_RainRippleWater.w);
#else
	return 0;
#endif
}
//inline float2 RAIN_PS_UVPerturbation (float2 uv, float tile, out float3 normal)
//{
//#ifdef RAIN_ON
//	float speed = (_Time.g*(_WaterSpeed*0.03));
//	float2 uv1 = (uv.xy * 1 + speed*float2(0.5, -0.2));
//	float3 normal1 = UnpackNormal(tex2D(_WaterNormalTex, TRANSFORM_TEX(uv1, _WaterNormalTex)));
//	float2 uv2 = (uv.xy * 1 + speed*float2(0.2, 1));
//	float3 normal2 = UnpackNormal(tex2D(_WaterNormalTex, TRANSFORM_TEX(uv2, _WaterNormalTex)));
//	normal = (normal1.rgb+normal2.rgb);
//	//normal = normalize(normal);
//	return (((normal.xy-0.5)*(_RainIntensity*0.02 * tile))+uv.xy);
//#else
//	normal = 0;
//	return uv;
//#endif
//}
//
//
//inline fixed4 RAIN_PS_Lighting(float4 texRGBA, float4 posWorld, float3 normalDir, float3 tangentDir, float3 bitangentDir, float3 normalLocal)
//{
//#ifdef RAIN_ON
//	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
//
//	//Light Dir
//	float3 lightDirection = normalize(_WeatherLightDirection.xyz);
//	//float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
//
//	float3 halfDirection = normalize(viewDirection+lightDirection);
//	float3x3 tangentTransform = float3x3( tangentDir, bitangentDir, normalDir);
//	float3 normalDirection = normalize(mul( normalLocal, tangentTransform ));
//
//	float3 waterNormal = lerp(normalDir,normalDirection, _RainIntensity*0.5);
//	float3 emissive = texRGBA.rgb;
//	float diff = max(0, dot(waterNormal, lightDirection));
//	float3 lightCol = _RainIntensity * diff * pow(max(0,dot(waterNormal,halfDirection)),exp2(lerp(1,11, _RainGloss)))*_RainSpecColor.rgb * 2; // Combine
//	float3 finalColor = emissive + lightCol;
//
//	return fixed4(finalColor,1);
//#else
//	return texRGBA;
//#endif
//}
//
//inline fixed4 RAIN_PS_Simple(float4 texRGBA, float4 uv, float3 normalLocal )
//{
//#ifdef RAIN_ON
//	float2 reflectUV = uv;
//	reflectUV = normalLocal.rg*(_RainIntensity*0.02) + reflectUV;
//	float4 reflectTex = tex2D(_RainReflectTex, reflectUV.xy);
//	float3 emissive = texRGBA.rgb + reflectTex * 0.5 * _RainIntensity;
//	float3 finalColor = emissive;
//	return fixed4(finalColor, 1);
//#else
//	return texRGBA;
//#endif
//}
//
//inline fixed4 RAIN_PS_CUBE(float4 texRGBA, float4 posWorld, float3 normalDir, float3 normalLocal )
//{
//#ifdef RAIN_ON
//	normalDir += normalLocal.xyz * _RainIntensity * 0.1;
//	normalDir = normalize(normalDir);
//	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
//	float3 viewReflectDirection = reflect(-viewDirection, normalDir);
//	float3 emissive = texRGBA.rgb* 0.7 + texCUBE(_RainCubemap, viewReflectDirection).rgb* 0.3;
//	float3 finalColor = lerp(texRGBA.rgb, emissive, _RainIntensity);
//	return fixed4(finalColor, 1);
//#else
//	return texRGBA;
//#endif
//}
//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//SNOW
float _Lux_SnowIntensity;
fixed4 _SnowColor;
sampler2D _Lux_SnowAlbedo;
sampler2D _Lux_SnowNormal;
float4 _Lux_SnowAlbedoNormal;

#if ((defined (SM_3_ON)) && (defined (SNOW_ON)))
#define SNOW_PS_BUMP \
		half4 snowAlbedoSmoothness = tex2D(_Lux_SnowAlbedo, i.uv.xy * _Lux_SnowAlbedoNormal.xy);\
		half snowAmount = 1 * max(0, dot(normalDirection, float3(0, 1, 0)));\
		tex.rgb = lerp(tex.rgb, lerp(snowAlbedoSmoothness.rgb * 0.45 * lightFaceCol, lightFaceCol* snowAlbedoSmoothness.rgb * 0.7 * _SnowColor, _GameTime), saturate(_Lux_SnowIntensity* snowAmount * 1));\
		float3 snowNormalDirection = normalDirection;\
		snowNormalDirection = NORMAL_PS_Normal(_Lux_SnowNormal, i.uv.xy * _Lux_SnowAlbedoNormal.zw, snowNormalDirection, i.tangentDir, i.bitangentDir, _Lux_SnowIntensity);\
		fixed3 snowLightDir = UnityWorldSpaceLightDir(i.posWorld.xyz);\
		float snowDiff = dot(snowLightDir, snowNormalDirection) * 0.5 + 0.5;\
		tex.rgb += tex.rgb * float3(1, 1, 1) * snowDiff * 0.1;

#define SNOW_PS \
		float luminance = Luminance(tex.rgb);\
		half4 snowAlbedoSmoothness = tex2D(_Lux_SnowAlbedo, i.uv.xy * _Lux_SnowAlbedoNormal.xy);\
		half snowAmount = 2 * max(0, dot(i.normalDir, float3(0, 1, 0)));\
		tex.rgb = lerp(tex.rgb, lerp(snowAlbedoSmoothness.rgb * 0.45 * lightFaceCol, lightFaceCol* snowAlbedoSmoothness.rgb * 0.7 * _SnowColor, _GameTime), saturate(_Lux_SnowIntensity* snowAmount* 1));\
		float3 snowNormalDirection = normalize(i.normalDir);\
		snowNormalDirection = NORMAL_PS_Normal(_Lux_SnowNormal, i.uv.xy * _Lux_SnowAlbedoNormal.zw, snowNormalDirection, i.tangentDir, i.bitangentDir, _Lux_SnowIntensity);\
		fixed3 snowLightDir = UnityWorldSpaceLightDir(i.posWorld.xyz);\
		float snowDiff = dot(snowLightDir, snowNormalDirection) * 0.5 + 0.5;\
		tex.rgb += tex.rgb * float3(1, 1, 1) * snowDiff * 0.1;

#define SNOW_PS_TREE \
		float luminance = Luminance(tex.rgb);\
		half4 snowAlbedoSmoothness = tex2D(_Lux_SnowAlbedo, i.uv.xy * _Lux_SnowAlbedoNormal.xy);\
		snowAlbedoSmoothness.rgb = lerp(snowAlbedoSmoothness.rgb * 0.45 * lightFaceCol, lightFaceCol* snowAlbedoSmoothness.rgb * 0.7 * _SnowColor, _GameTime);\
		snowAlbedoSmoothness.rgb = lerp(tex.rgb, snowAlbedoSmoothness.rgb, luminance+lerp(0.5,0.3,_GameTime));\
		tex.rgb = lerp(tex.rgb, snowAlbedoSmoothness.rgb, _Lux_SnowIntensity );\
		float3 snowNormalDirection = normalize(i.normalDir);\
		snowNormalDirection = NORMAL_PS_Normal(_Lux_SnowNormal, i.uv.xy * _Lux_SnowAlbedoNormal.zw, snowNormalDirection, i.tangentDir, i.bitangentDir, _Lux_SnowIntensity);\
		fixed3 snowLightDir = UnityWorldSpaceLightDir(i.posWorld.xyz);\
		float snowDiff = dot(snowLightDir, snowNormalDirection) * 0.5 + 0.5;\
		tex.rgb += tex.rgb * float3(1, 1, 1) * snowDiff * 0.1;

#else
	#define SNOW_PS_BUMP
	#define SNOW_PS
	#define SNOW_PS_TREE
#endif

//inline fixed3 SNOW_PS_func(fixed3 texRGB)
//{
//	fixed4 _SnowColor = fixed4(0.8897059,0.9361055,1,1);
//	fixed _SnowFallOff = -3;
//	
//    fixed snowRatio = saturate(1.0 - texRGB.r * (1.0 / _SnowIntensity));
//    return lerp(texRGB.rgb, pow(lerp(texRGB.rgb, _SnowColor.rgb, snowRatio), _SnowFallOff), snowRatio);
//}

#endif
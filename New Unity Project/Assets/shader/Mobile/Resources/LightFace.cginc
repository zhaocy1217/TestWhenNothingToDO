// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'defined NORMAL_ON' with 'defined (NORMAL_ON)'
// Upgrade NOTE: replaced 'defined RAIN_ON' with 'defined (RAIN_ON)'
// Upgrade NOTE: replaced 'defined SM_3_ON' with 'defined (SM_3_ON)'
#ifndef LIGHTFACE_INCLUDED
#define LIGHTFACE_INCLUDED
#include "UnityCG.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityLightingCommon.cginc"


float4x4 _texViewProj;
sampler2D _LightFace;
float4x4 _shadowViewProj;

fixed4 _FogColor;
fixed _FogIntensity;
float _FogStart;
float _FogEnd;
float _FogHeight;
fixed _LightFaceScale;
fixed _RoleLightAdjust;
fixed4 _ClearColor;
float _GameTime;
float4 _PointLight;
float4 _SpecLight;

#ifdef LOD_FADE_CROSSFADE
#define LOD_DITHER_COORDS(idx) UNITY_DITHER_CROSSFADE_COORDS_IDX(idx)
#define LOD_DITHER_VS UNITY_TRANSFER_DITHER_CROSSFADE_HPOS(o, o.pos);
#define LOD_DITHER_PS UNITY_APPLY_DITHER_CROSSFADE(i);
#define LOD_ALPHA_PS tex.a *= unity_LODFade.x;
#else
#define LOD_DITHER_COORDS(idx)
#define LOD_DITHER_VS
#define LOD_DITHER_PS
#define LOD_ALPHA_PS
#endif

#ifdef CROSSFADE_ON
uniform float _Crossfade;
uniform float4 _CrossfadeAuto;
uniform sampler2D _DitherMaskLOD2D;
//coords
#define CROSSFADE_DITHER_COORDS(idx) half3 ditherScreenPos : TEXCOORD##idx;
//vs
#define CROSSFADE_DITHER_VS o.ditherScreenPos = CROSSFADE_ComputeDitherScreenPos(o.pos);
//ps
#define CROSSFADE_DITHER_PS CROSSFADE_ApplyDitherCrossFade(i.ditherScreenPos);
#define CROSSFADE_ALPHA_PS tex.a = _Crossfade;
//auto
#define CROSSFADE_AUTO_ALPHA_PS tex.a = lerp(_CrossfadeAuto.x, _CrossfadeAuto.y, saturate(_Time.y - _CrossfadeAuto.z));
#define CROSSFADE_AUTO_DITHER_PS CROSSFADE_AUTO_ApplyDitherCrossFade(i.ditherScreenPos);

half3 CROSSFADE_ComputeDitherScreenPos(float4 hPos)
{
	half3 screenPos = ComputeScreenPos(hPos).xyw;
	screenPos.xy *= _ScreenParams.xy * 0.25;
	return screenPos;
}

void CROSSFADE_ApplyDitherCrossFade(half3 ditherScreenPos)
{
	half2 projUV = ditherScreenPos.xy / ditherScreenPos.z;
	projUV.y = frac(projUV.y) * 0.0625 /* 1/16 */ + _Crossfade * 0.1; // quantized lod fade by 16 levels
	clip(tex2D(_DitherMaskLOD2D, projUV).a - 0.5);
}
void CROSSFADE_AUTO_ApplyDitherCrossFade(half3 ditherScreenPos)
{
	half tf = saturate(_Time.y - _CrossfadeAuto.z);
	half fade = lerp(_CrossfadeAuto.x, _CrossfadeAuto.y, tf) * 0.9;
	half2 projUV = ditherScreenPos.xy / ditherScreenPos.z;
	projUV.y = frac(projUV.y) * 0.0625 /* 1/16 */ + fade; // quantized lod fade by 16 levels
	clip(tex2D(_DitherMaskLOD2D, projUV).a - 0.5);
}
#else
#define CROSSFADE_DITHER_COORDS(idx)
#define CROSSFADE_DITHER_VS
#define CROSSFADE_DITHER_PS
#define CROSSFADE_ALPHA_PS
#define CROSSFADE_AUTO_DITHER_PS
#endif

#ifdef LIGHTMAP_ON
#define LIGHTMAP_COORDS(idx1)		float4 texcoord1 : TEXCOORD##idx1;
#define LIGHTMAP_VS					o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#define LIGHTMAP_PS					fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv.zw));
#else
#define LIGHTMAP_COORDS(idx1)		
#define LIGHTMAP_VS					o.uv.zw = 0;
#define LIGHTMAP_PS					fixed3 lm = fixed3(1,1,1);
#endif

#ifdef LIGHT_FACE_ON
#define LIGHTFACE_COORDS(idx1)		half4 uvLightFace : TEXCOORD##idx1;
#define LIGHTFACE_VS				o.uvLightFace.xy = LightFaceUV(worldPos); o.uvLightFace.zw = 0;
#define LIGHTFACE_VS_SHADOW			o.uvLightFace.zw = LightFaceShadowUV(worldPos);

#define LIGHTFACE_PS				fixed3 lightFaceCol = GetLightFaceColor( i.uvLightFace.xy, 1);\
									tex.rgb *= lightFaceCol * lm;

#define LIGHTFACE_PS_ROLE			LightFaceColorRole(tex, i.uvLightFace.xy, 1, 1);
#define LIGHTFACE_PS_ROLE_F			LightFaceColorRole(tex, i.uvLightFace.xy, 1, _LFFactor);

#define LIGHTFACE_PS_SHADOW			fixed3 lightFaceCol = GetLightFaceColor( i.uvLightFace.xy, 1);\
									tex.rgb *= lightFaceCol;\
									tex.rgb *= lm;\
									fixed3 shadowCol = GetLightFaceShadow(i.uvLightFace.zw);\
									tex.rgb *= shadowCol;
#else
#define LIGHTFACE_COORDS(idx1)
#define LIGHTFACE_VS
#define LIGHTFACE_VS_SHADOW

#define LIGHTFACE_PS				LightFaceColor(tex, 0, lm, 1);\
									fixed3 lightFaceCol = 1;

#define LIGHTFACE_PS_ROLE			LightFaceColorRole(tex, 0, 1, 1);
#define LIGHTFACE_PS_ROLE_F			LightFaceColorRole(tex, 0, 1, 1);

#define LIGHTFACE_PS_SHADOW			LightFaceColorReceiveShadow(tex, 0, 0, lm, 1); \
									fixed3 lightFaceCol = 1;
#endif

#define FOG_COORDS(idx1) half4 fogDepth : TEXCOORD##idx1;
#define FOG_VS o.fogDepth = SimulateFogVS(o.pos.xyz, worldPos.xyz);
#define FOG_PS tex.xyz = SimulateFog(i.fogDepth, tex, 1);

#define WORLD_POS float4 worldPos = mul(unity_ObjectToWorld, v.vertex);


#if ((defined (SM_3_ON)) && ((defined (NORMAL_ON)) || (defined (RAIN_ON)) || (defined (SNOW_ON))))
#define NORMAL_COORDS(idx1, idx2, idx3, idx4)\
		float4 posWorld : TEXCOORD##idx1;\
		float3 normalDir : TEXCOORD##idx2;\
		float3 tangentDir : TEXCOORD##idx3;\
		float3 bitangentDir : TEXCOORD##idx4;
#define NORMAL_VS\
		o.posWorld = worldPos;\
		o.normalDir = UnityObjectToWorldNormal(v.normal); \
		o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );\
		o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
#else
#define NORMAL_COORDS(idx1, idx2, idx3, idx4) \
		float4 posWorld : TEXCOORD##idx1; \
		float3 normalDir : TEXCOORD##idx2;
#define NORMAL_VS \
		o.posWorld = worldPos; \
		o.normalDir = UnityObjectToWorldNormal(v.normal);
#endif

inline float4 SimulateFogVS(float3 screenPos, float3 worldPos)
{
	float4 o;
	o.x = screenPos.z * (1 - clamp(worldPos.y / _FogHeight, 0, 1));
	o.y = 0;
	o.zw = worldPos.xz / 200;
	return o;
}

inline fixed3 SimulateFog(float4 depth, fixed4 col, half fogScale)
{
	float fogFactor = (_FogEnd - abs(depth.x*fogScale)) / (_FogEnd - _FogStart);
	fogFactor = (1 - fogFactor)*_FogIntensity;
	fogFactor = clamp(fogFactor, 0.0, 1.0);
	fixed3 afterFog = _FogColor.rgb * (0.2 + _GameTime * 0.8) * fogFactor + (1 - fogFactor) * col.rgb;
	return afterFog;
}

inline half4 LightFaceUV(float4 worldPos)
{
	float4 viewPos = mul(_texViewProj, float4(worldPos.xyz, 1));
	viewPos = viewPos / viewPos.w;
	viewPos.xy = (viewPos.xy + fixed2(1.0f, 1.0f)) / 2.0f;
	return viewPos;
}

inline half4 LightFaceShadowUV(float4 worldPos)
{
	float4 viewPos = mul(_shadowViewProj, float4(worldPos.xyz, 1));
	viewPos = viewPos / viewPos.w;
	viewPos.xy = (viewPos.xy + fixed2(1.0f, 1.0f)) / 2.0f;
	return viewPos;
}

inline void LightFaceAlpha(inout fixed4 baseColor, float lightFactor)
{
	float brightness = (0.2990*baseColor.r + 0.587*baseColor.g + 0.114*baseColor.b);
	baseColor.a = ((brightness)* lightFactor);
}

inline fixed3 GetLightFaceColor(fixed2 uvLight, float lightFactor)
{
#ifdef LIGHT_FACE_ON	
	fixed4 lightColor = tex2D(_LightFace, uvLight).rgba;
#else
	fixed4 lightColor = _ClearColor + 0.5;
#endif
	return lightColor.rgb * (1 + _LightFaceScale);
}

inline fixed3 GetLightFaceShadow(fixed2 uvShadow)
{
	fixed4 shadowColor = tex2D(_LightFace, uvShadow).rgba;
	return shadowColor.a;
}

inline void LightFaceColor(inout fixed4 baseColor, fixed2 uvLight, fixed3 lightmap, float lightFactor)
{
#ifdef LIGHT_FACE_ON	
	fixed4 lightColor = tex2D(_LightFace, uvLight).rgba;
#else
	fixed4 lightColor = _ClearColor + 0.5;
#endif
	baseColor.rgb = baseColor.rgb * lightColor * (1 + _LightFaceScale) * lightmap;
}

inline void LightFaceColorRole(inout fixed4 baseColor, fixed2 uvLight, fixed3 lightmap, float lightFactor)
{
#ifdef LIGHT_FACE_ON	
	fixed4 lightColor = lerp(_ClearColor + 0.5, tex2D(_LightFace, uvLight).rgba, lightFactor);
#else
	fixed4 lightColor = _ClearColor + 0.5;
#endif				
	baseColor.rgb = baseColor.rgb * (1 + _RoleLightAdjust) * lightColor * (1 + _LightFaceScale) *lightmap;
}

inline void LightFaceShadow(inout fixed4 baseColor, fixed2 uvShadow)
{
	fixed4 shadowColor = tex2D(_LightFace, uvShadow).rgba;
	baseColor.rgb = baseColor.rgb * shadowColor.a;
}

inline void LightFaceColorReceiveShadow(inout fixed4 baseColor, fixed2 uvLight, fixed2 uvShadow, fixed3 lightmap, float lightFactor)
{
	LightFaceColor(baseColor, uvLight, lightmap, lightFactor);
#ifdef LIGHT_FACE_ON
	LightFaceShadow(baseColor, uvShadow);
#endif
}

inline void LightFaceColorWithNormal(inout fixed4 baseColor, fixed2 uvLight, fixed3 lightmap, float lightFactor)
{
}

inline void LightFaceColorReceiveShadowWithNormal(inout fixed4 baseColor, fixed2 uvLight, fixed2 uvShadow, fixed3 lightmap, float lightFactor)
{
}

inline fixed4 NORMAL_PS_Lighting(inout fixed4 tex, float4 posWorld, float3 normalDir, float3 tangentDir, float3 bitangentDir, float3 normalLocal, float bumpIntensity, float gloss, fixed3 specColor)
{
#ifdef NORMAL_ON
	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
	float3 lightDirection = normalize(_SpecLight.xyz - posWorld.xyz);
	float3 halfDirection = normalize(viewDirection + lightDirection);
	float3x3 tangentTransform = float3x3(tangentDir, bitangentDir, normalDir);
	float3 normalDirection = normalize(mul(normalLocal, tangentTransform));

	float3 normal = lerp(normalDir, normalDirection, bumpIntensity);
	float3 emissive = tex.rgb * 0.5;
	float diff = max(0.4, dot(normal, lightDirection));
	float3 lightCol = tex.rgb * diff + diff * pow(max(0, dot(normal, halfDirection)), exp2(lerp(1, 11, gloss)))*specColor.rgb * 2; // Combine
	tex.rgb = emissive + lightCol;

	return tex;
#else
	return tex;
#endif
}

inline float3 NORMAL_PS_Local2World(float3 normalLocal, float3 normalDir, float3 tangentDir, float3 bitangentDir, float bumpIntensity)
{
	float3x3 tangentTransform = float3x3(tangentDir, bitangentDir, normalDir);
	float3 normalDirection = normalize(mul(normalLocal, tangentTransform));
	float3 normal = lerp(normalDir, normalDirection, bumpIntensity);
	return normal;
}

inline float3 NORMAL_PS_Normal(sampler2D bumpTex, float2 uv, float3 normalDir, float3 tangentDir, float3 bitangentDir, float bumpIntensity)
{
	float3 normalLocal = UnpackNormal(tex2D(bumpTex, uv));
	return NORMAL_PS_Local2World(normalLocal, normalDir, tangentDir, bitangentDir, bumpIntensity);
}



#endif
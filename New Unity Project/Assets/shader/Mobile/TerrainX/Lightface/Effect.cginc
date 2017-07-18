#ifndef EFFECT_INCLUDED
#define EFFECT_INCLUDED
#include "UnityShaderVariables.cginc"
#include "UnityLightingCommon.cginc"

//FLOW

float4 _FlowFromPos;
float _FlowBeginTime;
float _FlowSpeed;
float _FlowWidth;

#ifdef FOLW_ON
	#define FLOW_COORDS(idx1) fixed flow: TEXCOORD##idx1;
#else
	#define FLOW_COORDS(idx1)
#endif

#ifdef FOLW_ON
	#define FLOW_VS o.flow = FLOW_VS_Func(worldPos);
#else
	#define FLOW_VS
#endif

inline fixed FLOW_VS_Func(float3 worldPos)
{
	float flowTime = max(_Time.y - _FlowBeginTime, 0);
	float flowDistance = flowTime * _FlowSpeed;
	float dis = distance(worldPos, _FlowFromPos.xyz);
	return step(abs(dis - flowDistance), _FlowWidth);
}


#ifdef FOLW_ON
	#define FLOW_PS tex.rgb = tex.rgb + i.flow;
#else
	#define FLOW_PS
#endif

//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
uniform sampler2D _DissolveNoise;
uniform float4 _DissolveNoise_ST;
uniform float4 _DissolveColor;
uniform float _DissolveAmount;

#ifdef DISSOLVE_ON
#define DISSOLVE_PS_TEX \
	float startAmount = 0.1;\
	float clipVal = tex2D(_DissolveNoise, TRANSFORM_TEX(i.uv, _DissolveNoise)).r;\
	float clipAmount = clipVal - _DissolveAmount;\
	if(clipAmount < startAmount)\
		tex.rgb *= _DissolveColor * (clipAmount / startAmount);\
	clip(clipAmount);

#define DISSOLVE_PS_ALPHA \
	float startAmount = 0.1;\
	fixed clipVal = Luminance(tex.rgb);\
	float clipAmount = clipVal - _DissolveAmount;\
	if(clipAmount < startAmount)\
		tex.rgb *= _DissolveColor * (clipAmount / startAmount);\
	tex.a = (clipVal - _DissolveAmount);
#endif
//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=




#endif

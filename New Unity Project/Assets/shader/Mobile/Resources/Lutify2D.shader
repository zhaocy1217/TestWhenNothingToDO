// Lutify - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

Shader "Hidden/Lutify 2D"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	CGINCLUDE

		#pragma vertex vert_img
		#pragma fragment frag_2d
		#pragma fragmentoption ARB_precision_hint_fastest

	ENDCG

	SubShader
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		// (0) - Gamma
		Pass
		{			
			CGPROGRAM
				#include "Lutify.cginc"
			ENDCG
		}

		// (1) - Linear
		Pass
		{			
			CGPROGRAM
				#define LUTIFY_LINEAR
				#include "Lutify.cginc"
			ENDCG
		}
	}

	FallBack off
}

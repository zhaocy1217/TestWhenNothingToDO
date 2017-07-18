// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Cloud/ShadowCaster" {
    Properties {
    	_shadowAlpha ("Shadow Transparency", Float) = 0.75
    	_shadowSharpness("Shadow Sharpness", Float) = 1
        _PerlinNormalMap ("PerlinNormalMap", 2D) = "white" {}
        _Tiling ("Tiling", Float ) = 3000
        _Density ("Density", Float ) = -0.25
        _Alpha ("Alpha", Float ) = 5
        _AlphaCut ("AlphaCut", Float ) = 0.01
        _TimeMult ("Speed", Float ) = 0.1
        _TimeMultSecondLayer ("SpeedSecondLayer", Float ) = 4
        _WindDirection ("WindDirection", Vector) = (1,0,0,0)
        _ObjectSpaceMapping ("Object Space Mapping", Float) = 0
        
        _Dithering ("Dithering", Float) = 1280
    }
    SubShader {
    	Tags { 
    		"RenderType"="Opaque"
    	}
        LOD 200
		Pass {
            Name "ShadowCaster"
			Tags { 
				"LightMode" = "ShadowCaster" 
			}
			//Offset 1, 1
			Cull Front
			//ZWrite On ZTest LEqual
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #define UNITY_PASS_SHADOWCASTER
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x
            #pragma target 3.0
            //#pragma glsl
            #pragma exclude_renderers gles
            
            #if !((SHADER_TARGET < 30) || defined (SHADER_API_MOBILE) || defined(SHADER_API_D3D11_9X) || defined (SHADER_API_PSP2) || defined (SHADER_API_PSM))
				#define UNITY_STANDARD_USE_DITHER_MASK 1
			#endif
            
            sampler3D	_DitherMaskLOD;
            fixed _shadowAlpha;
            fixed _shadowSharpness;
            fixed _ObjectSpaceMapping;
            float4 _TimeEditor;
            fixed _Tiling;
            fixed _Density;
            fixed _Alpha;
            fixed _AlphaCut;
            fixed _TimeMult;
            fixed _TimeMultSecondLayer;
            fixed4 _WindDirection;
            sampler2D _PerlinNormalMap;
            fixed _Dithering;
			
            struct VertexInput {
                float4 vertex   : POSITION;
                float3 normal	: NORMAL;
				float4 vertexColor : COLOR;
            };
            struct VertexOutput {
            	float4 dPos : POSITION1;
                float4 posWorld : TEXCOORD1;
                float4 vertexColor : COLOR;
                V2F_SHADOW_CASTER;
            };
			
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                //o.dPos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.dPos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz,1))*_Dithering;

                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : Color {
                
                fixed4 objPos = mul(unity_ObjectToWorld, float4(0,0,0,1));
                fixed2 worldUV = (i.posWorld.xz-objPos.xz*_ObjectSpaceMapping) / _Tiling;
                
                fixed2 baseAnimation = (_Time.g + _TimeEditor.g) * 0.001 * _WindDirection.rb;
                fixed2 newUV = worldUV + (baseAnimation * _TimeMult);
				fixed2 newUV2 = worldUV + (baseAnimation * _TimeMultSecondLayer) + fixed2(0.0, 0.5);
            	fixed4 cloudTexture = tex2D(_PerlinNormalMap, newUV);
            	fixed4 cloudTexture2 = tex2D(_PerlinNormalMap, newUV2);
            	fixed baseMorph = ((saturate(cloudTexture.a + _Density) * i.vertexColor.a) - cloudTexture2.a);
            	fixed cloudMorph = baseMorph * _Alpha;
            	fixed cloudAlphaCut = cloudMorph -_AlphaCut;
            	
            	#if UNITY_STANDARD_USE_DITHER_MASK
            		fixed alpha = saturate(cloudAlphaCut*exp(_shadowSharpness))*_shadowAlpha;
            		
            		fixed alphaRef = tex3D(_DitherMaskLOD, float3(i.dPos.rg,alpha*0.9375)).a;
            		clip (alphaRef - 0.01);
            	#else
            		clip(saturate(ceil(cloudAlphaCut)) - 0.5);
            	#endif

                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    //FallBack "Diffuse"
}

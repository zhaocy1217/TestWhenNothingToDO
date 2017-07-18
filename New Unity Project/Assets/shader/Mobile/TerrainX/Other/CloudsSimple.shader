// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Cloud/Standard" {
    Properties {
        _BaseColor ("BaseColor", Color) = (1,1,1,0.5)
        _IndirectContribution("Indirect Lighting", Float) = 1
        _Normalized ("Normalized", Float ) = 1			//1 = true, 2 = false
        _Shading ("Shading Color", Color) = (0, 0, 0, 0.5)
        _DepthColor ("Depth Intensity", Float ) = 0.5
        
        _PerlinNormalMap ("PerlinNormalMap", 2D) = "white" {}
        _Tiling ("Tiling", Float ) = 4000
        
        _Density ("Density", Float ) = -0.25
        _Alpha ("Alpha", Float ) = 5
        _AlphaCut ("AlphaCut", Float ) = 0.01
        
        _TimeMult ("Speed", Float ) = 0.1
        _TimeMultSecondLayer ("SpeedSecondLayer", Float ) = 4
        _WindDirection ("WindDirection", Vector) = (1,0,0,0)
        
        _DepthBlendMult ("Depth Blend Intensity", Range (0, 3)) = 2
        _EdgeBlend ("Edge Blend", Range (0, 10)) = 2
        //X, Y, Z are used to inverse normals at a specific slice pos to render the fake upper side of the cloud like he is supposed to be if geometry was double sided
        //W is used as the exact slice wich should be rendered when casting shadows
        [HideInInspector]_CloudNormalsDirection ("_CloudNormalsDirection", Vector) = (1, 1, -1, 0)
        _ObjectSpaceMapping ("Object Space Mapping", Float) = 0
        
        _shadowAlpha ("Shadow Transparency", Float) = 0.75
        _shadowSharpness ("Shadow Sharpness", Float) = 1
        
        _Dithering ("Dithering", Float) = 1280
    }
    SubShader {
        Tags {
        	//"IgnoreProjector"="True"
        	"Queue" = "Transparent-1"
        	"RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            //On = more accurate blending between different alpha objects but slower to render
            ZWrite On
            //ZTest Always
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            //#define UNITY_PASS_FORWARDBASE
            
            #include "UnityCG.cginc"
            //#pragma multi_compile_fwdbase
            #pragma target 2.0
            
            
            #define _GLOSSYENV 1
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            //#include "UnityPBSLighting.cginc"
            //#include "UnityStandardBRDF.cginc"
            //#pragma multi_compile_fwdbase_fullshadows

			//#pragma enable_d3d11_debug_symbols
            
            //#pragma multi_compile __ CV_DEPTHBLEND_ON
            #pragma multi_compile CV_LIGHTING_ON __
            #pragma multi_compile __ CV_HORIZONBLEND_ON
            
            
            uniform fixed4 _TimeEditor;
            uniform fixed _TimeMult;
            uniform fixed _TimeMultSecondLayer;
            uniform fixed _Tiling;
            fixed _ObjectSpaceMapping;
            fixed _Normalized;
            fixed _IndirectContribution;
            uniform fixed _Density;
            uniform fixed4 _BaseColor;
            uniform fixed _Alpha;
            uniform fixed _AlphaCut;
            uniform fixed _DepthColor;
            uniform sampler2D _PerlinNormalMap;uniform float4 _PerlinNormalMap_ST;
            uniform fixed4 _WindDirection;
            #ifdef CV_DEPTHBLEND_ON
            	uniform sampler2D _CameraDepthTexture;
            	uniform fixed _DepthBlendMult;
            #endif
            #ifdef CV_HORIZONBLEND_ON
            	uniform fixed _EdgeBlend;
            #endif
            //uniform half4 _LightColor0;
            uniform fixed4 _Shading;
            uniform fixed4 _CloudNormalsDirection;
            
            
            
            struct VertexInput {
                half4 vertex : POSITION;
                half4 vertexColor : COLOR;
                half3 normal : NORMAL;

                #ifdef CV_LIGHTING_ON
                	half4 tangent : TANGENT;
                #endif

            	float2 texcoord0 : TEXCOORD0;
            };
            
            
            
            struct VertexOutput {
                half4 pos : SV_POSITION;
                half4 posWorld : TEXCOORD0;
                half3 normalDir : TEXCOORD1;
                half4 vertexColor : COLOR;

                #ifdef CV_LIGHTING_ON
                	half3 tangentDir : TEXCOORD2;
                	half3 binormalDir : TEXCOORD3;
                	//LIGHTING_COORDS(6,7)
                #endif

                #ifdef CV_DEPTHBLEND_ON
                	half4 projPos : TEXCOORD4;
                #endif
            	float2 uv0 : TEXCOORD5;
            };
            
            
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.normalDir = mul(half4(v.normal,0), unity_WorldToObject).xyz;
                o.vertexColor = v.vertexColor;
                
                #ifdef CV_LIGHTING_ON
                	o.tangentDir = normalize( mul( unity_ObjectToWorld, half4( v.tangent.xyz, 0.0 ) ).xyz );
                	o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                #endif

                #ifdef CV_DEPTHBLEND_ON
					//o.projPos = ComputeScreenPos (o.pos);
                	//COMPUTE_EYEDEPTH(o.projPos.z);
                #endif

                o.uv0 = v.texcoord0;
                return o;
            }
            
            
            
            fixed4 frag(VertexOutput i) : COLOR {
            	
            	
            	//-------------------\\
            	//Animation and alpha||
                //-------------------//
                fixed2 baseAnimation =  (_Time.g + _TimeEditor.g) * 0.001 * _WindDirection.rb + _PerlinNormalMap_ST.ba;
                //fixed4 objPos = mul(_Object2World, float4(0,0,0,1));
                //fixed2 worldUV = (i.posWorld.xz-objPos.xz*_ObjectSpaceMapping) / _Tiling;
                fixed2 worldUV = lerp(i.posWorld.xz / _Tiling * _PerlinNormalMap_ST.rg, i.uv0/(_PerlinNormalMap_ST.rg * _Tiling*0.0005), _ObjectSpaceMapping);
                
                fixed2 newUV = worldUV + (baseAnimation * _TimeMult);
				fixed2 newUV2 = worldUV + (baseAnimation * _TimeMultSecondLayer) + fixed2(0.0, 0.5);
                fixed4 cloudTexture = tex2D(_PerlinNormalMap, newUV);
				fixed4 cloudTexture2 = tex2D(_PerlinNormalMap, newUV2);

                fixed baseMorph =  - cloudTexture2.a;
                fixed3 baseMorphNormals = - (cloudTexture2.rgb*2-1);


                #ifdef CV_DEPTHBLEND_ON
                	//float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                	//float partZ = max(0,i.projPos.z - _ProjectionParams.g);
					//fixed cloudMorph = baseMorph * _Alpha * (saturate((sceneZ-partZ)/partZ)*_DepthBlendMult);
					fixed cloudMorph = baseMorph * _Alpha;
                #else
                	fixed cloudMorph = baseMorph * _Alpha;
                #endif
                
                #ifdef CV_HORIZONBLEND_ON
        			cloudMorph *= saturate(1-length(i.uv0*2.0+-1.0));
        		#endif
                //fixed cloudAlphaCut = cloudMorph -_AlphaCut;

                
                //clip(saturate(ceil(cloudAlphaCut)) - 0.5);

                //-------------------\\
            	//Colors and lighting||
                //-------------------//

                fixed fakeDepth = saturate(_DepthColor+(i.vertexColor.b * _CloudNormalsDirection.g+1)/2);
                #ifdef CV_LIGHTING_ON
                	half3x3 tangentTransform = half3x3( i.tangentDir, i.binormalDir, i.normalDir);
                	half3 normalLocal = baseMorphNormals * _CloudNormalsDirection.rgb;
                	half3 normalDirection =  normalize(mul(normalLocal, tangentTransform));
					
					// New GI
					float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
					//float attenuation = LIGHT_ATTENUATION(i);
					// End GI
					
					
					
					fixed NdotL = dot(_WorldSpaceLightPos0.xyz, normalDirection);
					// Should we normalize ?	output 0.5 + 0.5*NdotL 	if _Normalized = 1
					//							output 0 + 1*NdotL 		if _Normalized = 2
					// Lerping between two dotProd might look cleaner but it'll run slower
					NdotL = (1+(1-_Normalized)*0.5)+(_Normalized*0.5)*NdotL;
					NdotL = max(0, NdotL);
					
                	fixed shadingRamp = (1.0 - _Shading.a*( 1.0 - NdotL )) * fakeDepth;
                #else
                	fixed shadingRamp = fakeDepth;
                #endif
                
                cloudMorph = saturate(cloudMorph);

				#ifdef CV_LIGHTING_ON
					fixed BaseLightIntensity = max(max(_LightColor0.x, _LightColor0.y), _LightColor0.z);
					fixed3 finalColor = lerp( _Shading.rgb, _BaseColor.rgb, shadingRamp) * lerp(_LightColor0.rgb, BaseLightIntensity, _BaseColor.a);
				#else
				    fixed3 finalColor = lerp( _Shading.rgb, _BaseColor.rgb, shadingRamp) * lerp(_LightColor0.rgb, 1.0, _BaseColor.a);
				#endif
                return fixed4(finalColor, cloudMorph);
            }
            ENDCG
		}
		
    }
    FallBack "Diffuse"
    CustomEditor "CloudMaterialInspector"
}
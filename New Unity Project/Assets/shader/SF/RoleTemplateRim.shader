// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9361,x:33601,y:32390,varname:node_9361,prsc:2|emission-9903-OUT,custl-2159-OUT;n:type:ShaderForge.SFN_LightColor,id:3406,x:32464,y:32570,varname:node_3406,prsc:2;n:type:ShaderForge.SFN_LightVector,id:6869,x:31363,y:32510,varname:node_6869,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9684,x:31363,y:32697,prsc:2,pt:True;n:type:ShaderForge.SFN_Dot,id:7782,x:31769,y:32590,cmnt:Lambert,varname:node_7782,prsc:2,dt:1|A-6869-OUT,B-9684-OUT;n:type:ShaderForge.SFN_Tex2d,id:851,x:31786,y:31983,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1941,x:32823,y:32498,cmnt:Diffuse Contribution,varname:node_1941,prsc:2|A-544-OUT,B-7782-OUT,C-3406-RGB;n:type:ShaderForge.SFN_Color,id:5927,x:31700,y:32254,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:2159,x:33098,y:32714,cmnt:Combine,varname:node_2159,prsc:2|A-1941-OUT,B-4357-OUT,C-1762-OUT;n:type:ShaderForge.SFN_Multiply,id:2460,x:33018,y:32276,cmnt:Ambient Light,varname:node_2460,prsc:2|A-544-OUT,B-4458-OUT;n:type:ShaderForge.SFN_Multiply,id:544,x:32132,y:32127,cmnt:Diffuse Color,varname:node_544,prsc:2|A-851-RGB,B-5927-RGB;n:type:ShaderForge.SFN_Slider,id:4458,x:32355,y:32357,ptovrint:False,ptlb:EmissionIntensity,ptin:_EmissionIntensity,varname:_EmissionIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Fresnel,id:6163,x:31826,y:32847,varname:node_6163,prsc:2|EXP-9751-OUT;n:type:ShaderForge.SFN_Slider,id:9751,x:31303,y:32933,ptovrint:False,ptlb:FresnelPow,ptin:_FresnelPow,varname:_FresnelPow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:3,max:11;n:type:ShaderForge.SFN_Negate,id:1930,x:31368,y:33133,varname:node_1930,prsc:2|IN-6869-OUT;n:type:ShaderForge.SFN_Dot,id:8625,x:31780,y:33114,varname:node_8625,prsc:2,dt:1|A-1930-OUT,B-9684-OUT;n:type:ShaderForge.SFN_Multiply,id:4357,x:32676,y:33040,varname:node_4357,prsc:2|A-544-OUT,B-8625-OUT,C-2190-RGB;n:type:ShaderForge.SFN_Color,id:2190,x:32076,y:33247,ptovrint:False,ptlb:BackColor,ptin:_BackColor,varname:_BackColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1843137,c2:0.4627451,c3:0.9176471,c4:1;n:type:ShaderForge.SFN_Multiply,id:1762,x:32590,y:32711,varname:node_1762,prsc:2|A-6163-OUT,B-1425-OUT,C-6774-RGB,D-7782-OUT;n:type:ShaderForge.SFN_Color,id:6774,x:32155,y:32895,ptovrint:False,ptlb:RimColor,ptin:_RimColor,varname:_RimColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9333333,c2:0.682353,c3:0.3647059,c4:1;n:type:ShaderForge.SFN_Cubemap,id:8564,x:32498,y:31804,ptovrint:False,ptlb:CubeMap,ptin:_CubeMap,varname:_CubeMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0;n:type:ShaderForge.SFN_Add,id:6605,x:33343,y:32223,varname:node_6605,prsc:2|A-2460-OUT,B-4452-OUT;n:type:ShaderForge.SFN_Multiply,id:4452,x:32986,y:31949,varname:node_4452,prsc:2|A-8564-RGB,B-2193-OUT,C-9353-R;n:type:ShaderForge.SFN_Slider,id:2193,x:32452,y:32068,ptovrint:False,ptlb:CubeMapIntensity,ptin:_CubeMapIntensity,varname:_CubeMapIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:4;n:type:ShaderForge.SFN_Lerp,id:9903,x:33399,y:32068,varname:node_9903,prsc:2|A-2460-OUT,B-4452-OUT,T-6947-OUT;n:type:ShaderForge.SFN_Slider,id:6947,x:32861,y:32147,ptovrint:False,ptlb:ReflectIntensity,ptin:_ReflectIntensity,varname:_ReflectIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:9353,x:31763,y:31702,ptovrint:False,ptlb:MaskTex,ptin:_MaskTex,varname:_MaskTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:1425,x:32230,y:32800,varname:node_1425,prsc:2,v1:1.4;proporder:851-5927-4458-9751-2190-6774-8564-2193-6947-9353;pass:END;sub:END;*/

Shader "GOE/SF/RoleTemplateRim" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _EmissionIntensity ("EmissionIntensity", Range(0, 2)) = 1
        _FresnelPow ("FresnelPow", Range(1, 11)) = 3
        _BackColor ("BackColor", Color) = (0.1843137,0.4627451,0.9176471,1)
        _RimColor ("RimColor", Color) = (0.9333333,0.682353,0.3647059,1)
        _CubeMap ("CubeMap", Cube) = "_Skybox" {}
        _CubeMapIntensity ("CubeMapIntensity", Range(0, 4)) = 0
        _ReflectIntensity ("ReflectIntensity", Range(0, 1)) = 0
        _MaskTex ("MaskTex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            //#pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform float _EmissionIntensity;
            uniform float _FresnelPow;
            uniform float4 _BackColor;
            uniform float4 _RimColor;
            uniform samplerCUBE _CubeMap;
            uniform float _CubeMapIntensity;
            uniform float _ReflectIntensity;
            uniform sampler2D _MaskTex; uniform float4 _MaskTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_544 = (_MainTex_var.rgb*_Color.rgb); // Diffuse Color
                float3 node_2460 = (node_544*_EmissionIntensity); // Ambient Light
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(i.uv0, _MaskTex));
                float3 node_4452 = (texCUBE(_CubeMap,viewReflectDirection).rgb*_CubeMapIntensity*_MaskTex_var.r);
                float3 emissive = lerp(node_2460,node_4452,_ReflectIntensity);
                float node_7782 = max(0,dot(lightDirection,normalDirection)); // Lambert
                float3 finalColor = emissive + ((node_544*node_7782*_LightColor0.rgb)+(node_544*max(0,dot((-1*lightDirection),normalDirection))*_BackColor.rgb)+(pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPow)*1.4*_RimColor.rgb*node_7782));
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

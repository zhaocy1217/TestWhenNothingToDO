// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|emission-5644-OUT,custl-5085-OUT;n:type:ShaderForge.SFN_LightColor,id:3406,x:32315,y:33021,varname:node_3406,prsc:2;n:type:ShaderForge.SFN_LightVector,id:6869,x:31003,y:32556,varname:node_6869,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9684,x:31003,y:32684,prsc:2,pt:True;n:type:ShaderForge.SFN_HalfVector,id:9471,x:31003,y:32835,varname:node_9471,prsc:2;n:type:ShaderForge.SFN_Dot,id:7782,x:31215,y:32599,cmnt:Lambert,varname:lambert,prsc:2,dt:1|A-6869-OUT,B-9684-OUT;n:type:ShaderForge.SFN_Dot,id:3269,x:31215,y:32825,cmnt:Blinn-Phong,varname:node_3269,prsc:2,dt:1|A-9684-OUT,B-9471-OUT;n:type:ShaderForge.SFN_Multiply,id:2746,x:32202,y:32782,cmnt:Specular Contribution,varname:node_2746,prsc:2|A-7782-OUT,B-5267-OUT,C-4865-RGB,D-851-A;n:type:ShaderForge.SFN_Tex2d,id:851,x:30487,y:32350,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1941,x:31838,y:32518,cmnt:Diffuse Contribution,varname:node_1941,prsc:2|A-544-OUT,B-7782-OUT;n:type:ShaderForge.SFN_Color,id:5927,x:30343,y:32734,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Exp,id:1700,x:31556,y:33018,varname:node_1700,prsc:2,et:1|IN-9978-OUT;n:type:ShaderForge.SFN_Slider,id:5328,x:30873,y:33183,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Power,id:5267,x:31869,y:32907,varname:node_5267,prsc:2|VAL-3269-OUT,EXP-1700-OUT;n:type:ShaderForge.SFN_Add,id:2159,x:32543,y:32768,cmnt:Combine,varname:node_2159,prsc:2|A-1941-OUT,B-2746-OUT;n:type:ShaderForge.SFN_Multiply,id:5085,x:32847,y:32863,cmnt:Attenuate and Color,varname:node_5085,prsc:2|A-2159-OUT,B-3406-RGB;n:type:ShaderForge.SFN_ConstantLerp,id:9978,x:31275,y:33102,varname:node_9978,prsc:2,a:1,b:11|IN-5328-OUT;n:type:ShaderForge.SFN_Color,id:4865,x:31869,y:33151,ptovrint:False,ptlb:Spec Color,ptin:_SpecColor,varname:_SpecColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:544,x:31050,y:32286,cmnt:Diffuse Color,varname:diffColor,prsc:2|A-851-RGB,B-5927-RGB;n:type:ShaderForge.SFN_Cubemap,id:4813,x:30638,y:31586,ptovrint:False,ptlb:CubeMap,ptin:_CubeMap,varname:_CubeMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0;n:type:ShaderForge.SFN_Slider,id:3468,x:30481,y:31830,ptovrint:False,ptlb:CubeMapIntensity,ptin:_CubeMapIntensity,varname:_CubeMapIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:6;n:type:ShaderForge.SFN_Multiply,id:9500,x:31867,y:32001,varname:node_9500,prsc:2|A-4813-RGB,B-3468-OUT,C-544-OUT,D-8759-R;n:type:ShaderForge.SFN_Add,id:5644,x:32634,y:32287,varname:node_5644,prsc:2|A-9500-OUT,B-9512-OUT,C-8378-OUT,D-936-OUT;n:type:ShaderForge.SFN_Tex2d,id:8759,x:30615,y:31986,ptovrint:False,ptlb:MaskTex,ptin:_MaskTex,varname:_MaskTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Slider,id:2354,x:31801,y:32220,ptovrint:False,ptlb:EmissionInstensity,ptin:_EmissionInstensity,varname:_EmissionInstensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:9512,x:32230,y:32362,varname:node_9512,prsc:2|A-544-OUT,B-2354-OUT;n:type:ShaderForge.SFN_Color,id:9315,x:31097,y:31362,ptovrint:False,ptlb:GlowColor,ptin:_GlowColor,varname:_GlowColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:1632,x:31048,y:31613,ptovrint:False,ptlb:GlowInstensity,ptin:_GlowInstensity,varname:_GlowInstensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:3;n:type:ShaderForge.SFN_Multiply,id:8378,x:31749,y:31658,varname:node_8378,prsc:2|A-9315-RGB,B-1632-OUT,C-8759-G;n:type:ShaderForge.SFN_Color,id:5615,x:30706,y:31040,ptovrint:False,ptlb:CapColor,ptin:_CapColor,varname:_CapColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:3687,x:30636,y:31275,ptovrint:False,ptlb:CapIntensity,ptin:_CapIntensity,varname:_CapIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:936,x:31129,y:31078,varname:node_936,prsc:2|A-5615-RGB,B-3687-OUT,C-2264-RGB;n:type:ShaderForge.SFN_Tex2d,id:2264,x:30705,y:30772,ptovrint:False,ptlb:CapTex,ptin:_CapTex,varname:_CapTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3512-OUT;n:type:ShaderForge.SFN_Transform,id:266,x:30266,y:30381,varname:node_266,prsc:2,tffrom:0,tfto:3|IN-1463-OUT;n:type:ShaderForge.SFN_NormalVector,id:1463,x:30064,y:30408,prsc:2,pt:False;n:type:ShaderForge.SFN_ComponentMask,id:853,x:30481,y:30399,varname:node_853,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-266-XYZ;n:type:ShaderForge.SFN_RemapRange,id:3512,x:30390,y:30632,varname:capUV,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-853-OUT;proporder:851-5927-5328-4865-2354-8759-4813-3468-9315-1632-2264-5615-3687;pass:END;sub:END;*/

Shader "GOE/SF/Template" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _SpecColor ("Spec Color", Color) = (1,1,1,1)
        _EmissionInstensity ("EmissionInstensity", Range(0, 1)) = 0
        _MaskTex ("MaskTex", 2D) = "black" {}
        _CubeMap ("CubeMap", Cube) = "_Skybox" {}
        _CubeMapIntensity ("CubeMapIntensity", Range(0, 6)) = 0
        _GlowColor ("GlowColor", Color) = (0.5,0.5,0.5,1)
        _GlowInstensity ("GlowInstensity", Range(0, 3)) = 0
        _CapTex ("CapTex", 2D) = "white" {}
        _CapColor ("CapColor", Color) = (0.5,0.5,0.5,1)
        _CapIntensity ("CapIntensity", Range(0, 1)) = 0
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
            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform float _Gloss;
            uniform float4 _SpecColor;
            uniform samplerCUBE _CubeMap;
            uniform float _CubeMapIntensity;
            uniform sampler2D _MaskTex; uniform float4 _MaskTex_ST;
            uniform float _EmissionInstensity;
            uniform float4 _GlowColor;
            uniform float _GlowInstensity;
            uniform float4 _CapColor;
            uniform float _CapIntensity;
            uniform sampler2D _CapTex; uniform float4 _CapTex_ST;
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
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffColor = (_MainTex_var.rgb*_Color.rgb); // Diffuse Color
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(i.uv0, _MaskTex));
                float2 capUV = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg*0.5+0.5);
                float4 _CapTex_var = tex2D(_CapTex,TRANSFORM_TEX(capUV, _CapTex));
                float3 emissive = ((texCUBE(_CubeMap,viewReflectDirection).rgb*_CubeMapIntensity*diffColor*_MaskTex_var.r)+(diffColor*_EmissionInstensity)+(_GlowColor.rgb*_GlowInstensity*_MaskTex_var.g)+(_CapColor.rgb*_CapIntensity*_CapTex_var.rgb));
                float lambert = max(0,dot(lightDirection,normalDirection)); // Lambert
                float3 finalColor = emissive + (((diffColor*lambert)+(lambert*pow(max(0,dot(normalDirection,halfDirection)),exp2(lerp(1,11,_Gloss)))*_SpecColor.rgb*_MainTex_var.a))*_LightColor0.rgb);
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

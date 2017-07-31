// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9361,x:33509,y:32043,varname:node_9361,prsc:2|normal-9348-OUT,emission-7570-OUT,custl-5085-OUT;n:type:ShaderForge.SFN_LightColor,id:3406,x:32734,y:32952,varname:node_3406,prsc:2;n:type:ShaderForge.SFN_LightVector,id:6869,x:31486,y:32478,varname:node_6869,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9684,x:30411,y:32362,prsc:2,pt:False;n:type:ShaderForge.SFN_HalfVector,id:9471,x:31334,y:33075,varname:node_9471,prsc:2;n:type:ShaderForge.SFN_Dot,id:7782,x:31804,y:32672,cmnt:Lambert,varname:node_7782,prsc:2,dt:1|A-6869-OUT,B-9684-OUT;n:type:ShaderForge.SFN_Dot,id:3269,x:31810,y:32963,cmnt:Blinn-Phong,varname:node_3269,prsc:2,dt:1|A-5889-OUT,B-9471-OUT;n:type:ShaderForge.SFN_Multiply,id:2746,x:32394,y:33042,cmnt:Specular Contribution,varname:node_2746,prsc:2|A-7782-OUT,B-5267-OUT,C-4865-RGB,D-3525-OUT;n:type:ShaderForge.SFN_Multiply,id:1941,x:32465,y:32693,cmnt:Diffuse Contribution,varname:node_1941,prsc:2|A-544-OUT,B-7782-OUT;n:type:ShaderForge.SFN_Color,id:5927,x:32024,y:32480,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Exp,id:1700,x:31867,y:33264,varname:node_1700,prsc:2,et:1|IN-9978-OUT;n:type:ShaderForge.SFN_Slider,id:5328,x:31238,y:33345,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Power,id:5267,x:32094,y:33176,varname:node_5267,prsc:2|VAL-3269-OUT,EXP-1700-OUT;n:type:ShaderForge.SFN_Add,id:2159,x:32734,y:32812,cmnt:Combine,varname:node_2159,prsc:2|A-1941-OUT,B-2746-OUT;n:type:ShaderForge.SFN_Multiply,id:5085,x:33036,y:32790,cmnt:Attenuate and Color,varname:node_5085,prsc:2|A-2159-OUT,B-3406-RGB;n:type:ShaderForge.SFN_ConstantLerp,id:9978,x:31671,y:33301,varname:node_9978,prsc:2,a:1,b:11|IN-5328-OUT;n:type:ShaderForge.SFN_Color,id:4865,x:32094,y:33398,ptovrint:False,ptlb:Spec Color,ptin:_SpecColor,varname:_SpecColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:2460,x:32872,y:32276,cmnt:Ambient Light,varname:node_2460,prsc:2|A-544-OUT,B-867-OUT;n:type:ShaderForge.SFN_Multiply,id:544,x:32295,y:32424,cmnt:Diffuse Color,varname:node_544,prsc:2|A-8335-OUT,B-5927-RGB;n:type:ShaderForge.SFN_Tex2d,id:774,x:28605,y:31316,ptovrint:False,ptlb:Splat0,ptin:_Splat0,varname:_Splat0,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3003,x:28511,y:31583,ptovrint:False,ptlb:Splat1,ptin:_Splat1,varname:_Splat1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:380,x:28166,y:31803,ptovrint:False,ptlb:Splat2,ptin:_Splat2,varname:_Splat2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7045,x:27779,y:32121,ptovrint:False,ptlb:Control,ptin:_Control,varname:_Control,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2188,x:29217,y:31337,varname:node_2188,prsc:2|A-774-RGB,B-7045-R;n:type:ShaderForge.SFN_Multiply,id:2067,x:29235,y:31476,varname:node_2067,prsc:2|A-3003-RGB,B-7045-G;n:type:ShaderForge.SFN_Multiply,id:5904,x:29332,y:31659,varname:node_5904,prsc:2|A-380-RGB,B-5593-OUT;n:type:ShaderForge.SFN_Add,id:8335,x:30274,y:31776,varname:node_8335,prsc:2|A-2188-OUT,B-2067-OUT,C-5904-OUT;n:type:ShaderForge.SFN_Slider,id:867,x:32498,y:32467,ptovrint:False,ptlb:EmissionIntensity,ptin:_EmissionIntensity,varname:_EmissionIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Cubemap,id:1374,x:31821,y:32186,ptovrint:False,ptlb:CubeMap,ptin:_CubeMap,varname:_CubeMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0|DIR-148-OUT;n:type:ShaderForge.SFN_Multiply,id:592,x:32121,y:31971,varname:node_592,prsc:2|A-3525-OUT,B-902-OUT,C-1374-RGB;n:type:ShaderForge.SFN_Add,id:7570,x:33087,y:32161,varname:node_7570,prsc:2|A-592-OUT,B-2460-OUT;n:type:ShaderForge.SFN_Slider,id:902,x:31620,y:32073,ptovrint:False,ptlb:CubeMapIntensity,ptin:_CubeMapIntensity,varname:_CubeMapIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Tex2d,id:6965,x:30753,y:30880,varname:_Bump1,prsc:2,ntxv:3,isnm:True|UVIN-9868-UVOUT,TEX-378-TEX;n:type:ShaderForge.SFN_Subtract,id:5593,x:28859,y:31949,varname:node_5593,prsc:2|A-7045-A,B-480-OUT;n:type:ShaderForge.SFN_Panner,id:9868,x:30418,y:30821,varname:node_9868,prsc:2,spu:1,spv:0|UVIN-2068-UVOUT,DIST-1866-OUT;n:type:ShaderForge.SFN_TexCoord,id:2068,x:30052,y:30844,varname:node_2068,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:9348,x:31240,y:31091,varname:node_9348,prsc:2|A-6965-RGB,B-7456-RGB;n:type:ShaderForge.SFN_TexCoord,id:7552,x:30052,y:31032,varname:node_7552,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:7268,x:30418,y:31039,varname:node_7268,prsc:2,spu:-2,spv:0|UVIN-7552-UVOUT,DIST-1866-OUT;n:type:ShaderForge.SFN_Tex2d,id:7456,x:30832,y:31159,varname:_node_7456,prsc:2,ntxv:3,isnm:True|UVIN-7268-UVOUT,TEX-378-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:378,x:30316,y:31300,ptovrint:False,ptlb:Bump,ptin:_Bump,varname:_Bump,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Reflect,id:148,x:31504,y:32250,varname:node_148,prsc:2|A-8374-OUT,B-5889-OUT;n:type:ShaderForge.SFN_ViewVector,id:3483,x:30439,y:32074,varname:node_3483,prsc:2;n:type:ShaderForge.SFN_Negate,id:8374,x:30910,y:32172,varname:node_8374,prsc:2|IN-3483-OUT;n:type:ShaderForge.SFN_NormalVector,id:898,x:30341,y:32643,prsc:2,pt:True;n:type:ShaderForge.SFN_Lerp,id:5889,x:31148,y:32531,varname:node_5889,prsc:2|A-898-OUT,B-9684-OUT,T-1371-OUT;n:type:ShaderForge.SFN_Slider,id:6411,x:29846,y:33007,ptovrint:False,ptlb:BumpFactor,ptin:_BumpFactor,varname:_BumpFactor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:1407,x:30503,y:32853,varname:node_1407,prsc:2|A-3525-OUT,B-7134-OUT;n:type:ShaderForge.SFN_Clamp01,id:1371,x:30817,y:32756,varname:node_1371,prsc:2|IN-1407-OUT;n:type:ShaderForge.SFN_RemapRange,id:7134,x:30299,y:32968,varname:node_7134,prsc:2,frmn:0,frmx:1,tomn:1,tomx:-1|IN-6411-OUT;n:type:ShaderForge.SFN_Multiply,id:3525,x:28447,y:32362,varname:node_3525,prsc:2|A-7045-B,B-2564-OUT;n:type:ShaderForge.SFN_Slider,id:2564,x:27972,y:32579,ptovrint:False,ptlb:WaterIntensity,ptin:_WaterIntensity,varname:_WaterIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Time,id:6539,x:29752,y:30586,varname:node_6539,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1866,x:30079,y:30648,varname:node_1866,prsc:2|A-6539-T,B-5091-OUT,C-8636-OUT;n:type:ShaderForge.SFN_Slider,id:5091,x:29658,y:30784,ptovrint:False,ptlb:FlowSpeed,ptin:_FlowSpeed,varname:_FlowSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Vector1,id:8636,x:29781,y:30903,varname:node_8636,prsc:2,v1:0.001;n:type:ShaderForge.SFN_Multiply,id:480,x:28742,y:32113,varname:node_480,prsc:2|A-7045-B,B-9406-OUT;n:type:ShaderForge.SFN_Vector1,id:9406,x:28531,y:32276,varname:node_9406,prsc:2,v1:1.5;proporder:5927-5328-4865-774-3003-380-7045-867-1374-902-378-6411-2564-5091;pass:END;sub:END;*/

Shader "GOE/SF/TerrainTemplate" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Splat0 ("Splat0", 2D) = "black" {}
        _Splat1 ("Splat1", 2D) = "black" {}
        _Splat2 ("Splat2", 2D) = "black" {}
        _Control ("Control", 2D) = "black" {}
        _EmissionIntensity ("EmissionIntensity", Range(0, 1)) = 0.5
        _CubeMap ("CubeMap", Cube) = "_Skybox" {}
        _CubeMapIntensity ("CubeMapIntensity", Range(0, 2)) = 0
        _Bump ("Bump", 2D) = "bump" {}
        _BumpFactor ("BumpFactor", Range(0, 1)) = 0
        _WaterIntensity ("WaterIntensity", Range(0, 1)) = 0
        _FlowSpeed ("FlowSpeed", Range(0, 1)) = 1
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
            #pragma exclude_renderers d3d11 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform float _Gloss;
            uniform float4 _SpecColor;
            uniform sampler2D _Splat0; uniform float4 _Splat0_ST;
            uniform sampler2D _Splat1; uniform float4 _Splat1_ST;
            uniform sampler2D _Splat2; uniform float4 _Splat2_ST;
            uniform sampler2D _Control; uniform float4 _Control_ST;
            uniform float _EmissionIntensity;
            uniform samplerCUBE _CubeMap;
            uniform float _CubeMapIntensity;
            uniform sampler2D _Bump; uniform float4 _Bump_ST;
            uniform float _BumpFactor;
            uniform float _WaterIntensity;
            uniform float _FlowSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_6539 = _Time + _TimeEditor;
                float node_1866 = (node_6539.g*_FlowSpeed*0.001);
                float2 node_9868 = (i.uv0+node_1866*float2(1,0));
                float3 _Bump1 = UnpackNormal(tex2D(_Bump,TRANSFORM_TEX(node_9868, _Bump)));
                float2 node_7268 = (i.uv0+node_1866*float2(-2,0));
                float3 _node_7456 = UnpackNormal(tex2D(_Bump,TRANSFORM_TEX(node_7268, _Bump)));
                float3 normalLocal = (_Bump1.rgb+_node_7456.rgb);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
////// Emissive:
                float4 _Control_var = tex2D(_Control,TRANSFORM_TEX(i.uv0, _Control));
                float node_3525 = (_Control_var.b*_WaterIntensity);
                float3 node_5889 = lerp(normalDirection,i.normalDir,saturate((node_3525+(_BumpFactor*-2.0+1.0))));
                float4 _Splat0_var = tex2D(_Splat0,TRANSFORM_TEX(i.uv0, _Splat0));
                float4 _Splat1_var = tex2D(_Splat1,TRANSFORM_TEX(i.uv0, _Splat1));
                float4 _Splat2_var = tex2D(_Splat2,TRANSFORM_TEX(i.uv0, _Splat2));
                float3 node_544 = (((_Splat0_var.rgb*_Control_var.r)+(_Splat1_var.rgb*_Control_var.g)+(_Splat2_var.rgb*(_Control_var.a-(_Control_var.b*1.5))))*_Color.rgb); // Diffuse Color
                float3 emissive = ((node_3525*_CubeMapIntensity*texCUBE(_CubeMap,reflect((-1*viewDirection),node_5889)).rgb)+(node_544*_EmissionIntensity));
                float node_7782 = max(0,dot(lightDirection,i.normalDir)); // Lambert
                float3 finalColor = emissive + (((node_544*node_7782)+(node_7782*pow(max(0,dot(node_5889,halfDirection)),exp2(lerp(1,11,_Gloss)))*_SpecColor.rgb*node_3525))*_LightColor0.rgb);
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

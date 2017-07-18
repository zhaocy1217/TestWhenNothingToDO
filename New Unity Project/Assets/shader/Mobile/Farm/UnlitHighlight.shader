// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:8252,x:33048,y:32732,varname:node_8252,prsc:2|custl-5379-OUT;n:type:ShaderForge.SFN_TexCoord,id:1011,x:32091,y:32828,varname:node_1011,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:5969,x:32317,y:32828,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1011-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:6640,x:32317,y:33057,ptovrint:False,ptlb:LightTex,ptin:_LightTex,varname:_LightTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7556-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:7556,x:32091,y:33057,varname:node_7556,prsc:2,uv:1;n:type:ShaderForge.SFN_Multiply,id:5379,x:32778,y:32974,varname:node_5379,prsc:2|A-5969-RGB,B-2-OUT;n:type:ShaderForge.SFN_Slider,id:3479,x:32160,y:33281,ptovrint:False,ptlb:LightTexStrenth,ptin:_LightTexStrenth,varname:_LightTexStrenth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Multiply,id:2,x:32532,y:33197,varname:node_2,prsc:2|A-6640-RGB,B-3479-OUT;n:type:ShaderForge.SFN_LightColor,id:636,x:32532,y:33014,varname:node_636,prsc:2;proporder:5969-6640-3479;pass:END;sub:END;*/

Shader "Unlit/Highlight" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _LightTex ("LightTex", 2D) = "white" {}
        _LightTexStrenth ("LightTexStrenth", Range(0, 2)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 100
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _LightTex; uniform float4 _LightTex_ST;
            uniform float _LightTexStrenth;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 _LightTex_var = tex2D(_LightTex,TRANSFORM_TEX(i.uv1, _LightTex));
                float3 finalColor = (_MainTex_var.rgb*(_LightTex_var.rgb*_LightTexStrenth));
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

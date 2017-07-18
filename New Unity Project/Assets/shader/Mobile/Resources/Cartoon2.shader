Shader "CM/Cartoon2" {
    Properties 
	{
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_EdgeThickness ("Outline Thickness", Float) = 1
		_EdgeColor("Outline Color", Color)= (1,1,1,1)
		_IlluminPower("Illumin Power", Range(0, 2)) = 1
		_CapTex("Cap Tex (RGB)", 2D) = "white" {}
		_CapColor("Cap Color", Color) = (0,0,0,1)
    }
    SubShader 
	{
		Tags
		{
			"RenderType"="Opaque"
			"Queue"="Geometry"
			"LightMode"="ForwardBase"
		}
		
		Pass
		{
			Cull Front
			ZTest Less
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			#define INV_EDGE_THICKNESS_DIVISOR 0.00285	// Outline thickness multiplier
			//#define SATURATION_FACTOR 0.6				// Outline color parameters
			//#define BRIGHTNESS_FACTOR 0.8

			//float4 _Color;
			//float4 _LightColor0;
			//float4 _MainTex_ST;
			//sampler2D _MainTex;

			float _EdgeThickness;
			float4 _EdgeColor;
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				//float2 uv : TEXCOORD0;
			};
			
			
			v2f vert( appdata_base v )
			{
				v2f o;
				//o.uv = TRANSFORM_TEX( v.texcoord.xy, _MainTex );

				half4 projSpacePos = mul( UNITY_MATRIX_MVP, v.vertex );
				half4 projSpaceNormal = normalize( mul( UNITY_MATRIX_MVP, half4( v.normal, 0 ) ) );
				half4 scaledNormal = _EdgeThickness * INV_EDGE_THICKNESS_DIVISOR * projSpaceNormal; // * projSpacePos.w;

				scaledNormal.z += 0.00001;
				o.pos = projSpacePos + scaledNormal;

				return o;
			}

			half4 frag( v2f i ) : COLOR
			{
				return _EdgeColor;

				//half4 diffuseMapColor = tex2D( _MainTex, i.uv );
				//half maxChan = max( max( diffuseMapColor.r, diffuseMapColor.g ), diffuseMapColor.b );
				//half4 newMapColor = diffuseMapColor;
				//maxChan -= ( 1.0 / 255.0 );
				//half3 lerpVals = saturate( ( newMapColor.rgb - half3( maxChan, maxChan, maxChan ) ) * 255.0 );
				//newMapColor.rgb = lerp( SATURATION_FACTOR * newMapColor.rgb, newMapColor.rgb, lerpVals );
				//return half4( BRIGHTNESS_FACTOR * newMapColor.rgb * diffuseMapColor.rgb, diffuseMapColor.a ) * _Color * _LightColor0;
			}
			
			ENDCG
		}
		
        Pass 
		{
			Cull Off
			ZTest LEqual
            CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			uniform float4 _MainTex_ST;
			float _IlluminPower;

			
			float _CapIntensity;
			uniform sampler2D _CapTex;
			fixed4 _CapColor;
				
			
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed2 cap : COLOR;
			};

			v2f vert (appdata_base v) 
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);

				half2 capCoord;
				capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
				capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
				o.cap = capCoord * 0.5 + 0.5;

				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}


			fixed4 frag(v2f i) : COLOR 
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 cap = tex2D(_CapTex, i.cap);
				
				tex.rgb = tex.rgb  * _Color.rgb * _IlluminPower * cap.rgb * _CapColor;
				return tex;
			}
			
            ENDCG
        }
    }
}
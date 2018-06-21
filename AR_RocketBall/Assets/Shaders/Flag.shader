﻿Shader "Custom/Flag"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
//		_Offset ("Offset", Float) = 0
//		_Speed ("Speed", Float) = 1
//		_Wave ("Wave", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Cull off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
//			float _Offset;
//			float _Speed;
//			float _Wave;
			
			v2f vert (appdata v)
			{
				v2f o;

//				if(v.vertex.x > -5)
//				{
//					v.vertex.y += sin((_Time.y * _Speed - v.vertex.x - v.vertex.z) * _Wave) * _Offset;
//					v.vertex.z += (v.vertex.x + 5) * 0.2;
//				}

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
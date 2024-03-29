﻿Shader "Custom/EyeTint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_R ("R", float) = 0 // 0 - 1
		_G ("G", float) = 0
		_B ("B", float) = 0
	}
	SubShader
	{

		Tags 
        { 
			"Queue"="Transparent"
        }                

		// No culling or depth
		Cull Off ZWrite Off ZTest Off
		ColorMask RGB
        Blend SrcAlpha OneMinusSrcAlpha

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
			float _R; float _G; float _B;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				return fixed4(col.r + _R, col.g + _G, col.b + _B, col.a);

				return col;
			}
			ENDCG
		}
	}
}

Shader "Custom/CMYCycleShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Duration ("Duration", float) = 7
		_Invert ("Invert", int) = 0 // 0 does nothing, 1 inverts palette
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
			float _Duration;
			fixed _Invert;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				const float PI = 3.1415926535;
				float sineInput = _Time[1] * (PI/2) / _Duration;

				fixed r = abs(_Invert - max(abs(sin(sineInput)), col.r));
				fixed g = abs(_Invert - max(abs(sin(sineInput + radians(120))), col.g));
				fixed b = abs(_Invert - max(abs(sin(sineInput + radians(240))), col.b));

				col = fixed4(r, g, b, 1);
				return col;
			}
			ENDCG
		}
	}
}

Shader "Custom/CircleGradientShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Duration ("Duration", float) = 1
		_CenterX ("Center X", float) = 0.5
		_CenterY ("Center Y", float) = 0.5
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
				float2 screenPos : TEXCOORD2;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			sampler2D _MainTex;
			float _Duration;
			float _CenterX;
			float _CenterY;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				const float PI = 3.1415926535;
				float2 center = float2(_CenterX, _CenterY);
				float distanceToCenter = distance(i.screenPos, center);
				fixed timeCycleR = ((_Time[1] - _Duration/3) % _Duration) / _Duration; // Cycles from 0 to 1
				fixed timeCycleG = (_Time[1] % _Duration) / _Duration;
				fixed timeCycleB = ((_Time[1] + _Duration/3) % _Duration) / _Duration;

				fixed r = 1 - abs(timeCycleR - distanceToCenter);
				fixed g = 1 - abs(timeCycleG - distanceToCenter);
				fixed b = 1 - abs(timeCycleB - distanceToCenter);

				r = max(r, col.r);
				g = max(g, col.g);
				b = max(b, col.b);

				col = fixed4(r, g, b, 1);

				return col;
			}
			ENDCG
		}
	}
}

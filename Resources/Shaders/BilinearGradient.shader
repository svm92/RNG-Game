Shader "Custom/BilinearGradientShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Duration ("Duration", float) = 3
		_ROffset ("R Offset", float) = 0
		_GOffset ("G Offset", float) = 2
		_BOffset ("B Offset", float) = 4
		_Phase ("Phase Difference", float) = 0 // Begin with n seconds already simulated
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
			float _ROffset;
			float _GOffset;
			float _BOffset;
			float _Phase;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				const float PI = 3.1415926535;

				fixed timeCycle = abs(sin(((_Time[1] + _Phase) * PI/2 / _Duration) + _ROffset)); // Cycles between 0 and 1
				float r1 = (abs(timeCycle - i.screenPos.x));

				timeCycle = abs(sin(((_Time[1] + _Phase) * PI/2 / _Duration) + _GOffset)); // Cycles between 0 and 1
				float g1 = (abs(timeCycle - i.screenPos.x));

				timeCycle = abs(sin(((_Time[1] + _Phase) * PI/2 / _Duration) + _BOffset)); // Cycles between 0 and 1
				float b1 = (abs(timeCycle - i.screenPos.x));

				timeCycle = abs(sin(((_Time[1] + _Phase) * PI/2 / _Duration) + _GOffset + _BOffset)); // Cycles between 0 and 1
				float r2 = (abs(timeCycle - i.screenPos.y));

				timeCycle = abs(sin(((_Time[1] + _Phase) * PI/2 / _Duration) + _ROffset + _BOffset)); // Cycles between 0 and 1
				float g2 = (abs(timeCycle - i.screenPos.y));

				timeCycle = abs(sin(((_Time[1] + _Phase) * PI/2 / _Duration) + _ROffset + _GOffset)); // Cycles between 0 and 1
				float b2 = (abs(timeCycle - i.screenPos.y));
				
				fixed r = (r1 + r2) / 2;
				fixed g = (g1 + g2) / 2;
				fixed b = (b1 + b2) / 2;

				r = min(r, col.r);
				g = min(g, col.g);
				b = min(b, col.b);

				col = fixed4(r, g, b, 1);
				return col;
			}
			ENDCG
		}
	}
}

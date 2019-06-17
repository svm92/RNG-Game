Shader "Custom/DoubleLinearGradientShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Duration1 ("Duration 1", float) = 3
		_Duration2 ("Duration 2", float) = 7
		_ROffset ("R Offset", float) = 0
		_GOffset ("G Offset", float) = 2
		_BOffset ("B Offset", float) = 4
		_Direction ("Direction", int) = 0
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
			float _Duration1;
			float _Duration2;
			float _ROffset;
			float _GOffset;
			float _BOffset;
			int _Direction;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				const float PI = 3.1415926535;
				float direction = (_Direction == 0) ? i.screenPos.x : i.screenPos.y;

				// Applies two simultaneous linear gradients in the same direction
				///////////////// First pass
				fixed timeCycle = abs(sin((_Time[1] * PI/2 / _Duration1) + _ROffset)); // Cycles between 0 and 1
				fixed r = abs(timeCycle - direction);

				timeCycle = abs(sin((_Time[1] * PI/2 / _Duration1) + _GOffset)); // Cycles between 0 and 1
				fixed g = abs(timeCycle - direction);

				timeCycle = abs(sin((_Time[1] * PI/2 / _Duration1) + _BOffset)); // Cycles between 0 and 1
				fixed b = abs(timeCycle - direction);

				r = min(r, col.r);
				g = min(g, col.g);
				b = min(b, col.b);

				col = fixed4(r, g, b, 1);

				///////////////// Second pass
				timeCycle = abs(sin((_Time[1] * PI/2 / _Duration2) + _GOffset)); // Cycles between 0 and 1
				r = abs(timeCycle - direction);

				timeCycle = abs(sin((_Time[1] * PI/2 / _Duration2) + _BOffset)); // Cycles between 0 and 1
				g = abs(timeCycle - direction);

				timeCycle = abs(sin((_Time[1] * PI/2 / _Duration2) + _ROffset)); // Cycles between 0 and 1
				b = abs(timeCycle - direction);

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

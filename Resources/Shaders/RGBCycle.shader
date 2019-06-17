Shader "Custom/RGBCycleShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Duration ("Cycle Duration", float) = 1 // Duration of a positive sine wave for a single color
		_Overlap ("Color Overlap", float) = 0.1 // Percentage (%) of the sine wave of two colors that overlaps
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Duration;
			float _Overlap;
			int _Invert;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				const float PI = 3.1415926535;
				float halfDuration = _Duration/2;
				_Overlap = _Duration * _Overlap; // // Take absolute value (in seconds) from the percentage input
				float period = _Duration * 3 - _Overlap * 3; // Time for the 3 sine waves to raise and fall
				// Period time divides the time in cycles of (period) seconds (i.e. in period=2.8, cyclic time counting from 0~2.8)
				float periodTime = _Time[1] % period;

				float waveRStart = 0;
				float waveREnd = waveRStart + _Duration;

				float waveGStart = waveREnd - _Overlap;
				float waveGEnd = waveGStart + _Duration;

				// The B wave has two beginnings and ends (overlapping over R at the beginning of the period,
				// and the one at the end)
				float waveBStart1 = waveRStart;
				float waveBEnd1 = waveRStart + _Overlap;
				float waveBStart2 = waveGEnd - _Overlap;
				float waveBEnd2 = waveBStart2 + _Duration;

				float r = 0;
				float g = 0;
				float b = 0;
				
				if (periodTime >= waveRStart && periodTime <= waveREnd) {
					r = abs(sin(((_Time[1] - waveRStart) % period) * (PI/2) / halfDuration));
				}

				if (periodTime >= waveGStart && periodTime <= waveGEnd) {
					g = abs(sin(((_Time[1] - waveGStart) % period) * (PI/2) / halfDuration));
				}

				if ((periodTime >= waveBStart1 && periodTime <= waveBEnd1) 
					|| (periodTime >= waveBStart2 && periodTime <= waveBEnd2)) {
					b = abs(sin(((_Time[1] - waveBStart2) % period) * (PI/2) / halfDuration));
				}

				r = abs(_Invert - max(r, col.r));
				g = abs(_Invert - max(g, col.g));
				b = abs(_Invert - max(b, col.b));

				col = fixed4(r, g, b, 1);
				return col;
			}
			ENDCG
		}
	}
}

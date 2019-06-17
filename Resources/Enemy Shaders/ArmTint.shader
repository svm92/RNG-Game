Shader "Custom/ArmTint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		// HSV modifiers
		_HMod ("H Modifier", float) = 0
		_SMod ("S Modifier", float) = 0
		_VMod ("V Modifier", float) = 0
		// Should the h, s or v value be overriden or added to? (0 add, 1 override)
		_HOverride ("H Fixed", float) = 0
		_SOverride ("S Fixed", float) = 0
		_VOverride ("V Fixed", float) = 0
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
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}

			float3 rgb_to_hsv(fixed4 RGB) {
				fixed r = RGB.r; fixed g = RGB.g; fixed b = RGB.b;

				float _max = max(max(r, g), b);
				float _min = min(min(r, g), b);
				float3 HSV = _max; // Set h, s and v to _max
				float d = _max - _min; // Delta

				if(_max == _min){ // Achromatic, hue = 0
					HSV.x = 0;
				}else{
					if (_max == r) {
						HSV.x = (g - b) / d + (g < b ? 6 : 0);
					}
					else if (_max == g) {
						HSV.x = (b - r) / d + 2;
					}
					else if (_max == b) {
						HSV.x = (r - g) / d + 4;
					}

					HSV.x /= 6;
				}

				return HSV;
			}

			float3 hsv_to_rgb(float3 HSV)
			{
				float3 RGB = HSV.z; // Set r, g and b to v
           
				float h = HSV.x * 6;
				float i = floor(h);
				float var_1 = HSV.z * (1.0 - HSV.y);
				float var_2 = HSV.z * (1.0 - HSV.y * (h - i));
				float var_3 = HSV.z * (1.0 - HSV.y * (1 - (h - i)));
				if      (i == 0) { RGB = float3(HSV.z, var_3, var_1); }
				else if (i == 1) { RGB = float3(var_2, HSV.z, var_1); }
				else if (i == 2) { RGB = float3(var_1, HSV.z, var_3); }
				else if (i == 3) { RGB = float3(var_1, var_2, HSV.z); }
				else if (i == 4) { RGB = float3(var_3, var_1, HSV.z); }
				else             { RGB = float3(HSV.z, var_1, var_2); }
           
			   return (RGB);
			}

			sampler2D _MainTex;
			float _HMod; float _SMod; float _VMod;
			float _HOverride; float _SOverride; float _VOverride;

			half4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				// Transform RGB to HSV
				fixed r = col.r; fixed g = col.g; fixed b = col.b;
				float3 HSV = rgb_to_hsv(col);

				// Apply changes to HSV
				HSV.x = (_HOverride == 1) ? _HMod : HSV.x + _HMod;
				HSV.y = (_SOverride == 1) ? _SMod : HSV.y + _SMod;
				HSV.z = (_VOverride == 1) ? _VMod : HSV.z + _VMod;

				// Back to RGB
				float3 RGB = hsv_to_rgb(HSV);

				// Normal sprite has a value of white (1). Shadows have a lower value. Don't change shadows
				if (i.color.x != 1) // If it is a shadow
					return half4(i.color.x, i.color.y, i.color.z, col.a * i.color.a);
				
				col = half4(RGB.r, RGB.g, RGB.b, col.a * i.color.a);

				return col;
			}
			ENDCG
		}
	}
}

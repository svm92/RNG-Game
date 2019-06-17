Shader "Custom/MovingCirclesShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_CenterR ("Center Red", Vector) = (0.5, 0.5, 0)
		_CenterG ("Center Green", Vector) = (0.5, 0.5, 0)
		_CenterB ("Center Blue", Vector) = (0.5, 0.5, 0)
		_CenterC ("Center Cyan", Vector) = (0.5, 0.5, 0)
		_CenterM ("Center Magenta", Vector) = (0.5, 0.5, 0)
		_CenterY ("Center Yellow", Vector) = (0.5, 0.5, 0)

		_RadiusR ("Radius Red", float) = 0.2
		_RadiusG ("Radius Green", float) = 0.2
		_RadiusB ("Radius Blue", float) = 0.2
		_RadiusC ("Radius Cyan", float) = 0.2
		_RadiusM ("Radius Magenta", float) = 0.2
		_RadiusY ("Radius Yellow", float) = 0.2
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
			float2 _CenterR; float2 _CenterG; float2 _CenterB; float2 _CenterC; float2 _CenterM; float2 _CenterY;
			float _RadiusR; float _RadiusG; float _RadiusB; float _RadiusC; float _RadiusM; float _RadiusY;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				const float canvasAspectRatio = (float)16/9;
				float distanceXToCenter;
				float distanceYToCenter;
				float distanceToCenter;

				fixed r = 0;
				distanceXToCenter = abs(i.screenPos.x - _CenterR.x);
				distanceYToCenter = abs(i.screenPos.y - _CenterR.y) / canvasAspectRatio;
				distanceToCenter = sqrt(pow(distanceXToCenter, 2) + pow(distanceYToCenter, 2));
				if (distanceToCenter <= _RadiusR) {
					r = 1;
				}

				fixed g = 0;
				distanceXToCenter = abs(i.screenPos.x - _CenterG.x);
				distanceYToCenter = abs(i.screenPos.y - _CenterG.y) / canvasAspectRatio;
				distanceToCenter = sqrt(pow(distanceXToCenter, 2) + pow(distanceYToCenter, 2));
				if (distanceToCenter <= _RadiusG) {
					g = 1;
				}

				fixed b = 0;
				distanceXToCenter = abs(i.screenPos.x - _CenterB.x);
				distanceYToCenter = abs(i.screenPos.y - _CenterB.y) / canvasAspectRatio;
				distanceToCenter = sqrt(pow(distanceXToCenter, 2) + pow(distanceYToCenter, 2));
				if (distanceToCenter <= _RadiusB) {
					b = 1;
				}

				fixed c = 0;
				distanceXToCenter = abs(i.screenPos.x - _CenterC.x);
				distanceYToCenter = abs(i.screenPos.y - _CenterC.y) / canvasAspectRatio;
				distanceToCenter = sqrt(pow(distanceXToCenter, 2) + pow(distanceYToCenter, 2));
				if (distanceToCenter <= _RadiusC) {
					c = 1;
				}

				fixed m = 0;
				distanceXToCenter = abs(i.screenPos.x - _CenterM.x);
				distanceYToCenter = abs(i.screenPos.y - _CenterM.y) / canvasAspectRatio;
				distanceToCenter = sqrt(pow(distanceXToCenter, 2) + pow(distanceYToCenter, 2));
				if (distanceToCenter <= _RadiusM) {
					m = 1;
				}

				fixed y = 0;
				distanceXToCenter = abs(i.screenPos.x - _CenterY.x);
				distanceYToCenter = abs(i.screenPos.y - _CenterY.y) / canvasAspectRatio;
				distanceToCenter = sqrt(pow(distanceXToCenter, 2) + pow(distanceYToCenter, 2));
				if (distanceToCenter <= _RadiusY) {
					y = 1;
				}

				r = max(r, col.r);
				g = max(g, col.g);
				b = max(b, col.b);

				r = (2*r + (m + y)/2) / 3;
				g = (2*g + (c + y)/2) / 3;
				b = (2*b + (m + c)/2) / 3;

				col = fixed4(r, g, b, 1);

				return col;
			}
			ENDCG
		}
	}
}

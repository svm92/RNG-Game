Shader "Custom/GrayTint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			
			sampler2D _MainTex;

			half4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				fixed v = (col.r + col.g + col.b) / 3.0;
				col = fixed4(v, v, v, col.a * i.color.a);

				// Normal sprite has a value of white (1). Shadows have a lower value. Don't change shadows
				if (i.color.x != 1) // If it is a shadow
					return half4(i.color.x, i.color.y, i.color.z, col.a * i.color.a);

				return col;
			}
			ENDCG
		}
	}
}

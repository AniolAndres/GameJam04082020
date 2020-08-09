Shader "Unlit/ROBETIShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BackgroundColor("Background Color", Color) = (1, 1, 1, 1)
		_ClickzoneColor("ClickZone Color", Color) = (1, 0, 0, 1)
		_MarkColor("Mark Color", Color) = (0, 0, 0, 1)
		_Min("Min", Float) = 0.25
		_Max("Max", Float) = 0.75
		_MinRadius("Min radius", Float) = 0.25
		_MaxRadius("Max radius", Float) = 0.75
		_MarkPosition("MarkPosition", Float) = 0.5
		_MarkWidth("MarkWidth", Float) = 0.1
    }
    SubShader
    {
		Tags{"Queue" = "Transparent"}
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			float _Min;
			float _Max;
			float _MinRadius;
			float _MaxRadius;
			float _MarkPosition;
			float _MarkWidth;
			float4 _BackgroundColor;
			float4 _ClickzoneColor;
			float4 _MarkColor;

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

			float Remap(float value, float from1, float to1, float from2, float to2) {
				return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
			}

			float Length(float3 a) {
				return sqrt(a.x*a.x + a.y*a.y* +a.z*a.z);
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float4 colorBlack = float4(0.0f, 0.0f, 0.0f, 1.0f);
				float pi = 3.1415;
				float2 newUV;
				newUV.x = i.uv.x - 0.5f;
				newUV.y = i.uv.y - 0.5f;
				float remapX = Remap(newUV.x, -0.5f, 0.5f, -1.0f, 1.0f);
				float remapY = Remap(newUV.y, -0.5f, 0.5f, -1.0f, 1.0f);
				//float angle = remapX + remapY;
				//angle = step(angle, 0.5f);

				float Xpositive = step(remapX, 0.0f);
				float Ypositive = step(-remapY, 0.0f);
				float2 vectorMap = normalize(float2(remapX, remapY));
				float angle = acos(dot(vectorMap, float2(1.0f, 0.0f)))/( 2.0f * pi); //from 0 to 0.5
				float angleB = acos(dot(vectorMap, float2(-1.0f, 0.0f))) / (2.0f * pi);
				float angle2 = angle + 2.0f * angleB * lerp(1.0f, 0.0f, Ypositive);


				float stete = step(angle2, _Max);
				float4 color = lerp(_BackgroundColor, _ClickzoneColor, step(angle2, _Max) - step(angle2, _Min));

				color = lerp(color, _MarkColor, step(angle2, _MarkPosition + _MarkWidth / 2.0f) - step(angle2, _MarkPosition - _MarkWidth / 2.0f));

			

				//ALPHA stuff
				float minRadius = step(sqrt(newUV.x * newUV.x + newUV.y * newUV.y), _MinRadius);
				float maxRadius = step(sqrt(newUV.x * newUV.x + newUV.y * newUV.y), _MaxRadius);
				float circleResult = maxRadius - minRadius;
				color.a = circleResult;


                return color;
            }
            ENDCG
        }
    }
}

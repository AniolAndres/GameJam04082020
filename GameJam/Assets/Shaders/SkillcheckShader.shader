Shader "Unlit/SkillCheckShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BackgroundColor("Background Color", Color) = (1, 1, 1, 1)
		_ClickzoneColor("ClickZone Color", Color) = (1, 0, 0, 1)
		_MarkColor("Mark Color", Color) = (0, 0, 0, 1)
		_Min("Min", Float) = 0.25
		_Max("Max", Float) = 0.75
		_MarkPosition("MarkPosition", Float) = 0.5
		_MarkWidth("MarkWidth", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			float _Min;
			float _Max;
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

            v2f vert (appdata v)
            {
                v2f o;
				float4 initialPosition = v.vertex;

				float pi = 3.1415f;
				float remap = Remap(v.uv.x, 0.0f, 1.0f, -pi, 0.0f);
				float sine = 50.0f * sin(- remap);
				initialPosition.z = initialPosition.z + sine;
                o.vertex = UnityObjectToClipPos(initialPosition);
				o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float stepRight = step(i.uv.x, _Min);
				float stepLeft = step(_Max, i.uv.x);
				float4 color = lerp(_BackgroundColor, _ClickzoneColor, max(stepRight, stepLeft));

				float stepMark1 = step( _MarkPosition + _MarkWidth / 2, i.uv.x );
				float stepMark2 = step(i.uv.x, _MarkPosition - _MarkWidth / 2);

				float stepResult = stepMark1 + stepMark2;
				float remap = Remap(stepResult, 1.0f, 0.0f, 0.0f, 1.0f);
				color = lerp(color, _MarkColor, remap);

                return color;
            }
            ENDCG
        }
    }
}

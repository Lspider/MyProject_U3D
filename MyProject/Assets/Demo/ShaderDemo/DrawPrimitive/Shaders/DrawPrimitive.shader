Shader "Samples/Draw Primitive"
{
	Properties
	{
		_Origin1("Circle Origin1",vector) = (100,500,0,0)
		_Origin2("Circle Origin2",vector) = (200,500,0,0)

		_Point1("Line Point1",vector) = (300,300,0,0)
		_Point2("Line Point2",vector) = (400,2800,0,0)
		_LineWidth("Line Width",range(1,20)) = 2.0

		_Antialias("Antialias Factor", float) = 3
	}
	SubShader
	{

		Pass
		{
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag  

			#include "UnityCG.cginc"  

			float4 _Origin1;
			float4 _Origin2;

			float4 _Point1;
			float4 _Point2;
			float _LineWidth;

			float _Antialias;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 srcPos : TEXCOORD0;
				fixed3 color : COLOR0;
			};

			fixed4 DrawCircle(fixed2 pos, fixed2 center, float radius, float3 color, float antialias) {
				float d = length(pos - center) - radius;
				float t = smoothstep(0, antialias, d);
				return fixed4(color, t);
			}

			fixed4 DrawLine(fixed2 pos, fixed2 point1, fixed2 point2, float width, float3 color, float antialias) {
				//斜截式 y=kx+b
				float k = (point1.y - point2.y) / (point1.x - point2.x);
				float b = point1.y - k * point1.x;

				float d = abs(k * pos.x - pos.y + b) / sqrt(k * k + 1);
				float t = smoothstep(width / 2.0, width / 2.0 + antialias, d);
				return fixed4(color, 1.0 - t);
			}

			fixed4 draw(fixed2 pos) {

				fixed4 circle1 = DrawCircle(pos, _Origin1, 40, fixed3(0, 1, 0), _Antialias);
				fixed4 circle2 = DrawCircle(pos, _Origin2, 25, fixed3(1, 0, 0), _Antialias);
				fixed4 circle3 = DrawCircle(pos, _Point1, 10, fixed3(0, 0, 1), _Antialias);
				fixed4 circle4 = DrawCircle(pos, _Point2, 10, fixed3(0, 0, 1), _Antialias);
				fixed4 line1 = DrawLine(pos, _Point1, _Point2, _LineWidth, fixed3(0.8, 0.2, 0.5), _Antialias);

				fixed4 fragColor = fixed4(0, 0, 0, 0);
				fragColor = lerp(fragColor, circle1, circle1.a);
				//fragColor = lerp(fragColor, circle2, circle2.a);
				//fragColor = lerp(fragColor, circle3, circle3.a);
				//fragColor = lerp(fragColor, circle4, circle4.a);
				fragColor = lerp(fragColor, line1, line1.a);

				return fragColor;
			}

			v2f vert(appdata_base  v)
			{
				v2f o;
				//o.pos = UnityObjectToClipPos(v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.srcPos = ComputeScreenPos(o.pos);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//fixed2 fragCoord = i.pos;
				//return draw(fragCoord);

				//绘制圆形，此处半径使用了固定值1000和500,当然大家也可以把他们写成可调的参数
				if (pow((i.pos.x - _Origin1.x), 2) + pow((i.pos.y - _Origin1.y), 2) <1000)
				{
					return fixed4(0, 1, 0, 1);
				}
				if (pow((i.pos.x - _Origin2.x), 2) + pow((i.pos.y - _Origin2.y), 2) <500)
				{
					return fixed4(1, 0, 0, 1);
				}

				//绘制直线上两点
				if (pow((i.pos.x - _Point1.x), 2) + pow((i.pos.y - _Point1.y), 2) <100)
				{
					return fixed4(0, 0, 1, 1);
				}
				if (pow((i.pos.x - _Point2.x), 2) + pow((i.pos.y - _Point2.y), 2) <100)
				{
					return fixed4(0, 0, 1, 1);
				}

				//绘制直线 
				float d = abs((_Point2.y - _Point1.y)*i.pos.x + (_Point1.x - _Point2.x)*i.pos.y + _Point2.x*_Point1.y - _Point2.y*_Point1.x) / sqrt(pow(_Point2.y - _Point1.y, 2) + pow(_Point1.x - _Point2.x, 2));
				//小于或者等于线宽的一半时，属于直线范围  
				if (d <= _LineWidth / 2)
				{
					return fixed4(0.8, 0.2, 0.5, 1);
				}

				//绘制网格直线  
				if ((unsigned int)i.pos.x % (unsigned int)(0.25*_ScreenParams.x) == 0)
				{
					return fixed4(0, 0, 1, 1);
				}

				if ((unsigned int)i.pos.y % (unsigned int)(0.1*_ScreenParams.x) == 0)
				{
					return fixed4(0, 0, 1, 1);
				}

				return fixed4(1,1,1,1);

			}
			ENDCG
		}
	}
}
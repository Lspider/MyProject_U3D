Shader "Samples/Screen Rotation" {
	Properties{
		_MainTex("Main Tex", 2D) = "white" {}
		_Rotation("Rotation", float) = 0
	}
	SubShader{
		Tags{ "Queue" = "Geometry" }

		Pass{
			Tags{ "LightMode" = "ForwardBase" }
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			#include "UnityCG.cginc"
			#define PI 3.14159265358979  

			sampler2D _MainTex;
			float _Rototation;

			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}
			
			/*
			由“float distance = sqrt((i.uv.x - 0.5)*(i.uv.x - 0.5) +(i.uv.y - 0.5)*(i.uv.y - 0.5));”
			可以计算点到屏幕中心的距离distance。
			由于距离越远旋转角度越大，使用“_Rot *=distance”将角度增量基准与距离联系起来，即可获取需要旋转的角度：angle = _Rot*distance + A。
            由反正切公式可得∠A = atan((y - 0.5)/(x - 0.5))，由于atan的取值为[-π/2,π/2]，还需根据y值确定∠A所在的象限，
			故而∠A = step(x,0.5)*PI+ atan((y - 0.5)/(x - 0.5)) 。计算∠A 后，便可由angle = _Rot*distance + A计算总的旋转角度。
            前面已经计算了点到屏幕中心的距离distance，故而：
			x1 = 0.5 + distance *cos(angle)
			y1 = 0.5 + distance *sin(angle)
			*/
			fixed4 frag(v2f i) : SV_Target
			{
				//与中心点(0.5,0.5)的距离
				float distance = sqrt((i.uv.x - 0.5)*(i.uv.x - 0.5) + (i.uv.y - 0.5)*(i.uv.y - 0.5));
				//距离越大，旋转角度越大
				//_Rototation *= distance;
				//计算旋转角度
				float angle = step(i.uv.x,0.5)*PI + atan((i.uv.y - 0.5) / (i.uv.x - 0.5)) + _Rototation;
				//计算坐标
				i.uv.x = 0.5 + distance *cos(angle);
				i.uv.y = 0.5 + distance *sin(angle);

				fixed4 c = tex2D(_MainTex, i.uv);
				return c;
			}
			ENDCG
		}
	}
	FallBack "Specular"
}
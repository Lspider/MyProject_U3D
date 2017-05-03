Shader "Samples/Image Animation" {
	Properties{
		_MainTex("Main Tex", 2D) = "white" {}
		_ColCount("Column Count", int) = 4
		_RowCount("Row Count", int) = 1
		_Color("Color Tint", Color) = (1, 1, 1, 1)
		_Speed("Speed", Range(1, 100)) = 10
	}
	SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		//LOD 200

		Pass{
			Tags{ "LightMode" = "ForwardBase" }
			//ZTest off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			int _ColCount;
			int _RowCount;
			float _Speed;
			fixed4 _Color;

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

			//由于纹理坐标被归一化到[0,1]的范围，1/_ColCount即表示每一张小图的宽度
			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				//o.uv.x = (_ColIndex + v.texcoord.x) / _ColCount;
				//o.uv.y = (_RowIndex + v.texcoord.y) / _RowCount;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target{

				float s = floor(_Time.y * _Speed);//移动总长度
				float row = floor(s / _RowCount);//整除 每张图片高度1/_RowCount
				float column = s - row * _RowCount;//取余，得第几列（减去总行数，剩下的就是列数）

				half2 uv = i.uv + half2(column, -row);//加上当前偏移，uv坐标在左上角
				uv.x /= _RowCount;//整除列数，限定列范围
				uv.y /= _ColCount;//整除行数，限定行范围

				fixed4 c = tex2D(_MainTex, uv);
				c.rgb *= _Color;

				return c;

			}
				ENDCG
		}
	}
	FallBack "Transparent/VertexLit"
}

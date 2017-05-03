﻿Shader "Samples/Image Clip" {
	Properties{
		_MainTex("Main Tex", 2D) = "white" {}
		_ColCount("Column Count", int) = 4
		_RowCount("Row Count", int) = 4
		_ColIndex("Col Index", int) = 0
		_RowIndex("Row Index", int) = 0
	}
	SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		//LOD 200

		Pass{
			Tags{ "LightMode" = "ForwardBase" }
			ZTest off
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
			int _ColIndex;
			int _RowIndex;

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

			/*
			核心代码是“o.uv.x = (_ColIndex + v.texcoord.x)/_ColCount”和“o.uv.y = (_RowIndex + v.texcoord.y)/_RowCount”，
			它们实现了UV坐标的变换。“o.uv.x = (_ColIndex + v.texcoord.x)/_ColCount”
			即是“o.uv.x = _ColIndex*(1/_ColCount) + v.texcoord.x *(1/_ColCount)”的化简式，
			由于纹理坐标被归一化到[0,1]的范围，1/_ColCount即表示每一张小图的宽度所占百分比
			*/
			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy *_MainTex_ST.xy + _MainTex_ST.zw;

				o.uv.x = (_ColIndex + v.texcoord.x) / _ColCount;//显示一列加上第N列的偏移
					//v.texcoord.x; //显示全列
				    //v.texcoord.x / _ColCount;//显示第一列

				o.uv.y = (_RowIndex + v.texcoord.y) / _RowCount;//显示第一行加上第M行的偏移
					//v.texcoord.y;//显示全行
					//v.texcoord.y / _RowCount;//显示第一行

				 //not work
				//序列帧动图显示顺序
				//1  2  3  4
				//5  6  7  8
				//9 10 11 12

				//float rowIndex = 0;
				//float colIndex = 0;
				//float speed = 40;
				//float rowCellUVPercent = 1 / _RowCount;
				//float colCellUVPercent = 1 / _ColCount;

				//float timeVal = fmod(_Time.y * speed,_RowCount);//移动总长度
				//timeVal = ceil(timeVal);

				//float xValue = v.texcoord.x;
				//xValue += rowCellUVPercent * timeVal * _RowCount;
				//xValue *= rowCellUVPercent;

				//o.uv = float2(xValue, v.texcoord.y);
				
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				return c;
			}
			ENDCG
		}
	}
	FallBack "Transparent/VertexLit"
}

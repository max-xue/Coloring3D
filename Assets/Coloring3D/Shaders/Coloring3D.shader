// Copyright (c) 2015-2016 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
// EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
// and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
Shader "Custom/Coloring 3D" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Uvpoint1("point1", Vector) = (0 , 0 , 0 , 0)
		_Uvpoint2("point2", Vector) = (0 , 0 , 0 , 0)
		_Uvpoint3("point3", Vector) = (0 , 0 , 0 , 0)
		_Uvpoint4("point4", Vector) = (0 , 0 , 0 , 0)
	}
	SubShader{
		//Queue 队列，"RenderType"="Transparent"渲染透明类型
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		Pass{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Uvpoint1;
			float4 _Uvpoint2;
			float4 _Uvpoint3;
			float4 _Uvpoint4;

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float4  fixedPos : TEXCOORD2;
			};

			/*
			带有位置、法线和一个纹理坐标的顶点着色器输入
			struct appdata_base {
			float4 vertex : POSITION;//位置
			float3 normal : NORMAL;//法线
			float4 texcoord : TEXCOORD0;//纹理坐标
			};
			*/
			v2f vert(appdata_base v)
			{
				v2f o;
				//UNITY_MATRIX_MVP：当前模型*观察*投影矩阵
				//把顶点坐标转换到齐次裁剪坐标系下
				//用于将顶点/方向矢量从模型空间变换到裁剪空间
				o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
				//拿顶点的uv去和材质球的tiling和offset作运算，确保材质球里的缩放和偏移设置是正确的（v.texcoord就是模型顶点的uv）
				//为给定的纹理计算其uv坐标，即根据mesh上的uv坐标(v.texcoord)来计算真正的纹理上对应的位置
				//等同于 o.uv = v.texcoord.xy *_MainTex_ST.xy + _MainTex_ST.zw;
				//对顶点纹理进行运算得到最终的纹理坐标
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

				//获取修正后的坐标
				//根据当前的uv坐标获取在x方向和y方向的线性插值
				//lerp(a,b,f) ->(1-f)*a + b*f
				float4 top = lerp(_Uvpoint1, _Uvpoint3, o.uv.x);
				float4 bottom = lerp(_Uvpoint2, _Uvpoint4, o.uv.x);
				//fixedPos是世界坐标
				float4 fixedPos = lerp(bottom, top, o.uv.y);
				//UNITY_MATRIX_VP:用于将顶点/矢量从世界空间变换到裁剪空间
				//ComputeScreenPos:裁剪空间坐标转换为屏幕空间坐标
				o.fixedPos = ComputeScreenPos(mul(UNITY_MATRIX_VP, fixedPos));

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				//i.;
				//tex2D 二维纹理查询 在一张贴图中对一个点进行采样的方法，返回一个float4
				//i.fixedPos是屏幕坐标，齐次除法后得到视口坐标
				return tex2D(_MainTex, i.fixedPos.xy / i.fixedPos.w);
			}
			ENDCG
		}
	}
		//FallBack "Diffuse"
}

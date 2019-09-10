Shader "Unlit/GradientSky"
{
	Properties
	{
		_TopColor("TopColor",Color) = (1,1,1,1)
		_CenterColor("CenterColor",Color) = (1,1,1,1)
		_BottomColor("BottomColor",Color) = (1,1,1,1)
		_Exp("Exp",float) = 1
	}

	SubShader
	{
		Tags { "RenderType"="Background" "Queue" = "Background" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			fixed4 _TopColor;
			fixed4 _CenterColor;
			fixed4 _BottomColor;
			fixed _Exp;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 viewDir: TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				float3 viewDir: TEXCOORD0;
			};

	
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.viewDir = v.viewDir;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//计算当前渲染点与顶部方向的cos值并有（-1,1）转换为 0到1 区间， 
				half topDir = dot(normalize(i.viewDir),fixed3(0,1,0)) * 0.5 + 0.5;
				//计算当前渲染点与底部方向的cos值并有（-1,1）转换为 0到1 区间
				half bottomDir = dot(normalize(i.viewDir),fixed3(0,-1,0)) * 0.5 + 0.5;
				//计算当前渲染点与正前方向的cos值并有（-1,1）转换为 0到1 区间
				half centerDir = dot(normalize(i.viewDir),fixed3(0,0,1)) * 0.5 + 0.5;

				//topDir,bottomDir,centerDir 为0则表示当前渲染点与顶部方向成180度，为1则表示当前渲染点与顶部方向成0度角
				
				//计算上半部分颜色，对顶部颜色和中间颜色进行插值，越靠近顶部越接近_TopColor，pow(topDir,_Exp)使得插值的颜色变化不为线性变化，而为指数变化，_Exp越大，顶部的颜色则扩散的越小
				fixed4 color1 = lerp(_CenterColor,_TopColor,pow(topDir,_Exp));
				//计算下半部分颜色
				fixed4 color2 = lerp(_CenterColor,_BottomColor,pow(bottomDir,_Exp));
				//根据当前渲染点垂直方向的位置对上半部分颜色和下半部分颜色插值
				fixed  k = i.viewDir.y * 0.5 + 0.5;
				return lerp(color2,color1,k);
			}
			ENDCG
		}
	}
}

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
				half topDir = dot(normalize(i.viewDir),fixed3(0,1,0)) * 0.5 + 0.5;
				half bottomDir = dot(normalize(i.viewDir),fixed3(0,-1,0)) * 0.5 + 0.5;
				half centerDir = dot(normalize(i.viewDir),fixed3(0,0,1)) * 0.5 + 0.5;
				fixed4 color1 = lerp(_CenterColor,_TopColor,pow(topDir,_Exp));
				fixed4 color2 = lerp(_CenterColor,_BottomColor,pow(bottomDir,_Exp));
				fixed  k = i.viewDir.y * 0.5 + 0.5;
				return lerp(color2,color1,k);
			}
			ENDCG
		}
	}
}

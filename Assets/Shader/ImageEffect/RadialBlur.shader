Shader "Hidden/RadialBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SampleDistance("_SampleDistance",float) = 0.5
		_BlurTex("BlurTex",2D) = "white" {}
		_SampleStrength("_SampleStrength",float) = 0.5
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _SampleDistance;
			float _SampleStrength;

			fixed4 frag (v2f i) : SV_Target
			{

				//计算辐射中心点位置  
				fixed2 dir = 0.5 - i.uv;
				//计算取样像素点到中心点距离  
				fixed dist = length(dir);
				dir /= dist;
				dir *= _SampleDistance;

				fixed4 blurCol = tex2D(_MainTex, i.uv + dir * 0.01);
				blurCol += tex2D(_MainTex, i.uv + dir * 0.02);
				blurCol += tex2D(_MainTex, i.uv + dir * 0.03);
				blurCol += tex2D(_MainTex, i.uv + dir * 0.05);
				blurCol += tex2D(_MainTex, i.uv + dir * 0.08);
				blurCol += tex2D(_MainTex, i.uv - dir * 0.01);
				blurCol += tex2D(_MainTex, i.uv - dir * 0.02);
				blurCol += tex2D(_MainTex, i.uv - dir * 0.03);
				blurCol += tex2D(_MainTex, i.uv - dir * 0.05);
				blurCol += tex2D(_MainTex, i.uv - dir * 0.08);
				blurCol *= 0.1;

				return blurCol;
			}
			ENDCG
		}

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			sampler2D _BlurTex;
			float _SampleDistance;
			float _SampleStrength;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed dist = length(0.5 - i.uv);
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 blurCol = tex2D(_BlurTex, i.uv);
				return lerp(col,blurCol, saturate(dist*_SampleStrength));
			}
			
			ENDCG
		}
	}
}

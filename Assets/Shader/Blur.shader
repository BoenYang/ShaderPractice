Shader "Hidden/Blur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_offset1("offset1",float) = 1
		_offset2("offset2",float) = 1

	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		//水平方向模糊
		Pass {
			Name "BLURHORIZONTAL"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform half _offset1;
			uniform half _offset2;
			half4 _MainTex_TexelSize;


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv[5] : TEXCOORD;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				o.uv[0] = v.uv;
				o.uv[1] = v.uv + float2(_MainTex_TexelSize.x * 1.0, 0.0);
				o.uv[2] = v.uv + float2(_MainTex_TexelSize.x * 2.0, 0.0);
				o.uv[3] = v.uv - float2(_MainTex_TexelSize.x * 1.0, 0.0);
				o.uv[4] = v.uv - float2(_MainTex_TexelSize.x * 2.0, 0.0);

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col0 = tex2D(_MainTex, i.uv[0]) * 0.4026;
				fixed4 col1 = tex2D(_MainTex, i.uv[1]) * 0.2442;
				fixed4 col2 = tex2D(_MainTex, i.uv[2]) * 0.0545;
				fixed4 col3 = tex2D(_MainTex, i.uv[3]) * 0.2442;
				fixed4 col4 = tex2D(_MainTex, i.uv[4]) * 0.0545;

				return (col0 + col1 + col2 + col3 + col4); 
			}
			ENDCG
		}

		Pass{
			Name "BLURVERTICAL"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			half4 _MainTex_TexelSize;
			sampler2D _MainTex;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv[5] : TEXCOORD;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv[0] = v.uv;
				o.uv[1] = v.uv + float2(0, _MainTex_TexelSize.y * 1);
				o.uv[2] = v.uv + float2(0, _MainTex_TexelSize.y * 2);
				o.uv[3] = v.uv - float2(0, _MainTex_TexelSize.y * 1);
				o.uv[4] = v.uv - float2(0, _MainTex_TexelSize.y * 2);
				return o;
			}

		

			fixed4 frag(v2f i) : SV_Target
			{ 
				fixed4 col0 = tex2D(_MainTex, i.uv[0]) * 0.4026;
				fixed4 col1 = tex2D(_MainTex, i.uv[1]) * 0.2442;
				fixed4 col2 = tex2D(_MainTex, i.uv[2]) * 0.0545;
				fixed4 col3 = tex2D(_MainTex, i.uv[3]) * 0.2442;
				fixed4 col4 = tex2D(_MainTex, i.uv[4]) * 0.0545;
				return (col0 + col1 + col2 + col3 + col4);
			}
			
			ENDCG
		}
	}
}

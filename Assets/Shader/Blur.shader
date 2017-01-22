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
				o.uv[1] = v.uv;
				o.uv[2] = v.uv;

				o.uv[1].x -= 2 * _MainTex_TexelSize.x;
				o.uv[2].x += 2 * _MainTex_TexelSize.x;

				return o;
			}
			


			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv[0])  * 0.147761;
				fixed4 col1 = tex2D(_MainTex, i.uv[1]) * 0.118318;
				fixed4 col2 = tex2D(_MainTex, i.uv[2]) * 0.118318;

				return (col + col1 + col2) * 2.5; 
			}
			ENDCG
		}

		Pass{
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
				o.uv[1] = v.uv;
				o.uv[2] = v.uv;

				o.uv[1].y -= 2 * _MainTex_TexelSize.y;
				o.uv[2].y += 2 * _MainTex_TexelSize.y;

				return o;
			}

		

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv[0])  * 0.147761;
				fixed4 col1 = tex2D(_MainTex, i.uv[1]) * 0.118318;
				fixed4 col2 = tex2D(_MainTex, i.uv[2]) * 0.118318;
				return (col + col1 + col2)* 2.5;
			}
			
			ENDCG
		}
	}
}

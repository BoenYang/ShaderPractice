Shader "Hidden/Bloom"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LuminanceThreshold("LuminanceThreshold",float) = 0.5
		_BloomTex("BloomTex",2D) = "white" {}
	}

	SubShader {

		Cull Off ZWrite Off ZTest Always

		Pass {
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
			float _LuminanceThreshold;

			fixed lumiance(fixed4 color) {
				return color.r * 0.2125 + color.g * 0.7154 + color.b * 0.0721;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed val = clamp(lumiance(col) - _LuminanceThreshold, 0.0, 1.0);
				return val*col;
			}

			ENDCG
		}

		UsePass "Hidden/Blur/BLURHORIZONTAL"

		UsePass "Hidden/Blur/BLURVERTICAL"
	
		Pass{
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
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _BloomTex;
			half4 _MainTex_TexelSize;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv.xy = v.uv;
				o.uv.zw = v.uv;

#if UNITY_UV_STARTS_AT_TOP
				if(_MainTex_TexelSize.y < 0.0)
					o.uv.w = 1.0 - o.uv.w;
#endif

				return o;
			}



			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv.xy) + tex2D(_BloomTex,i.uv.zw);
				return col;
			}

			ENDCG
		}
	}

	Fallback off
}

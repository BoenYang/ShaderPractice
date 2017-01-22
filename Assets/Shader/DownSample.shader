Shader "Hidden/DownSample"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
				float2 uv[4] : TEXCOORD;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				#ifdef UNITY_HALF_TEXEL_OFFSET
					v.uv.x += 0.5 * 2.0f;
					v.uv.y += 0.5 * 2.0f;
				#endif
				o.uv[0] = v.uv + _MainTex_TexelSize.xy * half2(1,1);
				o.uv[1] = v.uv + _MainTex_TexelSize.xy * half2(-1, -1);
				o.uv[2] = v.uv + _MainTex_TexelSize.xy * half2(-1, 1);
				o.uv[3] = v.uv + _MainTex_TexelSize.xy * half2(1,- 1);
				return o;
			}
			
		
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 color = fixed4(0,0,0,0);
				color += tex2D(_MainTex, i.uv[0]);
				color += tex2D(_MainTex, i.uv[1]);
				color += tex2D(_MainTex, i.uv[2]);
				color += tex2D(_MainTex, i.uv[3]);
				return color/4;
			}
			ENDCG
		}
	}
}

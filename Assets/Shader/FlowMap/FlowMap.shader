Shader "Unlit/FlowMap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FlowMap("Texture",2D) = "white" {}
		_FlowPower("FlowPower",float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
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

			sampler2D _MainTex;
			sampler2D _FlowMap;
			float4 _MainTex_ST;
			float _FlowPower;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				
				float4 flowDir = tex2D(_FlowMap, i.uv) * 2.0f - 1.0f;

				float fullCircle = 1.0;
				float halfCircle = fullCircle * 0.5;

				float phase0 = frac(_Time[1]);
                float phase1 = frac(_Time[1] + halfCircle);

				half4 tex0 = tex2D(_MainTex, i.uv + flowDir.xy * phase0 * _FlowPower);
                half4 tex1 = tex2D(_MainTex, i.uv + flowDir.xy * phase1 * _FlowPower);

			    float flowLerp = abs((halfCircle - phase0) / halfCircle);
                half4 finalColor = lerp(tex0, tex1, flowLerp);

				return finalColor;
			}
			ENDCG
		}
	}
}
